using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using log4net;

namespace DHCPv6
{
    public class SocketHelper
    {
        public List<byte> ipv6;
        public List<byte> dns;
        public List<byte> domain;
        private IPEndPoint ipep; 
        private Socket client; 
        private List<byte> solicit; 
        private List<byte> advertise; 
        private List<byte> request;
        private List<byte> reply;
        private List<byte> release;

        private List<byte> address;
        private List<byte> si;
        private List<byte> noce;
        private string error;
        private List<byte> error1;
        private bool result;
        private int k;

        private ILog _log = null;

        public SocketHelper()
        {
            ipep = new IPEndPoint(IPAddress.Parse("FF02::1:2"), 547);
            client = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.IPv6Any, 546)); //绑定客户端源端口
            _log = Log4netFactory.Create(typeof(SocketHelper));
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Send(string userName, string password, int type,ref string error)
        {
            solicit = MessageHelper.GetSolicitMessage(userName, type); //设置solicit报文
            //发送solicit报文
            k = 0;
            result = false;
            bool b1 = SendSolicit(k);
            if (!b1)
            {
                error = this.error;
                return false;
            }
            string noceStr = Encoding.ASCII.GetString(noce.ToArray());
            _log.Info("随机数："+noceStr);
            request = MessageHelper.GetRequestMessage(userName, password, type, noceStr, si, address);
            //发送request报文
            k = 0;
            result = false;
            bool b2 = SendRequest(k);
            error = this.error;
            return b2;
        }

