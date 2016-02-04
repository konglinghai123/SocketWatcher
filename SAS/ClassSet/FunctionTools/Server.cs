using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.IO;

namespace SAS.ClassSet.FunctionTools
{
    class Server
    {
        private  Thread threadWatch = null;
        private  Socket socketWatch = null;
        UIShow uishow = new UIShow();
        //private static ListBox lstbxMsgView;//显示接受的文件等信息
        //private static ListBox listbOnline;//显示用户连接列表
        private  ListView list;
        private static Dictionary<string, Socket> dict = new Dictionary<string, Socket>();
        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="localIp"></param>
        /// <param name="localPort"></param>
        public  void BeginListening(string localIp, string localPort, ListView listonline)
        {
            //基本参数初始化
            //lstbxMsgView = listbox;
            //listbOnline = listboxOnline;
            list = listonline;
            //创建服务端负责监听的套接字，参数（使用IPV4协议，使用流式连接，使用Tcp协议传输数据）
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取Ip地址对象
            IPAddress address = IPAddress.Parse(localIp);
            //创建包含Ip和port的网络节点对象
            IPEndPoint endpoint = new IPEndPoint(address, int.Parse(localPort));
            //将负责监听的套接字绑定到唯一的Ip和端口上
            socketWatch.Bind(endpoint);
            //设置监听队列的长度
            socketWatch.Listen(200);
            //创建负责监听的线程，并传入监听方法
            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;//设置为后台线程
            threadWatch.Start();//开始线程
            MessageBox.Show("服务器已成功启动");


        }

        /// <summary>
        /// 连接客户端
        /// </summary>
        private  void WatchConnecting()
        {
            while (true)//持续不断的监听客户端的请求
            {
                //开始监听 客户端连接请求，注意：Accept方法，会阻断当前的线程
                Socket connection = socketWatch.Accept();
                if (connection.Connected && !dict.ContainsKey(connection.RemoteEndPoint.ToString().Substring(0, connection.RemoteEndPoint.ToString().IndexOf(":"))))
                {
                    //向列表控件中添加一个客户端的Ip和端口，作为发送时客户的唯一标识
                    // listbOnline.Items.Add(connection.RemoteEndPoint.ToString());
                    //将与客户端通信的套接字对象connection添加到键值对集合中，并以客户端Ip做为健

                    //list.Items.Add("IP地址", connection.RemoteEndPoint.ToString());
                    string[] str = new string[]{
                        "客户端在线",
                      connection.RemoteEndPoint.ToString().Substring(0, connection.RemoteEndPoint.ToString().IndexOf(":"))
                    };
                    uishow.ShwMsgforView(list, str);
                    dict.Add( connection.RemoteEndPoint.ToString().Substring(0, connection.RemoteEndPoint.ToString().IndexOf(":")), connection);

                    //创建通信线程
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(RecMsg);
                    Thread thradRecMsg = new Thread(pts);
                    thradRecMsg.IsBackground = true;
                    thradRecMsg.Start(connection);

                }
              
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="socketClientPara"></param>
        private  void RecMsg(object socketClientPara)
        {
            Socket socketClient = socketClientPara as Socket;
            string ip = socketClient.RemoteEndPoint.ToString().Substring(0, socketClient.RemoteEndPoint.ToString().IndexOf(":"));
            while (true)
            {
                //定义一个接受用的缓存区（100M字节数组）
                //byte[] arrMsgRec = new byte[1024 * 1024 * 100];
                //将接收到的数据存入arrMsgRec数组,并返回真正接受到的数据的长度   
                if (socketClient.Connected)
                {
                    try
                    {
                        //因为终端每次发送文件的最大缓冲区是512字节，所以每次接收也是定义为512字节
                        byte[] buffer = new byte[512];
                        int size = 0;
                        long len = 0;
                        //string fileSavePath = @"Receivefile";//获得用户保存文件的路径
                        //if (!Directory.Exists(fileSavePath))
                        //{
                        //    Directory.CreateDirectory(fileSavePath);
                        //}
                        //string fileName = fileSavePath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".doc";
                        ////创建文件流，然后让文件流来根据路径创建一个文件
                        //FileStream fs = new FileStream(fileName, FileMode.Create);
                        ////从终端不停的接受数据，然后写入文件里面，只到接受到的数据为0为止，则中断连接
                        string command = "";
                        DateTime oTimeBegin = DateTime.Now;

                        while ((size = socketClient.Receive(buffer, 0, buffer.Length, SocketFlags.None)) > 0)
                        {
                           // fs.Write(buffer, 0, size);
                            command = Encoding.GetEncoding("GBK").GetString(buffer, 0, size);
                            len += size;
                            MessageBox.Show(command);
                            Client cle = new Client();
                            cle.send("", Encoding.GetEncoding("GBK").GetBytes(command.Trim()));
                            
                             
                        }
                     
                        //DateTime oTimeEnd = DateTime.Now;
                        //TimeSpan oTime = oTimeEnd.Subtract(oTimeBegin);
                        //fs.Flush();

                        dict.Remove(socketClient.RemoteEndPoint.ToString().Substring(0, socketClient.RemoteEndPoint.ToString().IndexOf(":")));
                        //listbOnline.Items.Remove(socketClient.RemoteEndPoint.ToString());


                        socketClient.Close();


                    }
                    catch
                    {

                        dict.Remove(socketClient.RemoteEndPoint.ToString().Substring(0, socketClient.RemoteEndPoint.ToString().IndexOf(":")));
                        //listbOnline.Items.Remove(socketClient.RemoteEndPoint.ToString());
                        string[] str = new string[]{
                        "客户端掉线",
                       socketClient.RemoteEndPoint.ToString().Substring(0,  socketClient.RemoteEndPoint.ToString().IndexOf(":"))
                      
                    };
                        uishow.ShwMsgforView(list, str);
                        break;
                    }
                }
                else
                {
                    dict.Remove(ip);
                    //listbOnline.Items.Remove(socketClient.RemoteEndPoint.ToString());
                    string[] str = new string[]{
                        "客户端掉线",
                      ip
                      
                    };
                    uishow.ShwMsgforView(list, str);
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public  void CloseTcpSocket()
        {
            dict.Clear();
            //listbOnline.Items.Clear();
            threadWatch.Abort();
            socketWatch.Close();

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public void sendmessage(string targer, byte[] arrMsg)
        {

            try
            {
                if (targer=="")
                {
                    foreach(Socket s in dict.Values){
                        s.Send(arrMsg);
                    }
                } 
                else
                {

                    dict[targer].Send(arrMsg);
                }
              
                
                //sokConnection.Send(arrMsg);
            
            }
            catch (SocketException )
            {
               
            }
            catch (Exception )
            {
                
            }
        }
    }
}
