using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace DHCPv6
{
    public class UAIeID
    {
        [DllImport("UAIeID.dll", EntryPoint = "UAI_Initialize")]
        static extern unsafe uint UAI_Initialize(uint dwFlags);
        [DllImport("UAIeID.dll", EntryPoint = "UAI_Finalize")]
        static extern unsafe uint UAI_Finalize(uint dwFlags);
        [DllImport("UAIeID.dll", EntryPoint = "UAI_GetDeviceList")]
        static extern unsafe uint UAI_GetDeviceList(char*** pszDeviceList, uint* pdwDeviceCount, uint dwFlags);
        [DllImport("UAIeID.dll", EntryPoint = "UAI_OpenDevice")]
        static extern unsafe uint UAI_OpenDevice(char* szDeviceName, void** phDev, uint dwFlags);
        [DllImport("UAIeID.dll", EntryPoint = "UAI_CloseDevice")]
        static extern unsafe uint UAI_CloseDevice(void* hDev, uint dwFlags);
        [DllImport("UAIeID.dll", EntryPoint = "UAI_VerifyPIN")]
        static extern unsafe uint UAI_VerifyPIN(void* hDev, char* szPIN, uint* pdwRetryNum, uint dwFlags);
        [DllImport("UAIeID.dll", EntryPoint = "UAI_Hash")]
        static extern unsafe uint UAI_Hash(void* hDev, uint dwDataType, uint dwAlg, byte* pbData, uint dwDataLen, byte* pbHashData, uint* pdwHashDataLen, uint dwFlags);
        [DllImport("UAIeID.dll", EntryPoint = "UAI_PrivateKeyCalc")]
        static extern unsafe uint UAI_PrivateKeyCalc(void* hDev, char* szContainerName, int bIsPlain, byte* pbDataIn, uint dwDataInLen, byte* pbDataOut, uint* pdwDataOutLen, uint dwFlags);

        public static unsafe string Test(string szToHashData, string szPin, out string error)
        {
            uint dwRet = 0;
            string szSignData = "";
            error = "";

            //接口初始化
            dwRet = UAI_Initialize(0);
            if (dwRet != 0)
            {
                //Console.WriteLine("UAI_Initialize error = {0}", dwRet);
                error = "UAI_Initialize error = " + dwRet;
                goto end;
            }

            //获取设备列表
            char** pszDeviceList = null;
            uint dwDeviceCount = 0;
            dwRet = UAI_GetDeviceList(&pszDeviceList, &dwDeviceCount, 0);
            if (dwRet != 0)
            {
                //Console.WriteLine("UAI_GetDeviceList error = {0}", dwRet);
                error = "UAI_GetDeviceList error = " + dwRet;
                goto end;
            }

            //打开设备
            void* hDev = null;
            dwRet = UAI_OpenDevice(pszDeviceList[0], &hDev, 0);
            if (dwRet != 0)
            {
                //Console.WriteLine("UAI_OpenDevice error = {0}", dwRet);
                error = "UAI_OpenDevice error = " + dwRet;
                goto end;
            }

            //PIN码验证
            uint dwRetryNum = 0;
            //string szPin = "136413";
            IntPtr pPin = Marshal.StringToCoTaskMemAnsi(szPin);
            char* chPin = (char*)pPin.ToPointer();
            dwRet = UAI_VerifyPIN(hDev, chPin, &dwRetryNum, 0);
            if (dwRet != 0)
            {
                //Console.WriteLine("UAI_VerifyPIN error = {0}", dwRet);
                error = "UAI_VerifyPIN error = " + dwRet;
                goto end;
            }
            Marshal.FreeCoTaskMem(pPin);

            //杂凑待签名数据
            //string szToHashData = "MTIz";
            byte[] byToHashData = Convert.FromBase64String(szToHashData);
            IntPtr pToHashData = Marshal.AllocCoTaskMem(byToHashData.Length);
            Marshal.Copy(byToHashData, 0, pToHashData, byToHashData.Length);
            byte* pbToHashData = (byte*)pToHashData;
            uint dwToHashDataLen = (uint)byToHashData.Length;
            const int dwHashResultLen = 20;
            IntPtr pHashData = Marshal.AllocCoTaskMem(dwHashResultLen);
            byte* pbHashData = (byte*)pHashData;
            uint dwHashDataLen = dwHashResultLen;
            dwRet = UAI_Hash(hDev, 0x00, 0x01, pbToHashData, dwToHashDataLen, pbHashData, &dwHashDataLen, 0);
            if (dwRet != 0)
            {
                Marshal.FreeCoTaskMem(pToHashData);
                Marshal.FreeCoTaskMem(pHashData);
               // Console.WriteLine("UAI_Hash error = {0}", dwRet);
                error = "UAI_Hash error = " + dwRet;
                goto end;
            }

            //待签名数据总共35字节，15字节OID+20字节签名原文杂凑值
            byte[] byOID_SHA1 = new byte[] { 0x30, 0x21, 0x30, 0x09, 0x06, 0x05, 0x2B, 0x0E, 0x03, 0x02, 0x1A, 0x05, 0x00, 0x04, 0x14 };
            byte[] byToSignData = new byte[byOID_SHA1.Length + dwHashDataLen];
            Array.Copy(byOID_SHA1, byToSignData, byOID_SHA1.Length);
            Marshal.FreeCoTaskMem(pToHashData);
            for (int i = 0; i < dwHashDataLen; i++)
            {
                byToSignData[byOID_SHA1.Length + i] = pbHashData[i];
            }
            Marshal.FreeCoTaskMem(pHashData);
            IntPtr pToSignData = Marshal.AllocCoTaskMem(byToSignData.Length);
            Marshal.Copy(byToSignData, 0, pToSignData, byToSignData.Length);
            byte* pbToSignData = (byte*)pToSignData.ToPointer();
            uint dwpToSignDataLen = (uint)byToSignData.Length;

            //对待签名数据进行RSA私钥运算
            string szContainerName = "{4A7A26B1-ABA5-48ef-8B6A-24A4BA42E787}";
            IntPtr pContainerName = Marshal.StringToCoTaskMemAnsi(szContainerName);
            char* pchszContainerName = (char*)pContainerName.ToPointer();
            const int dwSignResultLen = 128;
            IntPtr pSignData = Marshal.AllocCoTaskMem(dwSignResultLen);
            byte* pbSignData = (byte*)pSignData;
            uint dwSignDataLen = dwSignResultLen;
            dwRet = UAI_PrivateKeyCalc(hDev, pchszContainerName, 1, pbToSignData, dwpToSignDataLen, pbSignData, &dwSignDataLen, 0);
            Marshal.FreeCoTaskMem(pContainerName);
            //Marshal.FreeCoTaskMem(pToHashData);容易出现BUG
            if (dwRet != 0)
            {
                Marshal.FreeCoTaskMem(pSignData);
                //Console.WriteLine("UAI_PrivateKeyCalc error = {0}", dwRet);
                error = "UAI_PrivateKeyCalc error = " + dwRet;
                goto end;
            }
            byte[] bySignData = new byte[dwSignDataLen];
            for (int i = 0; i < dwSignDataLen; i++)
            {
                bySignData[i] = pbSignData[i];
            }
           
            szSignData = Convert.ToBase64String(bySignData);

            Marshal.FreeCoTaskMem(pSignData);

            //关闭设备
            dwRet = UAI_CloseDevice(hDev, 0);
            if (dwRet != 0)
            {
                //Console.WriteLine("UAI_CloseDevice error = {0}", dwRet);
                error = "UAI_PrivateKeyCalc error = " + dwRet;
                goto end;
            }

            //释放设备库
            dwRet = UAI_Finalize(0);
            if (dwRet != 0)
            {
                //Console.WriteLine("UAI_Finalize error = {0}", dwRet);
                error = "UAI_Finalize error = " + dwRet;
                goto end;
            }

         end:

            if (dwRet == 0)
            {
                //Console.WriteLine("all finish");
            }

            //Console.WriteLine("press any key to exit...");
            //Console.Read();

            return szSignData;
        }
    }
}
