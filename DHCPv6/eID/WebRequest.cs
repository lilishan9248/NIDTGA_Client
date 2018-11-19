using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Configuration;
using System.Net;
 

namespace DHCPv6
{
    public class WebUtil
    {
        public static string doPost(string url, string requestParams, out string error)
        {
            
            string str = "";
            try
            {
                //制定服务器地址
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post; //设定http 的传递方式
                request.Timeout = 100000;
                request.ReadWriteTimeout = 100000;

                request.ContentType = "application/json;charset=UTF-8";
                byte[] bytes = Encoding.UTF8.GetBytes(requestParams);
                request.ContentLength = bytes.Length;
               
                
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                requestStream.Close();

                Stream responseStream = ((HttpWebResponse)request.GetResponse()).GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);  //编码可以改成别的
                str = reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
                error = "";
            }
            catch (WebException e)
            {
                //写日志
                error = e.Message;

            }

            return str;

        }



    }
}
