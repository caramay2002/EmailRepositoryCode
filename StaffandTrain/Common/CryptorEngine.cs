using System;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

/// <summary>
/// Summary description for CryptorEngine
/// </summary>
public class CryptorEngine
{
    public static string Decrypt(string TextToBeDecrypted)
    {
        RijndaelManaged RijndaelCipher = new RijndaelManaged();
        //string Password = "PASSword@54321";
        string Password = "Hq@Umh!~vf.3[N>A";
        string DecryptedData="";
        try
        {
            //byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            //byte[] EncryptedData = Convert.FromBase64String(TextToBeDecrypted.Replace("", "+"));
            byte[] EncryptedData = Convert.FromBase64String(TextToBeDecrypted);

            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
            //Making of the key for decryption
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            //Creates a symmetric Rijndael decryptor object.
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            //Defines the cryptographics stream for decryption.THe stream contains decrpted data
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

            byte[] PlainText = new byte[EncryptedData.Length];
            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();

            //Converting to string
            DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
        }
        catch(Exception ex)
        {
            //DecryptedData = TextToBeDecrypted;
            //CPPLException objError = new CPPLException();
            //objError.CPPLErrorExceptionLogingByService(ex.ToString(), "CryptorEngine" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "CryptorEngine", "NA", "NA", TextToBeDecrypted);
            //DecryptedData = "ERROR";

        }
        return DecryptedData;
    }
    public static string Encrypt(string TextToBeEncrypted)
    {
        string EncryptedData="";
        try
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            //string Password = "PASSword@54321";
            string Password = "Hq@Umh!~vf.3[N>A";
            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToBeEncrypted);
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            //Creates a symmetric encryptor object.
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();
            //Defines a stream that links data streams to cryptographic transformations
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(PlainText, 0, PlainText.Length);
            //Writes the final state and clears the buffer
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            EncryptedData = Convert.ToBase64String(CipherBytes);
        }
        catch (Exception ex)
        {
            //DecryptedData = TextToBeDecrypted;
            //CPPLException objError = new CPPLException();
            //objError.CPPLErrorExceptionLogingByService(ex.ToString(), "CryptorEngine" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "CryptorEngine", "NA", "NA", TextToBeEncrypted);
            //EncryptedData = "ERROR";
        }
        return EncryptedData;
    }

    public static string HashPassword(string password)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        string hashedInput = HashPassword(password);
        return string.Equals(hashedInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
    }
}