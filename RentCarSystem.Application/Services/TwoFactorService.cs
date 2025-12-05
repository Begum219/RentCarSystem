using OtpNet;
using QRCoder;
using RentCarSystem.Application.Common.Interfaces;

namespace RentCarSystem.Application.Services
{
    public class TwoFactorService : ITwoFactorService
    {
        public string GenerateSecret()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeUri(string email, string secret)
        {
            return $"otpauth://totp/RentCarSystem:{email}?secret={secret}&issuer=RentCarSystem";
        }

        public byte[] GenerateQrCode(string qrCodeUri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public bool ValidateCode(string secret, string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
        }
    }
}