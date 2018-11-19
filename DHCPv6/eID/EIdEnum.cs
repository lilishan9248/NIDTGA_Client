using System;

namespace DHCPv6
{
    public class EIdConfig
    {
        public const string APPID = "01QT1605101028011111";
        public const string KEY = "11111111111111111111111111111111";

        public const string Encrypt_factor = "A2A5BEF6839D0ABB"; //加密因子
        public const string Sign_factor = "BE56E78AE93BF7DE"; //签名因子
    }


    public enum EIdType
    {
        ID_CARD = 01,
    }

    public enum EEncryptType
    {
        TRIPLE_DES_ECB_PKCS5PADDING = 1,
        SM4_ECB_PKCS5Padding = 2,
    }

    public enum ESignType
    {
        HMAC_SHA1 = 1,
        HMAC_SHA256 = 2,
        HMAC_MD5 = 3,
        HMAC_SM3 = 4,
    }

    public enum ESecurityType
    {
        SKEY = 10, //还有一个加密机是30
    }
}
