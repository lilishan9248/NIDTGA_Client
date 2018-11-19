using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace DHCPv6
{
    public class CommonHelper
    {
        /// <summary>
        /// 获取mac地址
        /// </summary>
        /// <returns></returns>
        public static List<byte> GetMacByte()
        {
            List<byte> macByte = new List<byte>();
            string[] macs = GetMac().Split(new char[] { ':' });
            Dictionary<string, byte> dic = InitDictionaryByte();
            foreach (var item in macs)
            {
                macByte.Add(dic[item.ToLower()]);
            }
            return macByte;
        }

        /// <summary>
        /// Nid转化为16进制
        /// </summary>
        /// <param name="nid"></param>
        /// <returns></returns>
        public static List<byte> NidToByte(string nid)
        {
            List<byte> nidByte = new List<byte>();
            Dictionary<string, byte> dic = InitDictionaryByte();
            for (int i = 0; i <= 8; i = i + 2)
            {
                nidByte.Add(dic[nid.Substring(i, 2)]);
            }
            return nidByte;
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <returns></returns>
        public static byte GetRandom()
        {
            Random r = new Random();
            int num = r.Next(255);
            byte b = 0x00;
            return (byte)(b + num);
        }

        //MD5加密
        public static string StrToMd5Str(string str)
        {
            //WriteLog(str);
            byte[] result = Encoding.ASCII.GetBytes(str.Trim());    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string md5str = BitConverter.ToString(output).Replace("-", "").ToLower();  //tbMd5pass为输出加密文本的文本框
            //WriteLog(md5str);
            return md5str;
        }

        //MD5加密
        public static byte[] StrToMd5Byte(string str)
        {
            //WriteLog(str);
            byte[] result = Encoding.ASCII.GetBytes(str.Trim());    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string md5str = BitConverter.ToString(output).Replace("-", "").ToLower();  //tbMd5pass为输出加密文本的文本框
            //WriteLog(md5str);
            return Encoding.ASCII.GetBytes(md5str);
        }

        //byte转化为string
        public static string ByteToString(List<byte> bytes)
        {
            Dictionary<byte, string> dic = InitDictionaryString();
            string str = string.Empty;
            foreach (var item in bytes)
            {
                str += dic[item];
            }
            return str;
        }

        //sting转化为ipv6格式
        public static string StrToIpv6(string str)
        {
            string s = string.Empty;
            for (int i = 0; i < str.Length; i = i + 4)
            {
                string ss = StrStarTirm0(str.Substring(i, 4));
                s += ss + ":";
            }
            return s.Substring(0, s.Length - 1);
        }

        public static void WriteLog(string content)
        {
            if (bool.Parse(ConfigHelper.GetSetting("log")))
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
                StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
                sw.WriteLine(DateTime.Now + "  " + content);
                sw.Close();
            }
        }

        private static string StrStarTirm0(string str)
        {
            string s = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                string t = str.Substring(i, 1);
                if (t == "0" && s.Length == 0)
                {
                    continue;
                }
                s = s + t;
            }
            return s == string.Empty ? "0" : s;
        }

        //获取mac地址
        private static string GetMac()
        {
            List<string> macs = new List<string>();
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString();
                        macs.Add(mac);
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            {
            }

            return macs[0];
        }

        //填充16进制数据字典
        private static Dictionary<string, byte> InitDictionaryByte()
        {
            Dictionary<string, byte> dic = new Dictionary<string, byte>();
            byte value = 0x00;
            for (int i = 0; i <= 15; i++)
            {
                string a = "0", b = "0";
                if (i < 10)
                {
                    a = i.ToString();
                }
                else
                {
                    switch (i)
                    {
                        case 10: a = "a"; break;
                        case 11: a = "b"; break;
                        case 12: a = "c"; break;
                        case 13: a = "d"; break;
                        case 14: a = "e"; break;
                        case 15: a = "f"; break;
                    }
                }
                for (int j = 0; j <= 15; j++)
                {
                    if (j < 10)
                    {
                        b = j.ToString();
                    }
                    else
                    {
                        switch (j)
                        {
                            case 10: b = "a"; break;
                            case 11: b = "b"; break;
                            case 12: b = "c"; break;
                            case 13: b = "d"; break;
                            case 14: b = "e"; break;
                            case 15: b = "f"; break;
                        }
                    }
                    string key = a + b;
                    dic.Add(key, value);
                    value = (byte)(value + 1);
                }
            }
            return dic;
        }

        private static Dictionary<byte, string> InitDictionaryString()
        {
            Dictionary<byte, string> dic = new Dictionary<byte, string>();
            byte key = 0x00;
            for (int i = 0; i <= 15; i++)
            {
                string a = "0", b = "0";
                if (i < 10)
                {
                    a = i.ToString();
                }
                else
                {
                    switch (i)
                    {
                        case 10: a = "a"; break;
                        case 11: a = "b"; break;
                        case 12: a = "c"; break;
                        case 13: a = "d"; break;
                        case 14: a = "e"; break;
                        case 15: a = "f"; break;
                    }
                }
                for (int j = 0; j <= 15; j++)
                {
                    if (j < 10)
                    {
                        b = j.ToString();
                    }
                    else
                    {
                        switch (j)
                        {
                            case 10: b = "a"; break;
                            case 11: b = "b"; break;
                            case 12: b = "c"; break;
                            case 13: b = "d"; break;
                            case 14: b = "e"; break;
                            case 15: b = "f"; break;
                        }
                    }
                    string value = a + b;
                    dic.Add(key, value);
                    key = (byte)(key + 1);
                }
            }
            return dic;
        }
    }
}