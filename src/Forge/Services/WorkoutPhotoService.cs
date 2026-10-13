namespace Forge.Services;

public static class WorkoutPhotoService
{
    public static async Task<string> PrepareAsync(FileResult file)
    {
        using var stream = await file.OpenReadAsync();
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        var bytes = buffer.ToArray();
#if ANDROID
        bytes = await Task.Run(() => {
            using var bounds = new Android.Graphics.BitmapFactory.Options { InJustDecodeBounds = true };
            Android.Graphics.BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, bounds);
            var sample = 1;
            while (Math.Max(bounds.OutWidth, bounds.OutHeight) / sample > 1400) sample *= 2;
            using var options = new Android.Graphics.BitmapFactory.Options { InSampleSize = sample };
            using var original = Android.Graphics.BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options)
                ?? throw new IOException("Immagine non leggibile.");
            using var jpeg = new MemoryStream();
            for (var quality = 85; quality >= 35; quality -= 10)
            {
                jpeg.SetLength(0);
                original.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg!, quality, jpeg);
                if (jpeg.Length <= 300_000) break;
            }
            return jpeg.ToArray();
        });
#endif
        return "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
    }
}