        /// <summary>
        /// 释放socket资源
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// 发送断网Release
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SendRelease(string userName, string password,int type, ref string error)
        {
            string noceStr = Encoding.ASCII.GetString(noce.ToArray());
            for (int i = 0; i < ipv6.Count; i++)
            {
                address[i + 4] = ipv6[i];
            }
            release = MessageHelper.GetReleaseMessage(userName, password, type,noceStr, si, address);
            client.SendTo(release.ToArray(), release.Count, SocketFlags.None, ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.IPv6Any, 0);
            EndPoint remote = (EndPoint)sender;
            client.ReceiveTimeout = 3000;
            try
            {
                while (true)
                {
                    byte[] recvContent = new byte[1024];
                    int length = client.ReceiveFrom(recvContent, ref remote);
                    reply = recvContent.ToList(); //赋值reply
                    bool rpy = IsReplyReEffect(); //判定reply有效性
                    _log.Info("断网Reply报文有效性："+rpy);
                    if (rpy)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Release请求："+ex.Message);
                result = false;
                error = "断网失败";
            }
            return result;
        }

        /// <summary>
        /// 验证断网Reply的有效性
        /// </summary>
        /// <returns></returns>
        public bool IsReplyReEffect()
        {
            if (reply[0] == 0x07)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送solicit报文
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private bool SendSolicit(int k)
        {
            if (k >= 3)
            {
                return result;
            }
            client.SendTo(solicit.ToArray(), solicit.Count, SocketFlags.None, ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.IPv6Any, 0);
            EndPoint remote = (EndPoint)sender;
            client.ReceiveTimeout = 6000;
            try
            {
                while (true)
                {
                    byte[] recvContent = new byte[1024];
                    int length = client.ReceiveFrom(recvContent, ref remote);
                    advertise = recvContent.ToList(); //赋值接收的数据
                    //每次绑定有效性 需要初始化参数
                    address = new List<byte>();
                    si = new List<byte>();
                    noce = new List<byte>();
                    error1 = new List<byte>();
                    int adt = IsAdvertiseEffect(); //判定advertise有效性
                    _log.Debug("Advertise报文有效性1、成功 2、失败 3、无效：" + adt);
                    if (adt == 1)
                    {
                        result = true;
                        break;
                    }
                    else if (adt == 2)
                    {
                        result = false;
                        break;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Solicit请求:" + ex.Message);
                error = "连接超时";
                k++;
                SendSolicit(k); //递归
            }
            return result;
        }

        /// <summary>
        /// 判定advertise是否有效 1、成功 2、失败 3、无效
        /// </summary>
        /// <returns></returns>
        private int IsAdvertiseEffect()
        {
            int i = 0;
            //type 1
            if (advertise[i] != 0x02)
            {
                return 3;
            }
            i = i + 1; //type

            //随机数 3
            if (solicit[1] != advertise[i] || solicit[2] != advertise[i + 1] || solicit[3] != advertise[i + 2])
            {
                return 3;
            }
            i = i + 3; //random

            //IA_NA  不做验证跳过
            i = i + 2; //ia_code
            int ia_value = bytesToInt(advertise[i], advertise[i + 1]);
            i = i + 2; //ia_len
            i = i + ia_value; //ia_value

            for (int j = i - 28; j < i; j++)
            {
                address.Add(advertise[j]); //address 赋值
            }

            //Client indentifier
            //code 2
            if (advertise[i] != 0x00 || advertise[i + 1] != 0x01)
            {
                return 3;
            }
            i = i + 2; //ci_codel
            int ci_value = bytesToInt(advertise[i], advertise[i + 1]);
            i = i + 2; //ci_len
            i = i + ci_value; //ci_value

            //Service indentifer
            //code 2
            if (advertise[i] != 0x00 || advertise[i + 1] != 0x02)
            {
                return 3;
            }
            i = i + 2; //si_code
            int si_value = bytesToInt(advertise[i], advertise[i + 1]);
            i = i + 2; //si_len
            i = i + si_value; //si_value

            for (int j = i - si_value - 4; j < i; j++)
            {
                si.Add(advertise[j]);  //server indentifer 赋值
            }

            //DNS
            i = i + 2; //dns_code
            int dns_value = bytesToInt(advertise[i], advertise[i + 1]);
            i = i + 2; //dns_len
            i = i + dns_value; //dns_value

            //Domain
            i = i + 2; //domain_code
            int domain_value = bytesToInt(advertise[i], advertise[i + 1]);
            i = i + 2; //domain_len
            i = i + domain_value; //domain_value

            //nonce
            if (advertise[i] == 0x10 && advertise[i + 1] == 0x02)
            {
                i = i + 2; //nonce_code
                int noce_value = bytesToInt(advertise[i], advertise[i + 1]);
                i = i + 2;//noce_len
                i = i + noce_value;//noce_value

                for (int j = i - noce_value; j < i; j++)
                {
                    noce.Add(advertise[j]);  //server indentifer 赋值
                }
            }
            //error
            else if (advertise[i] == 0x10 && advertise[i + 1] == 0x04)
            {
                i = i + 2; //error_code
                int error_value = bytesToInt(advertise[i], advertise[i + 1]);
                i = i + 2;//error_len
                i = i + error_value;//error_value

                for (int j = i - error_value; j < i; j++)
                {
                    error1.Add(advertise[j]);  //server indentifer 赋值
                }
                error = Encoding.ASCII.GetString(error1.ToArray());
                return 2;
            }
            else
            {
                return 3;
            }

            return 1;
        }

        /// <summary>
        /// 发送Request报文
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private bool SendRequest(int k)
        {
            if (k >= 3)
            {
                return result;
            }
            client.SendTo(request.ToArray(), request.Count, SocketFlags.None, ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.IPv6Any, 0);
            EndPoint remote = (EndPoint)sender;
            client.ReceiveTimeout = 6000;
            try
            {
                while (true)
                {
                    byte[] recvContent = new byte[1024];
                    int length = client.ReceiveFrom(recvContent, ref remote);
                    reply = recvContent.ToList(); //赋值reply
                    ipv6 = new List<byte>();
                    dns = new List<byte>();
                    domain = new List<byte>();
                    error1 = new List<byte>();
                    int rpy = IsReplyEffect(); //判定reply有效性
                    _log.Debug("Reply报文的有效性1、成功 2、失败 3、无效："+rpy);
                    if (rpy == 1)
                    {
                        result = true;
                        break;
                    }
                    else if (rpy == 2)
                    {
                        result = false;
                        break;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Request请求:" + ex.Message);
                error = "连接超时";
                k++;
                SendRequest(k);
            }
            return result;
        }

        /// <summary>
        /// 判定Reply报文是否有效 1、成功 2、失败 3、无效
        /// </summary>
        /// <returns></returns>
        private int IsReplyEffect()
        {
            if (reply[20] == 0x00 && reply[21] == 0x0d)
            {
                List<byte> errors = new List<byte>();
                int len = bytesToInt(reply[22], reply[23]) - 2;
                for (int j = 24 + 2; j < 24 + 2 + len; j++)
                {
                    errors.Add(reply[j]);
                }
                error = Encoding.ASCII.GetString(errors.ToArray());
                return 2;
            }

            int i = 0;
            //type
            if (reply[i] != 0x07)
            {
                return 3;
            }
            i = i + 1; //type

            //Random
            if (reply[i] != request[1] || reply[i + 1] != request[2] || reply[i + 2] != request[3])
            {
                return 3;
            }
            i = i + 3; //Random

            //IA_NA
            if (reply[i] != 0x00 || reply[i + 1] != 0x03)
            {
                return 3;
            }
            i = i + 2; //code
            i = i + 2; //len
            i = i + 4; //IAID
            i = i + 4; //T1
            i = i + 4; //T2
            //IA Address
            if (reply[i] != 0x00 || reply[i + 1] != 0x05)
            {
                return 3;
            }
            i = i + 2; //address_code
            int address_value = bytesToInt(reply[i], reply[i + 1]);
            i = i + 2; //address_len
            for (int j = i; j < i + 16; j++)
            {
                ipv6.Add(reply[j]);
            }
            i = i + address_value; //address_value

            //client indentifier
            //code 2
            if (reply[i] != 0x00 || reply[i + 1] != 0x01)
            {
                return 3;
            }
            i = i + 2; //codel
            int ci_value = bytesToInt(reply[i], reply[i + 1]);
            i = i + 2; //len
            i = i + ci_value; //ci_value

            //Service indentifer
            //code 2
            if (reply[i] != 0x00 || reply[i + 1] != 0x02)
            {
                return 3;
            }
            i = i + 2; //scode
            int si_value = bytesToInt(reply[i], reply[i + 1]);
            i = i + 2; //len
            i = i + si_value; //si_value

            //DNS
            //code
            if (reply[i] != 0x00 || reply[i + 1] != 0x17)
            {
                return 3;
            }
            i = i + 2; //code
            i = i + 2; //len
            for (int j = i; j < i + 16; j++)
            {
                dns.Add(reply[j]);
            }
            i = i + 16; //value

            ////Domain
            ////code
            //if (reply[i] != 0x00 || reply[i + 1] != 0x18)
            //{
            //    return 3;
            //}
            //i = i + 2; //code
            //int domain_value = bytesToInt(reply[i], reply[i + 1]);
            //i = i + 2; //len
            //for (int j = i; j < i + domain_value; j++)
            //{
            //    domain.Add(reply[j]);
            //}
            //i = i + domain_value;

            ////error
            ////code
            //if (reply[i] == 0x10 && reply[i + 1] == 0x04)
            //{
            //    i = i + 2; //code
            //    int error_value = bytesToInt(reply[i], reply[i + 1]);
            //    i = i + 2; //len
            //    for (int j = i; j < i + error_value; j++)
            //    {
            //        error1.Add(reply[j]);
            //    }
            //    error = Encoding.ASCII.GetString(error1.ToArray());
            //    return 2;
            //}
            return 1;
        }

        /// <summary>
        /// byte[]转化为int
        /// </summary>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        private int bytesToInt(byte b, byte a)
        {
            int addr = a & 0xFF;
            addr |= ((b << 8) & 0xFF00);
            return addr;
        }
    }
}