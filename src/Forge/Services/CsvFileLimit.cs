using System.Text;

namespace Forge.Services;

public static class CsvFileLimit
{
    public const int MaxBytes = 5 * 1024 * 1024;
    public const string ErrorMessage = "CSV troppo grande (massimo 5 MiB).";

    public static async Task<string> ReadAsync(Stream source, CancellationToken cancellationToken = default)
    {
        if (source.CanSeek && source.Length - source.Position > MaxBytes)
            throw new InvalidDataException(ErrorMessage);
        using var buffer = new MemoryStream();
        var chunk = new byte[16384];
        int read;
        while ((read = await source.ReadAsync(chunk.AsMemory(0, Math.Min(chunk.Length, MaxBytes + 1 - (int)buffer.Length)), cancellationToken)) > 0)
        {
            if (buffer.Length + read > MaxBytes) throw new InvalidDataException(ErrorMessage);
            buffer.Write(chunk, 0, read);
        }
        buffer.Position = 0;
        using var reader = new StreamReader(buffer, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
