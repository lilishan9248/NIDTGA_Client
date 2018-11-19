using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security;
using System.Security.Cryptography;


namespace DHCPv6
{
    public class SecurityHelper
    {
        #region 3DES 加密解密
        public static string DES3Encrypt(string data, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.BlockSize = 64;
            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
           

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(data);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public static string DES3Encrypt(string data, byte[] key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.BlockSize = 64;
            DES.Key = key;
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
          

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
   

            byte[] Buffer = ASCIIEncoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }


        public static string DES3Decrypt(string data, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DES.Mode = CipherMode.CBC;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            string result = "";
            try
            {
                byte[] Buffer = Convert.FromBase64String(data);
                result = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception e)
            {

            }
            return result;
        }

        #endregion

        #region DES加密解密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="key">8位字符的密钥字符串</param>
        /// <param name="iv">8位字符的初始化向量字符串</param>
        /// <returns></returns>
        public static string DESEncrypt(string data, string key, string iv)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }


        #endregion

        #region MD5签名
        public static string HMAC_MD5(string source, string key)
        {
            HMACMD5 hmacmd = new HMACMD5(Encoding.UTF8.GetBytes(key));
            byte[] inArray = hmacmd.ComputeHash(Encoding.UTF8.GetBytes(source));
            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < inArray.Length; i++)
            //{
            //    sb.Append(inArray[i].ToString("x2"));
            //}

            //hmacmd.Clear();

            //return sb.ToString().ToUpper();
            return Convert.ToBase64String(inArray);
        }

        public static string HMAC_MD5(string source, byte[] key)
        {
            HMACMD5 hmacmd = new HMACMD5(key);
            byte[] inArray = hmacmd.ComputeHash(Encoding.UTF8.GetBytes(source));
            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < inArray.Length; i++)
            //{
            //    sb.Append(inArray[i].ToString("x2"));
            //}

            //hmacmd.Clear();

            //return sb.ToString().ToUpper();
            return Convert.ToBase64String(inArray);
        }

        #endregion

        #region SHA1签名
        public static string HMAC_SHA1(string source, string key)
        {
            HMACSHA1 hmacmd = new HMACSHA1(Encoding.UTF8.GetBytes(key));
            byte[] inArray = hmacmd.ComputeHash(Encoding.UTF8.GetBytes(source));
            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < inArray.Length; i++)
            //{
            //    sb.Append(inArray[i].ToString("x2"));
            //}

            //hmacmd.Clear();

            //return sb.ToString().ToUpper(); ;
            return Convert.ToBase64String(inArray);

        }

        public static string HMAC_SHA1(string source, byte[] key)
        {
            HMACSHA1 hmacmd = new HMACSHA1(key);
            byte[] inArray = hmacmd.ComputeHash(Encoding.UTF8.GetBytes(source));
            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < inArray.Length; i++)
            //{
            //    sb.Append(inArray[i].ToString("x2"));
            //}

            //hmacmd.Clear();

            //return sb.ToString().ToUpper(); ;
            return Convert.ToBase64String(inArray);

        }
        #endregion


        /// <summary>
        /// generate private key and public key arr[0] for private key arr[1] for public key
        /// </summary>
        /// <returns></returns>
        public static string[] GenerateKeys()
        {
            string[] sKeys = new String[2];
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            sKeys[0] = rsa.ToXmlString(true);
            sKeys[1] = rsa.ToXmlString(false);
            return sKeys;
        }　

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey, string content)
        {
             RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            return Encoding.UTF8.GetString(cipherbytes);
        }
    }
}
