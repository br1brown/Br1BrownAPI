
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Br1BrownAPI.Cript_Decrypt
{
    public static class ENCRYPT
    {

        public static string PASSWORD { private get; set; }

        /// <summary>
        /// Cripta il testo passatogli
        /// </summary>
        /// <param name="txt">da criptare</param>
        /// <returns>valore criptato</returns>
        public static string Crypt_Text(string txt)
        {

            if (string.IsNullOrEmpty(txt) || string.IsNullOrEmpty(PASSWORD))
                return txt;

            byte[] keys;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                var tmp = UTF8Encoding.UTF8.GetBytes(PASSWORD);
                keys = md5.ComputeHash(tmp);
            }

            using (TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                string result = "";

                byte[] byteDaCrypt = UTF8Encoding.UTF8.GetBytes(txt);

                using (ICryptoTransform transform = tripleDes.CreateEncryptor())
                {
                    byte[] results = transform.TransformFinalBlock(byteDaCrypt, 0, byteDaCrypt.Length);

                    result = Convert.ToBase64String(results, 0, results.Length);
                }

                return result;
            }


        }

        /// <summary>
        /// decripta il testo passatogli
        /// </summary>
        /// <param name="txt">da decriptare</param>
        /// <returns>valore decriptato</returns>
        public static string Decrypt_Text(string txt)
        {

            if (string.IsNullOrEmpty(txt) || string.IsNullOrEmpty(PASSWORD))
                return txt;

            byte[] keys;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                var tmp = UTF8Encoding.UTF8.GetBytes(PASSWORD);
                keys = md5.ComputeHash(tmp);
            }

            using (TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                string result = "";

                ////decripto

                byte[] byteDaDeCrypt = Convert.FromBase64String(txt);

                using (ICryptoTransform transform = tripleDes.CreateDecryptor())
                {
                    byte[] results = transform.TransformFinalBlock(byteDaDeCrypt, 0, byteDaDeCrypt.Length);

                    result = UTF8Encoding.UTF8.GetString(results);
                }

                return result;

            }
        }

    }
}
