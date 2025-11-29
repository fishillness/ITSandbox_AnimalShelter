using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public static class EncryptionManager
{
    private static byte[] GetIV(string ivSecret)
    {
        using MD5 md5 = MD5.Create();
        return md5.ComputeHash(Encoding.UTF8.GetBytes(ivSecret));
    }

    private static byte[] GetKey(string key)
    {
        using SHA256 sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }

    public static string EncryptText(string text, string key, string iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = GetKey(key);
        aes.IV = GetIV(iv);

        using MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write);

        byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();

        byte[] encryptedBytes = memoryStream.ToArray();

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string DecryptText(string encryptedText, string key, string iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = GetKey(key);
        aes.IV = GetIV(iv);

        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

        using MemoryStream memoryStream = new MemoryStream(encryptedBytes);
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
}
