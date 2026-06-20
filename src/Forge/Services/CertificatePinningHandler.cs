using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Forge.Services;

public class CertificatePinningHandler : HttpClientHandler
{
    private static readonly string[] LetsEncryptIntermediateSubjects = new[]
    {
        "R3", "R4", "R5", "R6", "R7", "R8", "R9", "R10", "R11", "R12",
        "E5", "E6", "E7", "E8", "E9", "E10", "E11", "E12",
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
        {
            System.Diagnostics.Debug.WriteLine("[CertPin] cert or chain null");
            return false;
        }

        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            return true;
        }

        System.Diagnostics.Debug.WriteLine($"[CertPin] errors={sslPolicyErrors}");

        if (sslPolicyErrors != SslPolicyErrors.RemoteCertificateChainErrors)
        {
            System.Diagnostics.Debug.WriteLine($"[CertPin] unexpected errors: {sslPolicyErrors}");
            return false;
        }

        foreach (var status in chain.ChainStatus)
        {
            System.Diagnostics.Debug.WriteLine($"[CertPin] chain status: {status.Status}");
            if (status.Status != X509ChainStatusFlags.NoError
                && status.Status != X509ChainStatusFlags.UntrustedRoot)
            {
                return false;
            }
        }

        foreach (var element in chain.ChainElements)
        {
            var subject = element.Certificate.Subject;

            foreach (var root in LetsEncryptRootSubjects)
            {
                if (subject.Contains(root, StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"[CertPin] matched root: {root}");
                    return true;
                }
            }

            foreach (var intermediate in LetsEncryptIntermediateSubjects)
            {
                if (subject.Contains($"CN = {intermediate}", StringComparison.OrdinalIgnoreCase)
                    || subject.Contains($"CN={intermediate}", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"[CertPin] matched intermediate: {intermediate}");
                    return true;
                }
            }
        }

        System.Diagnostics.Debug.WriteLine("[CertPin] no LE cert found in chain, rejecting");
        return false;
    }
}
