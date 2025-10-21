using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using PROG6212POE.Models;

namespace PROG6212POE.Services
{
    public class ClaimJsonService
    {
        private readonly string _filePath = Path.Combine("Data", "claims.json");
        private readonly byte[] _key = Encoding.UTF8.GetBytes("A7C9E1G3H5J7K9L1"); // 16-byte key

        public ClaimJsonService()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, Encrypt("[]"));
        }

        public List<Claim> GetAllClaims()
        {
            var encrypted = File.ReadAllText(_filePath);
            var decrypted = Decrypt(encrypted);
            return JsonSerializer.Deserialize<List<Claim>>(decrypted) ?? new List<Claim>();
        }

        public void AddClaim(Claim claim)
        {
            var claims = GetAllClaims();
            claims.Add(claim);
            var json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, Encrypt(json));
        }

        private string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(plainText);
            return Convert.ToBase64String(ms.ToArray());
        }

        private string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[16];
            Array.Copy(fullCipher, iv, iv.Length);
            var cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor(aes.Key, iv);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        public void SaveAll(List<Claim> claims)
        {
            var json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, Encrypt(json));
        }

    }
}
