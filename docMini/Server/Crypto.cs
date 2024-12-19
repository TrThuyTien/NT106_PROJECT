using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
    public static class KeyGenerator
    {
        public static (byte[] Key, byte[] IV) GenerateKeyAndIV(string pass)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] secretBytes = Encoding.UTF8.GetBytes(pass);
                byte[] hash = sha256.ComputeHash(secretBytes);

                byte[] key = new byte[32];
                Array.Copy(hash, key, key.Length);

                byte[] iv = new byte[16];
                Array.Copy(hash, hash.Length - iv.Length, iv, 0, iv.Length);

                return (key, iv);
            }
        }
    }

    public class Crypto
    {
        private readonly byte[] key;
        private readonly byte[] iv;

        public Crypto(string sharedSecret)
        {
            var (generatedKey, generatedIV) = KeyGenerator.GenerateKeyAndIV(sharedSecret);
            key = generatedKey;
            iv = generatedIV;
        }

        // Mã hóa chuỗi văn bản
        public byte[] Encrypt(string plainText)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText))
                    throw new ArgumentException("Input text cannot be null or empty.");

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption failed: {ex.Message}");
                throw;
            }
        }

        // Giải mã chuỗi mã hóa
        public string Decrypt(byte[] cipherText)
        {
            try
            {
                if (cipherText == null || cipherText.Length == 0)
                    throw new ArgumentException("Cipher text cannot be null or empty.");

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(cipherText))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption failed: {ex.Message}");
                throw;
            }
        }

        // Getter cho khóa và IV
        public byte[] GetKey() => key;
        public byte[] GetIV() => iv;
    }
}
