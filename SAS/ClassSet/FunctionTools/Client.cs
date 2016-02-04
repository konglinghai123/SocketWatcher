using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.IO;
using SAS.ClassSet.MemberInfo;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace SAS.ClassSet.FunctionTools
{
    class Client
    {

        private readonly ManualResetEvent TimeoutObject = new ManualResetEvent(false); //连接超时对象 
        private readonly Dictionary<string, ManualResetEvent> DictTimeoutObject = new Dictionary<string, ManualResetEvent>();//连接超时对象集合
        private readonly Dictionary<string, ManualResetEvent> DictSendoutObject = new Dictionary<string, ManualResetEvent>();//发送超时集合 
        private readonly Dictionary<string, ManualResetEvent> DictRecoutObject = new Dictionary<string, ManualResetEvent>();//接收超时集合 
        private readonly ManualResetEvent SendTimeout = new ManualResetEvent(false); //发送超时对象 
        private readonly ManualResetEvent RecTimeout = new ManualResetEvent(false); //接受超时对象 
        private Dictionary<string, bool> IsApplyRetry = new Dictionary<string, bool>();
        public static Dictionary<string, SocketInfo> dict = new Dictionary<string, SocketInfo>();//当前在线的服务器字典<服务器Ip,对象的socket对象>
        private Dictionary<string,Thread> ThreadPool = new Dictionary<string, Thread>();//线程池
        private UIShow Show = new UIShow();//事务处理对象
        int timeoutSent = 5000;//发送超时参数
        int timeoutRec = 3000;//接收超时参数
        int timeoutCon = 10000;//连接超时参数
        private HandleCommand handle = new HandleCommand();//字符处理对象
        private const string OnLine = "在线";
        private const string DisConnection = "断线";
        private string recflag = "";//发送消息等待一段时间后接收的信息
        private Insert2DataBase Database = new Insert2DataBase();//写入数据库
        private bool IsWifi = false;
        byte[] buffer = new byte[1024];
        private Queue<SocketInfo> Qmessage = new Queue<SocketInfo>();
        /// <summary>
        /// 连接服务器方法，循环创建线程用于连接每台服务器
        /// </summary>
        /// <param name="ObjIp">传入的Ip对象集合</param>
        public void connect(object ObjIp)
        {
            List<ClientInfo> IpAndport = (List<ClientInfo>)ObjIp;

            for (int i = 0; i < IpAndport.Count; i++)
            {
                IsApplyRetry.Add(IpAndport[i].Ip,false);
                ManualResetEvent ConnectTimeout = new ManualResetEvent(false);
                DictTimeoutObject.Add(IpAndport[i].Ip, TimeoutObject);
                ManualResetEvent SendTimeout = new ManualResetEvent(false);
                DictSendoutObject.Add(IpAndport[i].Ip, SendTimeout);
                ManualResetEvent RecTimeout = new ManualResetEvent(false);
                DictRecoutObject.Add(IpAndport[i].Ip, RecTimeout);

                ParameterizedThreadStart pts = new ParameterizedThreadStart(AloneConnect);
                Thread thradRecMsg = new Thread(pts);
                thradRecMsg.IsBackground = true;
                thradRecMsg.Start(IpAndport[i]);
            }
            ParameterizedThreadStart p = new ParameterizedThreadStart(Testonline);
            Thread testonline = new Thread(p);
            testonline.IsBackground = true;
            testonline.Start(IpAndport);

            ParameterizedThreadStart handlemessage = new ParameterizedThreadStart(HandleMessage);
            Thread handle = new Thread(handlemessage);
            handle.IsBackground = true;
            handle.Start();
        }
        private int Findipname(ListView listView1, string ip)
        {

            try
            {
                for (int x = 0; x < listView1.Items.Count; x++)
                {
                    if (ip == listView1.Items[x].SubItems[1].Text)
                    {
                        // MessageBox.Show("ID"+x);

                        return x;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("ID[" + i + "]名称-" + mc + "-[" + ex + "]"); 
                return -1;
            }

        }
        //---------向显示屏发送 （在线/断线）---------
        public void FaiSong_Led(int Id, string txt1, int ye)
        {
            string command;
            string mc = txt1;
            if (Id > 0)
            {
                int id = Id - 1;
                if (mc.Length > 4)
                {
                    mc = mc.Substring(0, mc.Length - 4);
                }
                command = "+ZHA-" + ye + "ID-" + id + "-MC-" + mc + "\r\n";
                byte[] bytearray = Encoding.GetEncoding("GBK").GetBytes(command.Trim());//转码为Byte数组（GBK）
                                                                                        //-------------------------------------------------------------------------
                                                                                        // MessageBox.Show("ID[" + i + "]名称-" + mc + "-[" + command+"]");

                if (frmMain.FormList.Items[0].SubItems[2].Text == "在线")
                {
                    string Ip = frmMain.FormList.Items[0].SubItems[1].Text;
                    //sdt.BHUF = "";
                    send(Ip, bytearray);



                }
            }
        }
        //-------------------------------------------
        /// <summary>
        /// 连接一个服务器的方法
        /// </summary>
        /// <param name="objinfo">服务器信息对象</param>
        private void AloneConnect(object objinfo)
        {
            ClientInfo info = (ClientInfo)objinfo;
           
            SocketInfo socketinfo = new SocketInfo();
            if (PingTest(info.Ip))
            {

                DictTimeoutObject[info.Ip].Reset();
                Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(info.Ip), int.Parse(info.Port));
                socketinfo.Ip = info.Ip;
                socketinfo.Port = info.Port;
                socketinfo.Socket = socketClient;
                socketClient.BeginConnect(remoteEndPoint, ConnectCallBackMethod, socketinfo);

              







            }
            else
            {

            }


        }
        /// <summary>
        /// 连接时的回调方法
        /// </summary>
        /// <param name="asyncresult"></param>
        private void ConnectCallBackMethod(IAsyncResult asyncresult)
        {     //使阻塞的线程继续
            SocketInfo socket = asyncresult.AsyncState as SocketInfo;
            try
            {


                if (socket != null)
                {
                    socket.Socket.EndConnect(asyncresult);
                    socket.Isconnect = true;
                    ShowIpStauts(socket.Ip, OnLine);
                    dict.Add(socket.Ip, socket);
                    MsgToServer(socket.Socket);
                    //开通讯线程
                    ApplyThread(OnLine, socket);
                  
                }
                else
                {
                   
                }
            }
            catch (Exception)
            {

            }
            finally
            {

                DictTimeoutObject[socket.Ip].Set();
            }



        }
        /// <summary>
        /// 发送查询命令
        /// </summary>
        /// <param name="sender"></param>
        public void MsgToServer(Socket sender)
        {

            string strMsg = "+查询主机\r\n";//指令
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(strMsg.Trim());//转码为Byte数组（GBK）
            sender.Send(data);



        }
        /// <summary>
        /// 申请线程
        /// </summary>
        /// <param name="flag">申请线程类型</param>
        /// <param name="obj">委托对象</param>
        private void ApplyThread(string flag, object obj)
        {
            ParameterizedThreadStart thradstart;
            Thread thrad;
          
            switch (flag)
            {
                case OnLine:
                    thradstart = new ParameterizedThreadStart(RecMsg);
                    thrad = new Thread(thradstart);
                    thrad.IsBackground = true;
                    thrad.Start((SocketInfo)obj);
                    SocketInfo info = (SocketInfo)obj;
                    ThreadPool.Add(info.Ip, thrad);

                    break;
                case DisConnection:
                    thradstart = new ParameterizedThreadStart(Retry);
                    thrad = new Thread(thradstart);
                    thrad.IsBackground = true;
                    thrad.Start((ClientInfo)obj);
                    ClientInfo clientinfo = (ClientInfo)obj;
                    ThreadPool.Add(clientinfo.Ip, thrad);
                    break;

            }
        }
        /// <summary>
        /// 关闭所有线程
        /// </summary>
        public void CloseThread()
        {


            foreach (Thread t in ThreadPool.Values)
            {
                t.Abort();


            }
            foreach (SocketInfo s in dict.Values)
            {
                s.Socket.Close();
            }
            //dict.Clear();
            //ThreadPool.Clear();
        }


        //--异步发送回调方法         
        private void SendCallBackMethod(IAsyncResult asyncresult)
        {
            //使阻塞的线程继续  
            SocketInfo socket = asyncresult.AsyncState as SocketInfo;
            DictTimeoutObject[socket.Ip].Set();
        }
    
      
        /// <summary>
        /// 服务器无法连接时，不断尝试连接服务器
        /// </summary>
        /// <param name="ip">无法连接的Ip</param>
        /// <param name="Port">无法连接的无法端口</param>
        private void Retry(object Info)
        {
            ClientInfo info = (ClientInfo)Info;
            SocketInfo socketinfo = new SocketInfo();
            while (true)
            {

                try
                {
                    DictTimeoutObject[info.Ip].Reset();
                    Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(info.Ip), int.Parse(info.Port));
                    socketinfo.Ip = info.Ip;
                    socketinfo.Port = info.Port;
                    socketinfo.Socket = socketClient;
                    socketClient.BeginConnect(remoteEndPoint, ConnectCallBackMethod, socketinfo);

                    //阻塞当前线程

                    if (DictTimeoutObject[info.Ip].WaitOne(timeoutCon, false))
                    {
                        if (dict[info.Ip].Isconnect)
                        {
                            IsApplyRetry[info.Ip] = false;
                            int len = Findipname(frmMain.FormList, info.Ip);
                            FaiSong_Led(len, frmMain.FormList.Items[len].SubItems[3].Text, 1);
                            break;
                        }
                        else
                        {
                            continue;
                        }
                       
                    }

                    else
                    {

                        continue;
                    }




                }
                catch (System.Exception)
                {
                    continue;
                }
            }
        }
       
      
        /// <summary>
        /// 向服务器发送数据
        /// </summary>
        /// <param name="Ip">服务器Ip</param>
        /// <param name="arrMsg">发送的信息</param>
        public void send(string Ip, byte[] arrMsg)
        {
            try
            {
                if (Ip == "")
                {
                    foreach (SocketInfo s in dict.Values)
                    {
                        s.Socket.Send(arrMsg);

                    }
                }
                else
                {
                    dict[Ip].Socket.Send(arrMsg);
                }

            }
            catch (System.Exception)
            {

            }

        }

       
        /// <summary>
        /// 用于在主界面显示Ip及其在线状态
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="stauts"></param>
        private void ShowIpStauts(string ip, string stauts)
        {
            string[] str = new string[]{
                       stauts ,
                       ip
                    };
            Show.ShwMsgforView(frmMain.FormList, str);

        }
        /// <summary>
        /// 用于判别返回结果
        /// </summary>
        /// <param name="command">返回结果的字符串</param>
        private void ContorlCommand(object obj)
        {
           
            SocketInfo socketinfo=(SocketInfo)obj;
            string command = socketinfo.Command;
            string ip = socketinfo.Ip;
            Show.SetIpAndRec(command, ip);
            if (command.IndexOf("SHOW-2") != -1)
            {

               
                Show.ShwStuatsforView(frmMain.FormList, handle.QueryHandle(command), ip);
                //MessageInfo info = new MessageInfo(command, ip, "查询返回", DateTime.Now.ToString());
                //Database.insert(info);
                // Database.insert(info);//写入数据库

            }
            else if (command.IndexOf("+主机设防") != -1)
            {
               
                MessageInfo info = new MessageInfo(command, ip, "设防返回", DateTime.Now.ToString());
                Database.insert(info);
            }
            else if (command.IndexOf("SHOW-6") != -1)
            {

                Show.ShwStuatsforView(frmMain.FormList, handle.QueryHandle(command), ip);

            }
            //--------------------------------------------------------------
            else if (command.IndexOf("SHOW-9[") != -1)
            {
                DateTime dt = DateTime.Now;
                command = "+" + command;
                string[] str = command.Split('[', ']');
                MessageInfo info = new MessageInfo(frmMain.Decode(str[1]), ip, "警报", DateTime.Now.ToString());//将转发记录转码后保存
                Database.insert(info);

            }
            //--------------------------------------------------------------------------------
            else if (command.IndexOf("SHOW-8[") != -1)
            {
               
                DateTime dt = DateTime.Now;
                command = "+" + command;
                string[] str = command.Split('[', ']');
                MessageInfo info = new MessageInfo(frmMain.Decode(str[1]), ip, "警报", DateTime.Now.ToString());//将转发记录转码后保存
                Database.insert(info);
                //-------------------------------------------------------------向主机发数据--------------------------
                byte[] bytearray = Encoding.Unicode.GetBytes(command);
                foreach (string host in frmMain.HostIp)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string GJ = "-OK";

                        if (SendAcy(host, bytearray, GJ))
                        {
                            //MessageBox.Show("转发成功");
                            break;
                        }
                        else
                        {
                            Thread.Sleep(2000);
                            continue;
                        }

                    }
                }


            }
            else if (command.IndexOf("SWT-OK") != -1)
            {
                recflag = command;
                   DictRecoutObject[socketinfo.Ip].Set();

            }
            else if (command.IndexOf("REC-OK") != -1)
            {

                recflag = command;
                DictRecoutObject[socketinfo.Ip].Set();

            }
            else if (command.IndexOf("LedGengXin") != -1)
            {
                recflag = command;
                DictRecoutObject[socketinfo.Ip].Set();
                frmMain.fm.GengXin_OK();
                Thread.Sleep(1000);

            }
            else if (command.IndexOf("CHK-OK") != -1)
            {
                recflag = command;
                DictRecoutObject[socketinfo.Ip].Set();
            }
            else if (command.IndexOf("OAL-OK") != -1)
            {
                recflag = command;
                DictRecoutObject[socketinfo.Ip].Set();
            }
            else if (command.IndexOf("CAL-OK") != -1)
            {
                recflag = command;
                DictRecoutObject[socketinfo.Ip].Set();
            }
            else if (command.IndexOf("LED-OK") != -1)
            {
                recflag = command;
                DictRecoutObject[socketinfo.Ip].Set();
            }

        }


        public bool PingTest(string Ip)
        {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(Ip, 5000);//第一个参数为ip地址，第二个参数为ping的时间 

            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        
        public void Testonline(object ListIp)
        {
            List<ClientInfo> list = (List<ClientInfo>)ListIp;
            while (true)
            {
                foreach (ClientInfo info in list)
                {
                    //Thread.Sleep(1000);
                    if (PingTest(info.Ip))
                    {
                        if (!FindIsExist(info.Ip)&&!IsApplyRetry[info.Ip])
                        {
                            IsApplyRetry[info.Ip] = true;

                            ParameterizedThreadStart pts = new ParameterizedThreadStart(Retry);
                            Thread thradRecMsg = new Thread(pts);
                            thradRecMsg.IsBackground = true;
                            thradRecMsg.Start(info);
                        }
                    }
                    else
                    {
                        if (FindIsExist(info.Ip)&&!IsApplyRetry[info.Ip])
                        {
                            int len = Findipname(frmMain.FormList,info.Ip);
                            FaiSong_Led(len, frmMain.FormList.Items[len].SubItems[3].Text, 0);
                            MessageInfo messageinfo = new MessageInfo("强行断线", info.Ip, "断网记录", DateTime.Now.ToString());//将转发记录转码后保存
                            Database.insert(messageinfo);
                            IsApplyRetry[info.Ip] = true;
                            ThreadPool[info.Ip].Abort();
                            ThreadPool.Remove(info.Ip);
                            dict[info.Ip].Socket.Close();
                            dict[info.Ip].Socket.Dispose();
                            dict.Remove(info.Ip);
                            ShowIpStauts(info.Ip, DisConnection);
                            MessageBox.Show(DisConnection);
                            ParameterizedThreadStart pts = new ParameterizedThreadStart(Retry);
                            Thread thradRecMsg = new Thread(pts);
                            thradRecMsg.IsBackground = true;
                            thradRecMsg.Start(info);
                        }
                        
                    }
                }
               

                

            }
        }
        /// <summary>
        /// 查找线程字典是否存在
        /// </summary>
        private bool FindIsExist(string Ip)
        {
            bool IsExist = false;
            foreach (string ip in dict.Keys)
            {
                if (Ip.Equals(ip))
                {
                    IsExist = true;
                }
            }
            return IsExist;
        }
        private void HandleMessage(object obj)
        {
            
            while (true)
            {
                if (Qmessage.Count>0)
                {
                    SocketInfo info = Qmessage.Dequeue();
                   
                    ContorlCommand(info);
                }
            }
        }
        /// <summary>
        /// 异步转发
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="arrMsg"></param>
        /// <param name="GanJin"></param>
        /// <returns></returns>
        public bool SendAcy(string Ip, byte[] arrMsg, string GanJin)
        {
            bool IsSendSuccess = false;
            
            try
            {

                recflag = "";
                DictSendoutObject[Ip].Reset();
                dict[Ip].Socket.Send(arrMsg);
                Thread.Sleep(timeoutRec);

               
                        if (recflag.IndexOf(GanJin) != -1)
                        {

                            IsSendSuccess = true;
                            recflag = "";
                            return IsSendSuccess;
                        }
                        else
                        {

                            return IsSendSuccess;
                        }
                   
                        

                   




            }
            catch (System.Exception)
            {

                return IsSendSuccess;
            }

        }
        //------------------------------------------------------  
        /// <summary>
        /// 接受来自服务器的信息
        /// </summary>
        /// <param name="Socketclient">服务器的socket对象</param>
        private void RecMsg(object Socketclient)
        {
            SocketInfo s = (SocketInfo)Socketclient;
            try
            {
               
                DateTime dt = DateTime.Now;
               
                while (s.Socket.Receive(s.Bufffer, 0, s.Bufffer.Length, SocketFlags.None) > 0)
                {
                    string command = Encoding.GetEncoding("GBK").GetString(s.Bufffer);
                    s.Command = command;
                    s.Bufffer = new byte[1024];
                    Qmessage.Enqueue(s);
                   
                  
                   
                   
                    
                }
            }
            catch (Exception ex)
            {
                if (FindIsExist(s.Ip) && !IsApplyRetry[s.Ip])
                {
                  
                    IsApplyRetry[s.Ip] = true;
                    ThreadPool[s.Ip].Abort();
                    ThreadPool.Remove(s.Ip);
                    dict[s.Ip].Socket.Close();
                    dict[s.Ip].Socket.Dispose();
                    dict.Remove(s.Ip);
                    ShowIpStauts(s.Ip, DisConnection);
                    MessageBox.Show(ex.ToString());
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(Retry);
                    Thread thradRecMsg = new Thread(pts);
                    thradRecMsg.IsBackground = true;
                    ClientInfo info = new ClientInfo(s.Ip,s.Port);
                    thradRecMsg.Start(info);
                }
            }
           
           
            

        }
    }
}
