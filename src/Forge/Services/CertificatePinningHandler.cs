using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Forge.Services;

public class CertificatePinningHandler : HttpClientHandler
{
    private static readonly string[] LetsEncryptIntermediateSubjects = new[]
    {
        "R3", "R4", "R5", "R6", "R7", "R8", "R9", "R10",
        "E5", "E6", "E7", "E8", "E9", "E10",
    };

    private static readonly string[] LetsEncryptRootSubjects = new[]
    {
        "ISRG Root X1", "ISRG Root X2",
    };

    public CertificatePinningHandler()
    {
        ServerCertificateCustomValidationCallback = ValidateServerCertificate;
    }

    private bool ValidateServerCertificate(
        HttpRequestMessage request,
        X509Certificate2? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        if (certificate == null || chain == null)
            return false;

        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        if (sslPolicyErrors != SslPolicyErrors.RemoteCertificateChainErrors)
            return false;

        foreach (var status in chain.ChainStatus)
        {
            if (status.Status != X509ChainStatusFlags.NoError
                && status.Status != X509ChainStatusFlags.UntrustedRoot)
                return false;
        }

        foreach (var element in chain.ChainElements)
        {
            var subject = element.Certificate.Subject;

            foreach (var root in LetsEncryptRootSubjects)
            {
                if (subject.Contains(root, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            foreach (var intermediate in LetsEncryptIntermediateSubjects)
            {
                if (subject.Contains($"CN = {intermediate}", StringComparison.OrdinalIgnoreCase)
                    || subject.Contains($"CN={intermediate}", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return false;
    }
}
