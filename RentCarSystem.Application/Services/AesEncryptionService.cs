//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace RentCarSystem.Application.Services
//{
//    public class AesEncryptionService : IEncrytionService
//    {
//        private readonly byte[] _key;
//        private readonly byte[] _iv;

//        public AesEncryptionService (IConfiguration configuration)
//        {
//            //appsettings.json'dan şifreleme anahtarı alınıcak 
//            var encrptionKey = configuration["Encryption:Key"]
//                ?? throw new InvalidOperationException("Encryption:Key bulunamadı ");
//            var encryptionIv = configuration["Encryption:IV"]
//                ?? throw new InvalidOperationException("Encryption : IV bulunamadı");
//            _key = Convert.FromBase64String(encryptionKey);
//            _iv = Convert.FromBase64String(encryptionIv);



//        }
//        public string Encrypt (string plainText)
//        {
//            if (string.IsNullOrEmpty(plainText))
//                return plainText;

//            using var aes = Aes.Create();
//            aes.Key = _key;
//            aes.IV= _iv;

//            var encryptor = aes.CreateEncryptor(aes.Key,aes.IV);

//           using var msEncrypt = new MemoryStream();
//            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
//            using (var swEncrypt = new StreamWriter(csEncrypt))
//            {

//                swEncrypt.Write(plainText);

//            }

//            return Convert.ToBase64String(msEncrypt.ToArray());
//        }
//        public string Decrypt (string cipherText)
//        {
//            if (string.IsNullOrEmpty(cipherText))
//                return cipherText;

//            try
//            {
//                using var aes = Aes.Create();
//                aes.Key = _key;
//                aes.IV = _iv;
//                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

//                using var msDecrypt = new MemoryStream(Convert.FromBase64String (cipherText));
//                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
//                using var srDecrypt = new StreamReader(csDecrypt);
//                return srDecrypt.ReadToEnd();


//            }
//            catch (Exception)
//            {
//                return cipherText;
//                // eğer decryp 
//            }

//        }
//    }
//}
