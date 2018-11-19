using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using LitJson;
using Com.Itrus.Crypto;

namespace DHCPv6
{
    public class PkiRNVParams
    {
        public UserIdInfo userinfo;
        public EidCertId certid;
        public SecurityFactor securityfactor;
        private string PIN; //PIN码
        //构造
        public PkiRNVParams()
        {
            userinfo = new UserIdInfo();
            userinfo.idnum = "";
            userinfo.idtype = "01";
            userinfo.name = "";

            securityfactor = new SecurityFactor();
            securityfactor.encrypt_factor = EIdConfig.Encrypt_factor;
            securityfactor.sign_factor = EIdConfig.Sign_factor;

            certid = new EidCertId();
            certid.eid_issuer = "";
            certid.eid_issuer_sn = "";
            certid.eid_sn = "";

        }


        public void SetUserName(string value)
        {
            this.userinfo.name = value;
        }

        public void SetUserIDNum(string value)
        {
            this.userinfo.idnum = value;
        }

        public void SetEIDUser(string value)
        {
            this.certid.eid_issuer = value;
        }

        public void SetEIDUserSN(string value)
        {
            this.certid.eid_issuer_sn = value;
        }

        public void SetEIDSN(string value)
        {
            this.certid.eid_sn = value;
        }

        public void SetPIN(string value)
        {
            this.PIN = value;
        }

        public void init()
        {
            this.SetValue("version", "2.0.0");
            this.SetValue("app_id", EIdConfig.APPID);
            this.SetValue("biz_type", "0101001");
            this.SetValue("biz_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
            this.SetValue("biz_sequence_id", GenerateNonceStr());

            this.SetValue("security_factor", securityfactor);
            this.SetValue("encrypt_type", "1");//1为3DES,2为SM4
            this.SetValue("sign_type", "3");//3是MD5
            this.SetValue("security_type", "10");

            this.SetValue("attach", "123456");

            this.SetValue("user_id_info", BuildSecurityUser());
            this.SetValue("eid_cert_id", certid);

            string data_to_sign = this.BuildDataToSign();
            this.SetValue("data_to_sign", this.BuildDataToSignEncrypt(data_to_sign, this.GetValue("encrypt_type").ToString()));
            this.SetValue("eid_sign_algorithm", "20");
            this.SetValue("eid_sign", this.BuildEidSign(data_to_sign, this.GetValue("eid_sign_algorithm").ToString()));
            this.SetValue("sign", this.MakeSign());
        }



        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /**
      * 设置某个字段的值
      * @param key 字段名
       * @param value 字段值
      */
        private void SetValue(string key, object value)
        {
            m_values[key] = value;
        }
        /**
        * 根据字段名获取某个字段的值
        * @param key 字段名
         * @return key对应的字段值
        */
        private object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }


        /**
         * 判断某个字段是否已设置
         * @param key 字段名
         * @return 若字段key已被设置，则返回true，否则返回false
         */
        private bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        /**
               * @Dictionary格式化成Json
                * @return json串数据
               */
        public string ToJson()
        {
            //先检查

            string jsonStr = JsonMapper.ToJson(m_values);
            return jsonStr;
        }

