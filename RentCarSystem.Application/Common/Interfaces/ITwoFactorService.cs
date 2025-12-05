namespace RentCarSystem.Application.Common.Interfaces
{
    public interface ITwoFactorService
    {
        string GenerateSecret();
        string GenerateQrCodeUri(string email, string secret);
        byte[] GenerateQrCode(string qrCodeUri);
        bool ValidateCode(string secret, string code);
    }
}