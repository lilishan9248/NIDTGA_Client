using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DHCPv6
{
    public class MessageHelper
    {
        //solicit 报文
        public static List<byte> GetSolicitMessage(string userName,int type)
        {
            List<byte> solicit = new List<byte>();

            //msg_type(固定值) 1
            solicit.Add(0x01);

            //Transaction id（随机数）3
            solicit.Add(CommonHelper.GetRandom());
            solicit.Add(CommonHelper.GetRandom());
            solicit.Add(CommonHelper.GetRandom());

            //Client identifier(固定字节) 18
            //code（固定值） 2
            solicit.Add(0x00);
            solicit.Add(0x01);
            //length(固定) 2
            solicit.Add(0x00);
            solicit.Add(0x0e);
            //DUID type 2
            solicit.Add(0x00);
            solicit.Add(0x01);
            //DUID hardType 2
            solicit.Add(0x00);
            solicit.Add(0x01);
            //DUID time 4               待处理
            solicit.Add(0x1d);
            solicit.Add(0x12);
            solicit.Add(0x1c);
            solicit.Add(0xe8);
            //DUID mac 6
            foreach (var item in CommonHelper.GetMacByte())
            {
                solicit.Add(item);
            }

            //option request
            //option code 2
            solicit.Add(0x00);
            solicit.Add(0x06);
            //option length 2
            solicit.Add(0x00);
            solicit.Add(0x04);
            //dns 2
            solicit.Add(0x00);
            solicit.Add(0x17);
            //domain 2
            solicit.Add(0x00);
            solicit.Add(0x18);

            //Elapsed time
            //elapsed code 2
            solicit.Add(0x00);
            solicit.Add(0x08);
            //elapsed lenth 2
            solicit.Add(0x00);
            solicit.Add(0x02);
            //time 2
            solicit.Add(0x00);
            solicit.Add(0x00);

            //IA_NA
            //ia code 2
            solicit.Add(0x00);
            solicit.Add(0x03);
            //ia length 2
            solicit.Add(0x00);
            solicit.Add(0x0c);
            //IAID 4
            List<byte> mac = CommonHelper.GetMacByte();
            solicit.Add(mac[2]);
            solicit.Add(mac[3]);
            solicit.Add(mac[4]);
            solicit.Add(mac[5]);
            //T1 4
            solicit.Add(0x00);
            solicit.Add(0x00);
            solicit.Add(0x0e);
            solicit.Add(0x10);
            //T2 4
            solicit.Add(0x00);
            solicit.Add(0x00);
            solicit.Add(0x15);
            solicit.Add(0x18);
            //IA address       不需要

            //username
            char[] c = userName.ToCharArray();
            byte[] be = Encoding.Default.GetBytes(c);
            switch (type)
            {
                case 1:
                    //NID
                    //code 2
                    solicit.Add(0x10);
                    solicit.Add(0x01);
                    //length 2
                    solicit.Add(0x00);
                    solicit.Add(0x05);
                    //nid 4-8
                    foreach (var item in CommonHelper.NidToByte(userName))
                    {
                        solicit.Add(item);
                    }
                    break;
                case 2:
                    //SID学号
                    //code 2
                    solicit.Add(0x10);
                    solicit.Add(0x10);
                    byte[] b = Encoding.Default.GetBytes(userName);
                    //length 2
                    byte[] bl = BitConverter.GetBytes(b.Length);
                    solicit.Add(0x00);
                    foreach (var item in bl)
                    {
                        if (item != 0x00)
                        {
                            solicit.Add(item);
                        }
                    }
                    //字符串
                    foreach (var item in be)
                    {
                        solicit.Add(item);
                    }
                    break;
                case 3:
                    //MID手机号
                    //code 2
                    solicit.Add(0x10);
                    solicit.Add(0x11);
                    //length 2
                    solicit.Add(0x00);
                    solicit.Add(0x0b);
                    //字符串
                    foreach (var item in be)
                    {
                        solicit.Add(item);
                    }
                    break;
                default:
                    break;
            }
            
            return solicit;
        }

        //request 报文
        public static List<byte> GetRequestMessage(string userName, string password,int type, string noce, List<byte> si, List<byte> address)
        {
            List<byte> request = new List<byte>();

            //type 1
            request.Add(0x03);

            //Random 3
            request.Add(CommonHelper.GetRandom());
            request.Add(CommonHelper.GetRandom());
            request.Add(CommonHelper.GetRandom());

            //Client Identifier
            //code（固定值） 2
            request.Add(0x00);
            request.Add(0x01);
            //length(固定) 2
            request.Add(0x00);
            request.Add(0x0e);
            //DUID type 2
            request.Add(0x00);
            request.Add(0x01);
            //DUID hardType 2
            request.Add(0x00);
            request.Add(0x01);
            //DUID time 4               待处理
            request.Add(0x1d);
            request.Add(0x12);
            request.Add(0x1c);
            request.Add(0xe8);
            //DUID mac 6
            foreach (var item in CommonHelper.GetMacByte())
            {
                request.Add(item);
            }

            //Server Indentifier
            foreach (byte b in si)
            {
                request.Add(b);
            }

            //Option request
            //code 2
            request.Add(0x00);
            request.Add(0x06);
            //length 2
            request.Add(0x00);
            request.Add(0x04);
            //dns 2
            request.Add(0x00);
            request.Add(0x17);
            //domain 2
            request.Add(0x00);
            request.Add(0x18);

            //Elapsed time
            //code 2
            request.Add(0x00);
            request.Add(0x08);
            //lenth 2
            request.Add(0x00);
            request.Add(0x02);
            //time 2
            request.Add(0x00);
            request.Add(0x00);

            //IA_NA
            //code 2
            request.Add(0x00);
            request.Add(0x03);
            //length 2
            request.Add(0x00);
            request.Add(0x28); //len 40
            //IAID 4
            List<byte> mac = CommonHelper.GetMacByte();
            request.Add(mac[2]);
            request.Add(mac[3]);
            request.Add(mac[4]);
            request.Add(mac[5]);
            //T1 4
            request.Add(0x00);
            request.Add(0x00);
            request.Add(0x0e);
            request.Add(0x10);
            //T2 4
            request.Add(0x00);
            request.Add(0x00);
            request.Add(0x15);
            request.Add(0x18);
            //IA address   //24
            foreach (var item in address)
            {
                request.Add(item);
            }

            //username
            char[] c = userName.ToCharArray();
            byte[] be = Encoding.Default.GetBytes(c);
            switch (type)
            {
                case 1:
                    //NID
                    //code 2
                    request.Add(0x10);
                    request.Add(0x01);
                    //length 2
                    request.Add(0x00);
                    request.Add(0x05);
                    //nid 4-8
                    foreach (var item in CommonHelper.NidToByte(userName))
                    {
                        request.Add(item);
                    }
                    break;
                case 2:
                    //SID学号
                    //code 2
                    request.Add(0x10);
                    request.Add(0x10);
                    byte[] b = Encoding.Default.GetBytes(userName);
                    //length 2
                    byte[] bl = BitConverter.GetBytes(b.Length);
                    request.Add(0x00);
                    foreach (var item in bl)
                    {
                        if (item != 0x00)
                        {
                            request.Add(item);
                        }
                    }
                    //字符串
                    foreach (var item in be)
                    {
                        request.Add(item);
                    }
                    break;
                case 3:
                    //MID手机号
                    //code 2
                    request.Add(0x10);
                    request.Add(0x11);
                    //length 2
                    request.Add(0x00);
                    request.Add(0x0b);
                    //字符串
                    foreach (var item in be)
                    {
                        request.Add(item);
                    }
                    break;
                default:
                    break;
            }

            //Password Digest
            request.Add(0x10);
            request.Add(0x03);
            //length
            request.Add(0x00);
            request.Add(0x20);
            foreach (byte item in CommonHelper.StrToMd5Byte(CommonHelper.StrToMd5Str(password) + noce))
            {
                request.Add(item);
            }

            //Mac
            //code 2
            request.Add(0x10);
            request.Add(0x06);
            //length 2
            request.Add(0x00);
            request.Add(0x06);
            //mac
            foreach (var item in CommonHelper.GetMacByte())
            {
                request.Add(item);
            }
            return request;
        }

        public static List<byte> GetReleaseMessage(string userName, string password, int type, string noce, List<byte> si, List<byte> address)
        {
            List<byte> request = new List<byte>();

            //type 1
            request.Add(0x08);

            //Random 3
            request.Add(CommonHelper.GetRandom());
            request.Add(CommonHelper.GetRandom());
            request.Add(CommonHelper.GetRandom());

            //Client Identifier
            //code（固定值） 2
            request.Add(0x00);
            request.Add(0x01);
            //length(固定) 2
            request.Add(0x00);
            request.Add(0x0e);
            //DUID type 2
            request.Add(0x00);
            request.Add(0x01);
            //DUID hardType 2
            request.Add(0x00);
            request.Add(0x01);
            //DUID time 4               待处理
            request.Add(0x1d);
            request.Add(0x12);
            request.Add(0x1c);
            request.Add(0xe8);
            //DUID mac 6
            foreach (var item in CommonHelper.GetMacByte())
            {
                request.Add(item);
            }

            //Server Indentifier
            foreach (byte b in si)
            {
                request.Add(b);
            }

            //Option request
            //code 2
            request.Add(0x00);
            request.Add(0x06);
            //length 2
            request.Add(0x00);
            request.Add(0x04);
            //dns 2
            request.Add(0x00);
            request.Add(0x17);
            //domain 2
            request.Add(0x00);
            request.Add(0x18);

            //Elapsed time
            //code 2
            request.Add(0x00);
            request.Add(0x08);
            //lenth 2
            request.Add(0x00);
            request.Add(0x02);
            //time 2
            request.Add(0x00);
            request.Add(0x00);

            //IA_NA
            //code 2
            request.Add(0x00);
            request.Add(0x03);
            //length 2
            request.Add(0x00);
            request.Add(0x28); //len 40
            //IAID 4
            List<byte> mac = CommonHelper.GetMacByte();
            request.Add(mac[2]);
            request.Add(mac[3]);
            request.Add(mac[4]);
            request.Add(mac[5]);
            //T1 4
            request.Add(0x00);
            request.Add(0x00);
            request.Add(0x0e);
            request.Add(0x10);
            //T2 4
            request.Add(0x00);
            request.Add(0x00);
            request.Add(0x15);
            request.Add(0x18);

            //IA address
            foreach (var item in address)
            {
                request.Add(item);
            }

            //username
            char[] c = userName.ToCharArray();
            byte[] be = Encoding.Default.GetBytes(c);
            switch (type)
            {
                case 1:
                    //NID
                    //code 2
                    request.Add(0x10);
                    request.Add(0x01);
                    //length 2
                    request.Add(0x00);
                    request.Add(0x05);
                    //nid 4-8
                    foreach (var item in CommonHelper.NidToByte(userName))
                    {
                        request.Add(item);
                    }
                    break;
                case 2:
                    //SID学号
                    //code 2
                    request.Add(0x10);
                    request.Add(0x10);
                    byte[] b = Encoding.Default.GetBytes(userName);
                    //length 2
                    byte[] bl = BitConverter.GetBytes(b.Length);
                    request.Add(0x00);
                    foreach (var item in bl)
                    {
                        if (item != 0x00)
                        {
                            request.Add(item);
                        }
                    }
                    //字符串
                    foreach (var item in be)
                    {
                        request.Add(item);
                    }
                    break;
                case 3:
                    //MID手机号
                    //code 2
                    request.Add(0x10);
                    request.Add(0x11);
                    //length 2
                    request.Add(0x00);
                    request.Add(0x0b);
                    //字符串
                    foreach (var item in be)
                    {
                        request.Add(item);
                    }
                    break;
                default:
                    break;
            }
            //Password Digest
            request.Add(0x10);
            request.Add(0x03);
            //length
            request.Add(0x00);
            request.Add(0x20);
            foreach (byte item in CommonHelper.StrToMd5Byte(CommonHelper.StrToMd5Str(password) + noce))
            {
                request.Add(item);
            }

            //Mac
            //code 2
            request.Add(0x10);
            request.Add(0x06);
            //length 2
            request.Add(0x00);
            request.Add(0x06);
            //mac
            foreach (var item in CommonHelper.GetMacByte())
            {
                request.Add(item);
            }
            return request;
        }
    }
}