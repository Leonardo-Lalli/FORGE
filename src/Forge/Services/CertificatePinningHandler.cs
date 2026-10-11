namespace Forge.Services;

// Use the platform's certificate chain, hostname and expiry validation.
// Subject names are not cryptographic pins and must never override TLS errors.
// This retains compatibility with any CA trusted by a self-hosting user's device.
public sealed class CertificatePinningHandler : HttpClientHandler
{
}
