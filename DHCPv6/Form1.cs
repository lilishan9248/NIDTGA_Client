using DHCPv6.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using log4net;
using Timer = System.Windows.Forms.Timer;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace DHCPv6
{
    public partial class Form1 : Form
    {
        #region 窗体绘制相关定义

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        // 无窗体可拖拽变量
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        #endregion 窗体绘制相关定义

        private System.Windows.Forms.Timer timer; //断网监听
        private SocketHelper socketHelper;
        private string error = string.Empty;
        private string internetName = string.Empty;
        private ILog _log = null;
        private int interType = 0;
        //liujun 2016-7-31 eID证书信息
        private string issuer = "";
        private string issuer_sn = "";
        private string eid_sn = "";
        private int login_type = 1;


        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 代理方法定义
        /// </summary>
        /// <returns></returns>
        private delegate bool DelegeteLoading();
        private delegate void DelegateLable(bool b);
        private delegate void DelegateDhcp();
        private delegate void DelegateEid();
        private delegate void DelegateSetError(string s);
        private delegate void DelegateSetLabel6(string s);
        private delegate void DelegateCircle(bool b);

        private void Form1_Load(object sender, EventArgs e)
        {
            _log = Log4netFactory.Create(typeof(Form1));
            Init(); //初始化
            socketHelper = new SocketHelper(); //socket 实例化
            InitTime(); //初始化时间
        }

        /// <summary>
        /// 初始化时间
        /// </summary>
        private void InitTime()
        {
            Timer timer = new Timer();
            this.lbTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.lbTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            string user = ConfigHelper.GetSetting("user");
            string pwd = ConfigHelper.GetSetting("pwd");
            string rememberPwd = ConfigHelper.GetSetting("rememberPwd");
            string successMini = ConfigHelper.GetSetting("successMini");
            string autoStart = ConfigHelper.GetSetting("autoStart");
            this.ckbPwd.Checked = bool.Parse(rememberPwd ?? "false");
            this.ckbMini.Checked = bool.Parse(successMini ?? "false");
            this.ckbStart.Checked = bool.Parse(autoStart ?? "false");
            this.txtUser.Text = user;
            if (this.ckbPwd.Checked)
            {
                this.txtPwd.Text = pwd;
            }
            this.panelOn.Visible = true;
            this.panelOff.Visible = false;

        }

        /// <summary>
        /// 连接网络
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.lbError.Visible = false; //隐藏错误lable
                string user = this.txtUser.Text.ToLower().Trim();
                if (!IsUser(user))
                {
                    this.lbError.Visible = true;
                    this.lbError.Text = "用户名错误";
                    return;
                }

                interType = IsInternet();

                _log.Debug("当前网络类型0、无网络可用，1、有线网络，2、无线网络，3、两个网络：" + " " + interType);
                if (interType == 0) //无网络
                {
                    this.lbError.Visible = true;
                    this.lbError.Text = "网络不可用";
                    return;
                }
                List<string> wireLocalIps = GetWireIPv6LocalAddress();
                List<string> wirelessLocalIps = GetWirelessIPv6LocalAddress();
                #region
                if (interType == 1) //有线网络
                {
                    if (wireLocalIps.Count == 0)
                    {
                        this.lbError.Visible = true;
                        this.lbError.Text = "没用可用的IPv6网络";
                        return;
                    }
                    internetName = GetWireName();
                }
                if (interType == 2) //无线网络
                {
                    if (wirelessLocalIps.Count == 0)
                    {
                        this.lbError.Visible = true;
                        this.lbError.Text = "没用可用的IPv6网络";
                        return;
                    }
                    internetName = GetWirelessName();
                }
                if (interType >= 3) //无线有线都可用
                {
                    if (wireLocalIps.Count == 0 && wirelessLocalIps.Count == 0)
                    {
                        this.lbError.Visible = true;
                        this.lbError.Text = "没用可用的IPv6网络";
                        return;
                    }
                    if (wireLocalIps.Count != 0)
                    {
                        internetName = GetWireName();
                        interType = 1;
                    }
                    else
                    {
                        internetName = GetWirelessName();
                        interType = 2;
                    }
                }

                #endregion
                _log.Debug("最终网络：" + internetName);
                this.picBtn.Enabled = false; //
                this.picBtn.Image = (Image)Resources.logUnable;
                ShowLoadingCircle(); //显示加载条

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }

            try
            {
                DelegeteLoading dl = new DelegeteLoading(Networking);
                dl.BeginInvoke(new AsyncCallback(HideLoadingCircle), dl);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }

        }
        /// <summary>
        /// 断网操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                this.picCutbtn.Enabled = false;
                this.picCutbtn.Image = (Image)Resources.cutBtnUnable;
                DelegeteLoading shut = new DelegeteLoading(ShutWork);
                shut.BeginInvoke(new AsyncCallback(HideShut), shut);

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }

        }

        /// <summary>
        /// 异步处理
        /// </summary>
        /// <param name="result"></param>
        private void DhcpAsync(IAsyncResult result)
        {

        }
        /// <summary>
        /// 停止DHCP线程
        /// </summary>
        private void StopDhcp()
        {
            int pid = GetDhcpPid();
            _log.Debug("DHCP Pid:" + pid);
            if (pid == -1)
            {
                return;
            }
            string doscommand = string.Format("taskkill -pid {0} /f & net start Audiosrv & exit", pid);
            ExecDos(doscommand);
        }

        /// <summary>
        /// 开启DHCP服务
        /// </summary>
        private void StartDhcp()
        {
            ExecDos("net start dhcp & net start EventLog & net start lmhosts & net start Wcmsvc & net start wscsvc & net start vmnetdhcp & exit");
        }

        /// <summary>
        /// 获取DHCP的Pid
        /// </summary>
        /// <returns></returns>
        private int GetDhcpPid()
        {
            try
            {
                string result = ExecDosStr("tasklist /svc /fo list & net stop vmnetdhcp &exit");
                string[] arry = result.ToLower().Replace("dhcpv6", "").Replace("vmnetdhcp", "").Split(':');
                for (int i = 0; i < arry.Length; i++)
                {
                    if (arry[i].IndexOf("dhcp", StringComparison.Ordinal) != -1)
                    {
                        return int.Parse(arry[i - 1].Replace("服务", "").Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }

            return -1;
        }

        /// <summary>
        /// 联网
        /// </summary>
        /// <returns></returns>
        private bool Networking()
        {
            #region socket通信

            string userName = this.txtUser.Text.Trim().ToLower();
            string password = this.txtPwd.Text.Trim().ToLower();
            int type = this.login_type;
            StopDhcp();//关闭DHCP线程
            //联网操作
            bool b = socketHelper.Send(userName, password, type, ref error);
            //开启DHCP服务
            DelegateDhcp dhcp = new DelegateDhcp(StartDhcp);
            dhcp.BeginInvoke(new AsyncCallback(DhcpAsync), dhcp);
            if (!b)
            {
                return false;
            }

            #endregion socket通信

            #region 修改IP

            string wireName = GetWireName();
            string wirelessName = GetWirelessName();
            string route = internetName == wireName ? GetWireRoute() : GetWirelessRoute();
            _log.Debug("当前网关：" + route);
            route = string.IsNullOrEmpty(route) ? GetRoute() : route;
            _log.Debug("最终网关：" + route);
            ConfigHelper.UpdateSetting("route", route);
            //删除网关
            List<string> routes = internetName == wireName ? GetWireRouteList() : GetWirelessRouteList();
            routes = routes.Count > 0 ? routes : GetRouteList();
            RemoveRoutes(routes, internetName);
            // 修改IP
            List<string> wireIps = GetWireIPv6Address();
            List<string> wirelessIps = GetWirelessIPv6Address();
            ResetIp(internetName, wireName, wireIps, wirelessName, wirelessIps, route);
            //修改用户信息
            ModifyInfo();

            #endregion 修改IP

            return true;
        }

        /// <summary>
        /// 展示加载条
        /// </summary>
        private void ShowLoadingCircle()
        {
            this.lbError.Visible = false;
            this.loadingCircle1.NumberSpoke = 80;
            this.loadingCircle1.RotationSpeed = 10;
            this.loadingCircle1.Visible = true;
            this.loadingCircle1.Active = true;
        }

        /// <summary>
        /// 隐藏加载条
        /// </summary>
        /// <param name="result"></param>
        private void HideLoadingCircle(IAsyncResult result)
        {
            bool b = (result.AsyncState as DelegeteLoading).EndInvoke(result);
            HideLoadingCircle(b);
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="b"></param>
        private void HideLoadingCircle(bool b)
        {
            if (this.lbError.InvokeRequired)
            {
                this.Invoke(new DelegateLable(HideLoadingCircle), b);
            }
            else
            {
                this.loadingCircle1.Active = false;
                this.loadingCircle1.Visible = false;
                this.picBtn.Image = (Image)Resources.log;
                this.picBtn.Enabled = true;
                if (b)
                {
                    this.panelOn.Visible = false;
                    this.panelOff.Visible = true;
                    this.lbUser.Text = this.txtUser.Text;
                    NetworkListen();

                    if (this.ckbMini.Checked)
                    {
                        this.WindowState = FormWindowState.Minimized;
                    }

                    //liujun 增加eid 认证功能 2016-4-28
                    if (this.rbeID.Checked)
                    {
                        SetCircle(true);
                        if (CheckeIDCard() == -1)
                        {
                            this.lbError.Visible = true;
                            this.lbError.Text = "eID本地认证失败";

                            //失败断网
                            this.picCutbtn.Enabled = false;
                            this.picCutbtn.Image = (Image)Resources.cutBtnUnable;
                            DelegeteLoading shut = new DelegeteLoading(ShutWork);
                            shut.BeginInvoke(new AsyncCallback(HideShut), shut);

                        }
                        else
                        {
                            //liujun 增加eid服务器认证功能， 2016-7-31

                            if (this.ckbMini.Checked)//勾了最小化,因为还需要用窗体因此还得给弄回来
                            {
                                this.WindowState = FormWindowState.Normal;
                            }
                           
;                           DelegateEid DeID = new DelegateEid(EidCheck);
                            DeID.BeginInvoke(new AsyncCallback(HideEid), DeID);
                        }
                    }


                }
                else
                {
                    if (!string.IsNullOrEmpty(ConfigHelper.GetSetting(error)))
                    {
                        this.lbError.Text = ConfigHelper.GetSetting(error);
                    }
                    else
                    {
                        this.lbError.Text = error;
                    }

                    this.lbError.Visible = true;
                }
            }
        }

        /// <summary>
        /// eID实名认证代理方法
        /// </summary>
        /// <returns></returns>
        private void EidCheck()
        {
            eIDForm eid = new eIDForm();
            if (eid.ShowDialog() == DialogResult.OK)
            {
                PkiRNVParams p = new PkiRNVParams();

                p.SetPIN(eid.txtPIN.Text);
                p.SetUserIDNum(eid.txtNo.Text);
                p.SetUserName(eid.txtName.Text);
                p.SetEIDSN(this.eid_sn);
                p.SetEIDUser(this.issuer);
                p.SetEIDUserSN(this.issuer_sn);
                p.init();
                string requestUrl = ConfigHelper.GetSetting("eIDUrl") + EIdConfig.APPID; ;
                string s = p.ToJson();
                _log.Info("发布报文:" + s);

                string error = "";
                string result = WebUtil.doPost(requestUrl, s, out error);
                if (error != "")
                    _log.Error(error);
                _log.Info("接送报文" + result);
                try
                {
                    ResponseData data = JsonHelper.DeserializeJsonToObject<ResponseData>(result);
                    if (data.result_detail != "0000000") //认证失败
                    {
                        string info = "";
                        switch (data.result)
                        {
                            case "0101001":
                                info = "version为空";
                                break;
                            case "0302000":
                                info = "姓名身份证不匹配";
                                break;
                            case "0302001":
                                info = "eID证书已过期";
                                break;
                            case "0302002":
                                info = "eID签名验签失败";
                                break;
                            case "0302003":
                                info = "eID HMAC验签失败";
                                break;
                            default:
                                info = "其它错误：编号" + data.result_detail;
                                break;

                        }

                        //this.lbError.Visible = true;
                        //this.lbError.Text = "eID远程实名认证失败,失败原因：" + info;
                        SetError("eID远程实名认证失败,失败原因：" + info);
                        //失败断网
                        this.picCutbtn.Enabled = false;
                        this.picCutbtn.Image = (Image)Resources.cutBtnUnable;
                        DelegeteLoading shut = new DelegeteLoading(ShutWork);
                        shut.BeginInvoke(new AsyncCallback(HideShut), shut);
                    }
                    else //认证成功
                    {
                        if (this.ckbMini.Checked)//勾了最小化
                        {
                            this.WindowState = FormWindowState.Minimized;
                        }
                        //2017-6-27  增加提示效果,隐藏
                        SetLabel6("欢迎使用教育网IPv6网络");
                        SetCircle(false);
                    }
                }
                catch (Exception ex)
                {
                    //2017-6-27  增加提示效果,隐藏
                    SetLabel6("欢迎使用教育网IPv6网络");
                    _log.Error(ex.Message);
                    SetCircle(false);
                }

            }
            else
            {
                SetError("eID远程实名认证取消");
                //失败断网

                DelegeteLoading shut = new DelegeteLoading(ShutWork);
                shut.BeginInvoke(new AsyncCallback(HideShut), shut);

            }
        }

        private void HideEid(IAsyncResult result)
        {
            //bool b = (result.AsyncState as DelegateEid).EndInvoke(result);

        }

        /// <summary>
        /// 异步操作错误信息
        /// </summary>
        /// <param name="s"></param>
        private void SetError(string s)
        {
            if (this.lbError.InvokeRequired)
            {
                this.Invoke(new DelegateSetError(SetError), s);
            }
            else
            {
                this.lbError.Visible = true;
                this.lbError.Text = s;
                this.picCutbtn.Enabled = false;
                this.picCutbtn.Image = (Image)Resources.cutBtnUnable;
            }
        }


        /// <summary>
        /// 异步操作错误信息
        /// </summary>
        /// <param name="s"></param>
        private void SetLabel6(string s)
        {
            if (this.label6.InvokeRequired)
            {
                this.Invoke(new DelegateSetLabel6(SetLabel6), s);
            }
            else
            {
                this.label6.Text = s;
            }
        }

        /// <summary>
        /// 异步操作错误信息
        /// </summary>
        /// <param name="s"></param>
        private void SetCircle(bool b)
        {
            if (this.loadingCircle2.InvokeRequired)
            {
                this.Invoke(new DelegateCircle(SetCircle), b);
            }
            else
            {
                if (b)//显示
                {
                    this.loadingCircle2.NumberSpoke = 80;
                    this.loadingCircle2.RotationSpeed = 10;
                    this.loadingCircle2.Visible = true;
                    this.loadingCircle2.Active = true;
                    this.picCutbtn.Image = (Image)Resources.cutBtnUnable;
                    this.picCutbtn.Enabled = false;
                }
                else
                {
                    this.loadingCircle2.Active = false;
                    this.loadingCircle2.Visible = false;
                    this.picCutbtn.Image = (Image)Resources.cutBtn;
                    this.picCutbtn.Enabled = true;
                }
            }
        }


        /// <summary>
        /// 断网
        /// </summary>
        /// <returns></returns>
        private bool ShutWork()
        {
            string userName = this.txtUser.Text.Trim().ToLower();
            string password = this.txtPwd.Text.Trim().ToLower();
            int type = this.login_type;
            StopDhcp(); //关闭DHCP线程
            bool result = socketHelper.SendRelease(userName, password, type, ref error);
            //开启DHCP服务
            DelegateDhcp dhcp = new DelegateDhcp(StartDhcp);
            dhcp.BeginInvoke(new AsyncCallback(DhcpAsync), dhcp);
            //StartDhcp(); 
            return result;
        }

        private void HideShut(IAsyncResult result)
        {
            bool b = (result.AsyncState as DelegeteLoading).EndInvoke(result);
            HideShut(b);
        }

        private void HideShut(bool b)
        {
            if (this.lbError.InvokeRequired)
            {
                this.Invoke(new DelegateLable(HideShut), b);
            }
            else
            {
                if (b)
                {
                    RemoveIPs();
                    this.panelOn.Visible = true;
                    this.panelOff.Visible = false;
                    this.picCutbtn.Enabled = true;
                    this.picCutbtn.Image = (Image)Resources.cutBtn;
                    this.timer.Stop();
                }
                else
                {
                    this.picCutbtn.Enabled = true;
                    this.picCutbtn.Image = (Image)Resources.cutBtn;
                }
            }

        }
        /// <summary>
        /// 断网监听
        /// </summary>
        private void NetworkListen()
        {
            if (timer == null)
            {
                timer = new System.Windows.Forms.Timer();
            }
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (IsInternet(interType) == 0)
            {
                timer.Stop();
                int seconds = int.Parse(ConfigHelper.GetSetting("timespan"));
                for (int i = 0; i < seconds; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                    if (IsInternet(interType) == 0)
                    {
                        continue;
                    }
                    break;
                }
                if (IsInternet(interType) == 0)
                {
                    timer.Dispose();
                    RemoveIPs();
                    if (MessageBox.Show("网络不可用", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        this.Close();
                    }
                }
                else
                {
                    timer.Start();
                }

            }
        }

        /// <summary>
        /// 设置IP
        /// </summary>
        /// <param name="internetName">有效网络名称</param>
        /// <param name="wireName">有线网络名称</param>
        /// <param name="wireIps">有线IP地址组</param>
        /// <param name="wirelessName">无线网络名称</param>
        /// <param name="wirelessIps">无线IP地址组</param>
        private void ResetIp(string internetName, string wireName, List<string> wireIps, string wirelessName,
            List<string> wirelessIps, string route)
        {
            string ipv6 = CommonHelper.StrToIpv6(CommonHelper.ByteToString(socketHelper.ipv6));
            _log.Info("获取的IP：" + ipv6);
            string dns = CommonHelper.StrToIpv6(CommonHelper.ByteToString(socketHelper.dns));
            _log.Info("获取的DNS：" + dns);
            #region 禁止临时IPv6地址的生成

            string command = "netsh interface ipv6 set privacy state=disable";

            #endregion 禁止临时IPv6地址的生成

            #region 禁用自动配置

            command += string.Format("&netsh interface ipv6 set interface {0} routerdiscovery=disable", internetName);

            #endregion 禁用自动配置

            #region 兼容win8,win8.1,win10平台

            command += string.Format("&netsh interface ipv6 add address {0} {1}", internetName, "2001:da8:266:f200:bd3a:9ca1:9c6b:ce72");
            command += string.Format("&netsh interface ipv6 delete address {0} {1}", internetName, "2001:da8:266:f200:bd3a:9ca1:9c6b:ce72");

            #endregion 兼容win8,win8.1,win10平台

            //删除网卡IP
            foreach (string item in wireIps)
            {
                command += string.Format("&netsh interface ipv6 delete address {0} {1}", wireName, item);
            }
            //删除无线网卡IP
            foreach (string item in wirelessIps)
            {
                command += string.Format("&netsh interface ipv6 delete address {0} {1}", wirelessName, item);
            }

            command += string.Format("&netsh interface ipv6 set address {0} {1}/64 store=active", internetName, ipv6);//设置IPv6地址   store=active
            command += string.Format("&netsh interface ipv6 set dnsservers {0} static {1} primary no", internetName, dns);//设置DNS
            command += string.Format("&netsh interface ipv6 add route ::/0 {0} {1}", internetName, route);//设置网关
            command += "&exit";
            ExecDos(command); //执行dos命令
        }

        /// <summary>
        /// 删除网关
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="internetName"></param>
        private void RemoveRoutes(List<string> routes, string internetName)
        {
            if (routes.Count == 0) return;
            string command = string.Format("ipconfig");
            foreach (var item in routes)
            {
                command += string.Format("&netsh interface ipv6 delete route ::/0 {0} {1}", internetName, item);
            }
            command += "&exit";
            ExecDos(command); //执行dos命令
        }

        /// <summary>
        /// 执行DOS命令
        /// </summary>
        /// <param name="command"></param>
        private void ExecDos(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(command);
            p.StandardInput.AutoFlush = true;
            string output = p.StandardOutput.ReadToEnd();
            _log.Debug("DOS命令：" + command);
            _log.Debug("DOS执行结果：" + output);
            p.WaitForExit();
            p.Close();
        }
        private string ExecDosStr(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(command);
            p.StandardInput.AutoFlush = true;
            string output = p.StandardOutput.ReadToEnd();
            _log.Debug("DOS命令：" + command);
            _log.Debug("DOS执行结果：" + output);
            p.WaitForExit();
            p.Close();
            return output;
        }

        /// <summary>
        /// 执行eID客户端 liujun 2016-4-25 增加
        /// </summary>
        /// <param name="command">命令类型1为只检查设备;2为检查设备输入密码</param>
        /// <returns></returns>
        private string ExecEID(string command)
        {
            Process myProcess = new Process();
            //VC++生成的exe
            string fileName = AppDomain.CurrentDomain.BaseDirectory + @"/eIDDemo.exe";
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = fileName;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = command;//参数以空格分隔，如果某个参数为空，可以传入””
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            p.Close();
            return output;
        }

        /// <summary>
        /// 检查eID设备 liujun 2016-4-25 增加
        /// </summary>
        /// <returns></returns>
        private int CheckeIDDevice()
        {
            try
            {
                string result = ExecEID("");
                //检查硬件反馈
                if (result.IndexOf("错误码") > 0)
                    return -1;
                else if (result.IndexOf("签名成功") > 0)
                    return 1;
                else
                    return -2;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 检查eID卡片 liujun 2016-4-25 增加
        /// </summary>
        /// <returns></returns>
        private int CheckeIDCard()
        {

            //2017-6-27  增加提示效果,隐藏
            this.label6.Text = "eID身份证认证中.......";
            try
            {
                string result = ExecEID("r");
                //检查硬件反馈
                if (result.IndexOf("错误码") > 0)
                    return -1;
                else if (result.IndexOf("签名成功") > 0)
                {
                    //获取银行卡内部消息
                    result = result.Replace("\r\n", ",");
                    string[] fields = result.Split(',');
                    this.issuer = FindeIDInfo(fields, "证书发行方：");
                    this.issuer_sn = FindeIDInfo(fields, "证书设备序列号：");
                    this.eid_sn = FindeIDInfo(fields, "证书序列号：");
                    return 1;
                }

                else
                    return -2;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return -1;
            }

        }

        private string FindeIDInfo(string[] fileds, string key)
        {
            string value = "";
            for (int i = 0; i < fileds.Length; i++)
            {
                if (fileds[i] == key)
                    value = fileds[i + 1];
            }
            return value;
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        private void ModifyInfo()
        {
            ConfigHelper.UpdateSetting("user", txtUser.Text.Trim());
            if (this.ckbPwd.Checked)
            {
                ConfigHelper.UpdateSetting("pwd", txtPwd.Text.Trim());
                ConfigHelper.UpdateSetting("rememberPwd", "true");
            }
        }

        /// <summary>
        /// 记住密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbPwd_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 登陆最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbMini_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbMini.Checked)
            {
                ConfigHelper.UpdateSetting("successMini", "true");
            }
            else
            {
                ConfigHelper.UpdateSetting("successMini", "false");
            }
        }

        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbStart_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ckbStart.Checked)
                {
                    ConfigHelper.UpdateSetting("autoStart", "true");
                }
                else
                {
                    ConfigHelper.UpdateSetting("autoStart", "false");
                }

                string strName = Application.ExecutablePath;
                string strnewName = strName.Substring(strName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                if (ckbStart.Checked)
                {
                    if (!File.Exists(strName)) //指定文件是否存在
                        return;
                    Microsoft.Win32.RegistryKey Rkey =
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    if (Rkey == null)
                        Rkey =
                            Microsoft.Win32.Registry.LocalMachine.CreateSubKey(
                                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                    Rkey.SetValue(strnewName, strName); //修改注册表，使程序开机时自动执行。
                }
                else
                {
                    //修改注册表，使程序开机时不自动执行。
                    Microsoft.Win32.RegistryKey Rkey =
                        Microsoft.Win32.Registry.LocalMachine.CreateSubKey(
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                    Rkey.DeleteValue(strnewName, false);
                }
            }
            catch
            {
                return;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (!this.ckbMini.Checked)
            {
                return;
            }
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false; //不显示在系统任务栏
                this.notifyIcon1.Visible = true; //托盘图标可见
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true; //显示在系统任务栏
            this.WindowState = FormWindowState.Normal; //还原窗体
        }

        /// <summary>
        /// 获取联网网卡信息
        /// </summary>
        /// <returns></returns>
        private string GetInternetName()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in nics)
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    return item.Name;
                }
            }
            return ""; //没有网卡可用
        }

        /// <summary>
        /// 获取有线网卡信息
        /// </summary>
        /// <returns></returns>
        private string GetWireName()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in nics)
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    item.OperationalStatus == OperationalStatus.Up)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    return item.Name;
                }
            }
            return ""; //没有网卡可用
        }

        // 获取无线线网卡信息
        private string GetWirelessName()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in nics)
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                    item.OperationalStatus == OperationalStatus.Up)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    return item.Name;
                }
            }
            return ""; //没有网卡可用
        }

        /// <summary>
        /// 获取有线IPv6地址
        /// </summary>
        /// <returns></returns>
        private List<string> GetWireIPv6Address()
        {
            List<string> ips = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.UnicastAddresses.Count > 0)
                    {
                        foreach (var item in ip.UnicastAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6 &&
                                !item.Address.IsIPv6LinkLocal)
                            {
                                ips.Add(item.Address.ToString());
                            }
                        }
                    }
                }
            }
            return ips;
        }

        /// <summary>
        /// 获取有线IPv6本地链路地址
        /// </summary>
        /// <returns></returns>
        private List<string> GetWireIPv6LocalAddress()
        {
            List<string> ips = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.UnicastAddresses.Count > 0)
                    {
                        foreach (var item in ip.UnicastAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6 &&
                                item.Address.IsIPv6LinkLocal)
                            {
                                ips.Add(item.Address.ToString());
                            }
                        }
                    }
                }
            }
            return ips;
        }

        /// <summary>
        /// 获取无线IPv6地址
        /// </summary>
        /// <returns></returns>
        private List<string> GetWirelessIPv6Address()
        {
            List<string> ips = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.UnicastAddresses.Count > 0)
                    {
                        foreach (var item in ip.UnicastAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6 &&
                                !item.Address.IsIPv6LinkLocal)
                            {
                                ips.Add(item.Address.ToString());
                            }
                        }
                    }
                }
            }
            return ips;
        }

        // 获取无线IPv6本地地址
        private List<string> GetWirelessIPv6LocalAddress()
        {
            List<string> ips = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.UnicastAddresses.Count > 0)
                    {
                        foreach (var item in ip.UnicastAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6 &&
                                item.Address.IsIPv6LinkLocal)
                            {
                                ips.Add(item.Address.ToString());
                            }
                        }
                    }
                }
            }
            return ips;
        }

        /// <summary>
        /// 判定是否有可用网络
        /// </summary>
        /// <returns>0、无网络可用，1、有线网络，2、无线网络，3、两个网络</returns>
        private int IsInternet(int type = 0)
        {
            try
            {
                int count = 0;
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface item in nics)
                {
                    string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + item.Id + "\\Connection";
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                    if (rk != null)
                    {
                        // 区分 PnpInstanceID  
                        // 如果前面有 PCI 就是本机的真实网卡 
                        string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                        if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                        {
                            if (type == 0 || type == 1)
                            {
                                if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                                item.OperationalStatus == OperationalStatus.Up)
                                {
                                    count += 1;
                                }
                            }
                        }
                    }
                    //无线
                    if (type == 0 || type == 2)
                    {
                        if (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                        item.OperationalStatus == OperationalStatus.Up)
                        {
                            count += 2;
                        }
                    }
                }
                return count; //没有网卡可用
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 获取有线网关
        /// </summary>
        /// <returns></returns>
        private string GetWireRoute()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.GatewayAddresses.Count > 0)
                    {
                        foreach (var item in ip.GatewayAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                if (item.Address.ToString().IndexOf('%') == -1)
                                {
                                    return item.Address.ToString();
                                }
                                return item.Address.ToString().Substring(0, item.Address.ToString().IndexOf('%'));
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取有线网关
        /// </summary>
        /// <returns></returns>
        private List<string> GetWireRouteList()
        {
            var wireRoutes = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.GatewayAddresses.Count > 0)
                    {
                        foreach (var item in ip.GatewayAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                if (item.Address.ToString().IndexOf('%') == -1)
                                {
                                    wireRoutes.Add(item.Address.ToString());
                                }
                                else
                                {
                                    wireRoutes.Add(item.Address.ToString().Substring(0, item.Address.ToString().IndexOf('%')));

                                }
                            }
                        }
                    }
                }
            }
            return wireRoutes;
        }

        /// <summary>
        /// 获取无线网关
        /// </summary>
        /// <returns></returns>
        private string GetWirelessRoute()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.GatewayAddresses.Count > 0)
                    {
                        foreach (var item in ip.GatewayAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                if (item.Address.ToString().IndexOf('%') == -1)
                                {
                                    return item.Address.ToString();
                                }
                                else
                                {
                                    return item.Address.ToString().Substring(0, item.Address.ToString().IndexOf('%'));
                                }

                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取无线网关
        /// </summary>
        /// <returns></returns>
        private List<string> GetWirelessRouteList()
        {
            var wirelessRoutes = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    if (ip.GatewayAddresses.Count > 0)
                    {
                        foreach (var item in ip.GatewayAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                if (item.Address.ToString().IndexOf('%') == -1)
                                {
                                    wirelessRoutes.Add(item.Address.ToString());

                                }
                                else
                                {
                                    wirelessRoutes.Add(item.Address.ToString().Substring(0, item.Address.ToString().IndexOf('%')));

                                }
                            }
                        }
                    }
                }
            }
            return wirelessRoutes;
        }

        /// <summary>
        /// 以上两种情况不能获取网关时调用
        /// </summary>
        /// <returns></returns>
        private string GetRoute()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection nics = mc.GetInstances();
            foreach (ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true && nic["DefaultIPGateway"] != null)
                {
                    foreach (var item in nic["DefaultIPGateway"] as String[])
                    {
                        if (item.IndexOf(':') >= 0) return item;
                    }
                }
            }
            return ConfigHelper.GetSetting("route");
        }

        /// <summary>
        /// 以上两种情况不能获取网关时调用
        /// </summary>
        /// <returns></returns>
        private List<string> GetRouteList()
        {
            var routes = new List<string>();
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection nics = mc.GetInstances();
            foreach (ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                {
                    if (nic["DefaultIPGateway"] == null) return routes;
                    foreach (var item in nic["DefaultIPGateway"] as String[])
                    {
                        if (item.IndexOf(':') >= 0) routes.Add(item);
                    }
                }
            }
            return routes;
        }

        /// <summary>
        /// 用户名是否有效
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool IsUser(string user)
        {

            switch (login_type)
            {
                case 1:
                    if (user.Length != 10)
                    {
                        return false;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        string s = user.Substring(i, 1);
                        string ss = "0123456789abcdef";
                        int t = ss.IndexOf(s, StringComparison.Ordinal);
                        if (t == -1)
                        {
                            return false;
                        }
                    }
                    break;
                case 2:
                    if (user.Length > 20)
                    {
                        return false;
                    }
                    break;
                case 3:
                    if (user.Length != 11)
                    {
                        return false;
                    }
                    if (!Regex.IsMatch(user, @"^[0-9]\d*$"))
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// 断网移除IP地址
        /// </summary>
        private void RemoveIPs()
        {
            var wireIps = GetWireIPv6Address();
            var wirelessIps = GetWirelessIPv6Address();
            string wireName = GetWireName();
            string wirelessName = GetWirelessName();
            //删除网关
            List<string> routes = internetName == wireName ? GetWireRouteList() : GetWirelessRouteList();
            routes = routes.Count > 0 ? routes : GetRouteList();
            RemoveRoutes(routes, internetName);
            //dos
            string command = "ipconfig";
            //删除网卡IP
            foreach (string item in wireIps)
            {
                command += string.Format("&netsh interface ipv6 delete address {0} {1}", wireName, item);
            }
            //删除无线网卡IP
            foreach (string item in wirelessIps)
            {
                command += string.Format("&netsh interface ipv6 delete address {0} {1}", wirelessName, item);
            }

            //启动临时IPv6
            command += string.Format("&netsh interface ipv6 set privacy state=enable");
            //自动获取DNS
            command += string.Format("&netsh interface ipv6 set dns name={0} source=dhcp", internetName);
            //开启自动配置
            command += string.Format("&netsh interface ipv6 set interface {0} routerdiscovery=enable", internetName);

            command += "&exit";
            ExecDos(command); //执行dos命令
        }

        #region 窗体设置圆角

        public void SetWindowRegion()
        {
            System.Drawing.Drawing2D.GraphicsPath FormPath;
            FormPath = new System.Drawing.Drawing2D.GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            FormPath = FrmStyleControl.GetRoundedRectPath(rect, 20);
            this.Region = new Region(FormPath);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SetWindowRegion();
        }

        #endregion 窗体设置圆角

        private void picMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void lbError_Click(object sender, EventArgs e)
        {
            return;
        }

        private void rbeID_CheckedChanged(object sender, EventArgs e)
        {
            if (rbeID.Checked)
            {
                picBtn.Enabled = false;
                if (CheckeIDDevice() == -1)
                //if(1==-1)
                {
                    MessageBox.Show("读取设备失败");
                    rbeID.Checked = false;
                    rbNID.Checked = true;
                    picBtn.Enabled = true;
                }
                else
                {
                    MessageBox.Show("设备识别成功");
                    picBtn.Enabled = true;
                }

            }

        }

        private void rbInNID_CheckedChanged(object sender, EventArgs e)
        {
            if (rbInNID.Checked)
            {
                login_type = 1;
            }
        }

        private void rbInSID_CheckedChanged(object sender, EventArgs e)
        {
            if (rbInSID.Checked)
            {
                login_type = 2;
            }
        }

        private void rbInMID_CheckedChanged(object sender, EventArgs e)
        {
            if (rbInMID.Checked)
            {
                login_type = 3;
            }
        }
    }
}