        /**
       * @Dictionary格式转化成url参数格式
       * @ return url格式串, 该串不包含sign字段值
       */
        private string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    // throw new WxPayException("WxPayData内部含有值为null的字段!");
                }
                //if (pair.Key == "user_id_info") //对象user处理
                //{
                //    UserIdInfo o = (UserIdInfo)pair.Value;
                //    buff += "user_id_info=idnum=" + o.idnum + "&idtype=" + o.idtype + "&name=" + o.name + "&";
                //}
                else if (pair.Key == "security_factor") //对象security_factor处理
                {
                    SecurityFactor o = (SecurityFactor)pair.Value;
                    buff += "security_factor=encrypt_factor=" + o.encrypt_factor + "&sign_factor=" + o.sign_factor + "&";
                }
                else if (pair.Key == "eid_cert_id") //对象eid_cert_id处理
                {
                    EidCertId o = (EidCertId)pair.Value;
                    buff += "eid_cert_id=eid_issuer=" + o.eid_issuer + "&eid_issuer_sn=" + o.eid_issuer_sn + "&eid_sn=" + o.eid_sn + "&";
                }
                else
                {
                    if (pair.Key != "sign" && pair.Value.ToString() != "")
                    {
                        buff += pair.Key + "=" + pair.Value + "&";
                    }
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        /**
       * @生成签名，详见签名生成算法
       * @return 签名, sign字段不参加签名
       */
        private string MakeSign()
        {
            //转url格式
            string str = ToUrl();

            //获密钥
            byte[] bytes = buildSM4Key(EIdConfig.Sign_factor, EIdConfig.KEY);

            return SecurityHelper.HMAC_MD5(str, bytes);
        }

        //生成待签原文
        private string BuildDataToSign()
        {
            //签名时间:流水号:业务数据
            //string result = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + Guid.NewGuid().ToString().Replace("-", "") + ":" + Guid.NewGuid().ToString().Replace("-", "");

            string data = DateTime.Now.ToString("yyyyMMddHHmmss") + ":" + DateTime.Now.ToString("yyyyMMddHHmmss") + GetInviteCode(4) + ":" + GetInviteCode(12);

            //Console.WriteLine("原签名:" + data);
            byte[] bytedata = Encoding.UTF8.GetBytes(data);

            string data64 = Convert.ToBase64String(bytedata);
            //Console.WriteLine("原签名64值:" + data64);
            return data64;
        }

        //生成待签原文加密
        private string BuildDataToSignEncrypt(string DataToSign, string EncryptType)
        {
            string Security = "";
            if (EncryptType == "1")//DES3
            {
                byte[] keys = buildSM4Key(EIdConfig.Encrypt_factor, EIdConfig.KEY);
                Security = SecurityHelper.DES3Encrypt(DataToSign, keys);
            }
            else//非对称-2
            {
            }
            return Security;
        }

        //生成eid 签名
        private string BuildEidSign(string DataToSign, string eid_sign_algorithm)
        {
            string result = "";
            if (eid_sign_algorithm == "20")//非对称-1
            {
                string error = "";
                result = UAIeID.Test(DataToSign, this.PIN, out error);
            }
            else//非对称-2
            {

            }
            return result;
        }

        //用户加密
        private string BuildSecurityUser()
        {
            string data = JsonMapper.ToJson(this.userinfo);
            byte[] keys = buildSM4Key(EIdConfig.Encrypt_factor, EIdConfig.KEY);

            string Security = SecurityHelper.DES3Encrypt(data, keys);

            return Security;
        }

        private byte[] buildSM4Key(String factor, String key)
        {
            byte[] keys = HexStringToBytes(key);
            return Sm4.EncriptB(keys, getFactor(factor));
        }

        private byte[] getFactor(String signFactor)
        {
            byte[] factor = HexStringToBytes(signFactor);
            byte[] ret = new byte[16];
            for (int i = 0; i < factor.Length; i++)
            {
                ret[i] = factor[i];
                ret[i + factor.Length] = (byte)(factor[i] ^ 0xFFFFFFFF);
            }
            return ret;
        }

        private static byte[] HexStringToBytes(string source)
        {
            if (source.Length % 2 != 0)
            {
                throw new ArgumentException("Argument must be divisible by two.", "source");
            }

            byte[] bytes = new byte[source.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(source.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        private static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;

            if (bytes != null)
            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {

                    strB.Append(bytes[i].ToString("X2"));

                }

                hexString = strB.ToString();

            } return hexString;

        }

        //获取随机邀请码
        private string GetInviteCode(int num)
        {
            string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] chastr = str.ToCharArray();
            string code = "";

            long tick = DateTime.Now.Ticks;

            Random rd = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            int i;
            for (i = 0; i < num; i++)
            {
                code += str.Substring(rd.Next(0, str.Length), 1);
            }
            return code;
        }
        private string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }

    //内部对象
    public class SecurityFactor //安全因子
    {
        public string encrypt_factor;
        public string sign_factor;
    }


    public class UserIdInfo //实名信息
    {
        public string name;
        public string idnum;
        public string idtype;
    }

    public class EidCertId //eID证书信息
    {
        public string eid_issuer;
        public string eid_issuer_sn;
        public string eid_sn;
    }

    //返回对象
    public class ResponseData
    {
        public string version;
        public string biz_sequence_id;
        public SecurityFactor security_factor;
        public string encrypt_type;
        public string sign_type;
        public string sign;
        public string security_type;
        public string attach;
        public string result;
        public string result_detail;
        public string result_time;
        public string user_info;
        public string extension;

    }
}
