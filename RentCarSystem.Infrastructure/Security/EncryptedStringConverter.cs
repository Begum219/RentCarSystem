using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RentCarSystem.Infrastructure.Security
{
    public class EncryptedStringConverter : ValueConverter<string?, string?>
    {
        public EncryptedStringConverter(IAesEncryptionService encryptionService)
            : base(
                v => encryptionService.Encrypt(v!),
                v => encryptionService.Decrypt(v!))
        {
        }
    }
}