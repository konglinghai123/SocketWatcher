using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using SAS.Forms;
using ICSharpCode.SharpZipLib.Zip;
using SAS.ClassSet.FunctionTools;
using SAS.ClassSet.Common;
using SAS.ClassSet.MemberInfo;
using SAS.ClassSet;
using System.Threading;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Runtime.InteropServices;



namespace SAS
{
    public partial class frmMain : Form
    {
        public static frmMain fm;
        public static List<string> HostIp = new List<string>();//主机Ip地址集合
        public static Dictionary<string, string> IpAndRec = new Dictionary<string, string>();//用于存储对于Ip所接受到的信息
        public static Dictionary<string, string> IpAndName = new Dictionary<string, string>();//用于存储Ip对应的机房名称
        private static System.Diagnostics.Process p;
        public string Bufs;
        private int TM1 = 0;
        //-----------------------------------
       // Timer timer = new System.Timers.Timer();  
        // timer.Enabled = true;  
         //timer.Interval = 60000;//执行间隔时间,单位为毫秒   
        // timer.Start();  
         // timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);  
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);


        //-----------------------------------
        public frmMain()
        {
            InitializeComponent();
            fm = this;
            FormList = this.listView1;
            ListView.CheckForIllegalCrossThreadCalls = false;
         
             
            
        }
        SqlHelper helper = new SqlHelper();
        public static ListView FormList;
        ParameterizedThreadStart pts;
        Thread thradRecMsg;
        Client client = new Client();
        List<ClientInfo> List = new List<ClientInfo>();
        Insert2DataBase insert = new Insert2DataBase();//增加日志对象
        DataTable dt;
        
    




    //-----------------------------------------------------
        /// <summary>
        /// 设置Listviewd的属性
        /// </summary>
        private void SetListviewBorder()
        {
            ImageList il = new ImageList();
           
             //设置高度
            il.ImageSize = new Size(1, 26);
            //绑定listView控件
            listView1.SmallImageList = il;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // 开启双缓冲
            //DateTime dt = DateTime.Now;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            SetListviewBorder();
            // Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
           
            if (Common.load())
            {
                ListViewShow();

                startconnect();
                //timer1.Start();
                
            }
            else
            {
                MessageBox.Show("找不到数据库");
            }

        }

        private void tsbSet_Click(object sender, EventArgs e)
        {

            frmSetting f = new frmSetting();
            f.Show();
            
        }


        /// <summary>
        /// 开始向所有服务发送连接请求，即判断是否在线
        /// </summary>
        private void startconnect()
        {
            DataTable dt = helper.getDs("select * from MedicineInfo", "MedicineInfo").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ClientInfo info = new ClientInfo(dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString());
                List.Add(info);
            }


            pts = new ParameterizedThreadStart(client.connect);
            thradRecMsg = new Thread(pts);
            thradRecMsg.IsBackground = true;
            thradRecMsg.Start(List);
            
           
        }
        private void groupBox3_Enter(object sender, EventArgs e)
        {
           
        }

        private void 查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          if (listView1.CheckedItems.Count==1)
          {
              if (listView1.CheckedItems[0].SubItems[2].Text=="在线")
              {

                    if (!backgroundWorker1.IsBusy)
                    {
                        backgroundWorker1.RunWorkerAsync(backgroundWorker1);
                    } 
                    else
                    {
                        MessageBox.Show("正在等待上一次的返回结果，请稍后");
                    }
                        
                  
                
              } 
              else
              {
                  MessageBox.Show("请选择在线设备");
              }
          } 
          else
          {
          }
        }
      
        /// <summary>
        /// 向服务器发送信息
        /// </summary>
        public void MsgToServer()
        {
            List<string> ListIp = new List<string>();
            Client sender = new Client();
            string strMsg = "+查询主机\r\n";//指令
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(strMsg.Trim());//转码为Byte数组（GBK）
            //获取当前在线的服务器集合，存储在ListIp中
             for (int i = 0; i < listView1.Items.Count;i++ )
            {
                if (listView1.Items[i].SubItems[2].Text=="在线")
                {
                    ListIp.Add(listView1.Items[i].SubItems[1].Text);
                }
            }
            //遍历在线集合，发送指令数组data
            foreach(string IpNode in ListIp){
                sender.send(IpNode, data);
            }
            //Thread.Sleep(10000);
            
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //button3.Enabled = false;
            if (listView1.CheckedItems.Count == 1)
            {
                if (listView1.CheckedItems[0].SubItems[2].Text == "在线")
                {
                     if (!backgroundWorker2.IsBusy)
                     {
                          backgroundWorker2.RunWorkerAsync(backgroundWorker2);
                     } 
                     else
                     {
                         //string GJ = "";
                         //if(SendAcy(host, bytearray, GJ)!=-1){
                         MessageBox.Show("正在等待上一次操作的返回结果");

                     }
                   

                }
                else
                {
                    MessageBox.Show("请选择在线设备");
                }
            }
            else
            {
            }
            
        }
        /// <summary>
        /// 刷新主界面
        /// </summary>
        public void ListViewShow()
        {  
            Client.dict.Clear();
            HostIp.Clear();
            IpAndName.Clear();
            IpAndRec.Clear();
            dt = helper.getDs("select * from MedicineInfo", "MedicineInfo").Tables[0];
            listView1.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] str = new string[]{
                    (i+1).ToString(),
                    dt.Rows[i][2].ToString(),
                     "断线",
                        dt.Rows[i][0].ToString(),
                          dt.Rows[i][1].ToString(),"","","","","","","","","","","","",""
                    };

                ListViewItem lit = new ListViewItem(str);
                listView1.Items.Add(lit);
            }
            
            DataRow[] dr = dt.Select("Type='主机'");
            for (int i = 0; i < dr.Length;i++ )
            {
                HostIp.Add(dr[i][2].ToString());
            }
            for (int i = 0; i < listView1.Items.Count;i++ )
            {
                listView1.Items[i].BackColor = System.Drawing.Color.Red;
               
                IpAndName.Add(listView1.Items[i].SubItems[1].Text, listView1.Items[i].SubItems[3].Text);
               
              

            }
        }

        private void 更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.CloseThread();
            Application.Restart();
           
        }
        /*private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e) 
        {
        
        
        }*/
     
       
        public void LisVw1(string ips,int le,string txt) 
        {  string Ip=ips;
            
            for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (Convert.ToString(this.listView1.Items[i].SubItems[1].Text) == Ip)
                        {
                            //if ((listView1.Items[i].SubItems[2].Text) == "断线")
                            //{

                            listView1.Items[i].SubItems[le].Text = Convert.ToString(txt);
                            //}
                        }
                    }
               
        
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            Point curPos = this.listView1.PointToClient(Control.MousePosition);
            ListViewItem lvwItem = this.listView1.GetItemAt(curPos.X, curPos.Y);
            //--------------------------------------------------------------------
       
           // textBox5.Text = this.listView1.Items[5].Text;
            //textBox1.Text = this.listView1.Items[3].Text;
            //MessageBox.Show("wr-"+i);
            //--------------------------------------------
            

            //---------------------------------------------------------------------
            foreach (ListViewItem s in listView1.Items)
            {
                s.Checked = false;
                s.Selected = false;
            }
            if (lvwItem != null)
            {

                lvwItem.Selected = true;
                if (e.X > 16) lvwItem.Checked = true;
             
               
            }
            else { }
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // Server s = new Server();
           // s.BeginListening("192.168.1.102", "8555", listView1);
           // MessageBox.Show("服务器已开启");
            WoDeWenJian wwj = new WoDeWenJian();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "D:\\";
            openFileDialog.Filter = "文件(*.txt)|*.txt|所有文件(*.*)|*.*|CSV文件(*.CSV)|*.csv";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
          
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(openFileDialog.FileName);
                textBox8.Text = wwj.Read(openFileDialog.FileName);
            //    textBox8.Text = "";
               
            }

        }
    
        #region 将控件的状态转换为命令
        /// <summary>
        /// 将所有复选框的状态转换为0/1
        /// </summary>
        /// <returns>开始设防的数据，即各开关状态</returns>
        private string configcommand()
        {
            int[] shefang = new int[7];
            string command = "";
            foreach (Control c in groupBox1.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox ch = (CheckBox)c;
                    switch (ch.Text)
                    {
                        case "射频":
                            if (ch.Checked)
                            {
                                shefang[3] = 1;
                            }
                            else
                            {
                                shefang[3] = 0;
                            }
                            break;
                        case "烟感":
                            if (ch.Checked)
                            {
                                shefang[0] = 1;
                            }
                            else
                            {
                                shefang[0] = 0;
                            }
                            break;
                        case "市电":
                            if (ch.Checked)
                            {
                                shefang[4] = 1;
                            }
                            else
                            {
                                shefang[4] = 0;
                            }
                            break;
                        case "门禁":
                            if (ch.Checked)
                            {
                                shefang[1] = 1;
                            }
                            else
                            {
                                shefang[1] = 0;
                            }
                            break;
                        case "外烟":
                            if (ch.Checked)
                            {
                                shefang[5] = 1;
                            }
                            else
                            {
                                shefang[5] = 0;
                            }
                            break;
                        case "视频":
                            if (ch.Checked)
                            {
                                shefang[2] = 1;
                            }
                            else
                            {
                                shefang[2] = 0;
                            }
                            break;
                        case "温度":
                            if (ch.Checked)
                            {
                                shefang[6] = 1;
                            }
                            else
                            {
                                shefang[6] = 0;
                            }
                            break;
                    }

                }


            }
            foreach (int i in shefang)
            {
                command = command + i.ToString();
            }
            return command;
        }
        /// <summary>
        /// 常开/常闭
        /// </summary>
        /// <returns></returns>
        private string effvalue()
        {
            if (radioButton1.Checked)
            {
                return "常开";
            }
            else
            {
                return "常闭";
            }
        }
        /// <summary>
        /// 所有的电话，包括授权电话和报警电话
        /// </summary>
        /// <returns></returns>
        private string phone()
        {
            int i = 0;
            string tel = "";
            foreach (TextBox tb in groupBox4.Controls)
            {
                if (tb.Text != "")
                {
                    tel = tel + ";" + tb.Text.Trim();
                }
                else
                {
                    if (i == 0) { tel = tel + ";"; }
                
                }
                i = i + 1;


            }
            tel = tel + ";";
            return tel.Substring(1);
        }
        /// <summary>
        /// 将地址转换为Unicode码
        /// </summary>
        /// <param name="strEncode"></param>
        /// <returns></returns>
        public static string Encode(string strEncode)
        {
            string strReturn = "";//  存储转换后的编码
            foreach (short shortx in strEncode.ToCharArray())
            {
                strReturn += shortx.ToString("X4");
            }
            return strReturn;
        }
        /// <summary>
        /// 将Uincode转中文
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        public static string Decode(string strDecode)
        {
            string sResult = "";
            try
            {
                for (int i = 0; i < strDecode.Length / 4; i++)
                {
                    sResult += (char)short.Parse(strDecode.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber);
                }
                return sResult;
            }
            catch
            {
                return strDecode;
            }
        }
        /// <summary>
        /// 编辑成命令
        /// </summary>
        /// <returns></returns>
        private string formatcommand()
        {
            string Mphone = phone().Substring(0, phone().IndexOf(";"));
            string Ephone = phone().Substring(phone().IndexOf(";") + 1);
            string command = string.Format("+主机设防[开始设定[{0}]授权[{1}]报警[{2}]地址[{3}][{4}]温度[{5}]延时[{6}]响声[{7}]发送[{8}]修改名称[1]70DF96FE[2]95E87981[3]89C69891[4]5C049891[5]75356E90[6]591670DF结束", configcommand(), Mphone
                , Ephone, Encode(textBox1.Text.Trim()), effvalue(), textBox2.Text.Trim(), textBox4.Text.Trim(), textBox3.Text.Trim(), textBox6.Text.Trim());
            //MessageInfo info = new MessageInfo(command, textBox4.Text, "设防", DateTime.Now.ToLongTimeString());
            return command;
        }
        #endregion

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string strMsg = "+查询主机\r\n";//指令
            //将要发送的字符串 转成 utf8对应的字节数组

            //获得列表中 选中的KEY
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(strMsg.Trim());
            string GJ = "-OK";
            if (client.SendAcy(listView1.CheckedItems[0].SubItems[1].Text, data,GJ))
            {
                MessageBox.Show("查询成功");
                strMsg ="[查询成功-Query Success]";
               // MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text, "查询操作", DateTime.Now.ToString());
                //insert.insert(info);
            }
            else
            {
                MessageBox.Show("查询失败");
                strMsg = "[查询失败-Query Failure]";
            }
            MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text, "查询操作", DateTime.Now.ToString());
            insert.insert(info);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //查询操作完成后
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {   
            string strMsg = formatcommand();//发送设防命令
            //将要发送的字符串 转成 utf8对应的字节数组

            //获得列表中 选中的KEY
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(strMsg.Trim());
            string GJ="-OK";
            if (client.SendAcy(listView1.CheckedItems[0].SubItems[1].Text, data, GJ))
            {
                MessageBox.Show("设置成功");
                strMsg = "[设置成功-Set Up Successfully]";
               // MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text,"设置操作",DateTime.Now.ToString());
               // insert.insert(info);
            }
            else
            {
                MessageBox.Show("设置失败");
                strMsg = "[设置失败-Set Failure]";
            }
            MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text, "设防操作", DateTime.Now.ToString());
            insert.insert(info);

        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //设防操作完成后
        }

        private void tsbLog_Click(object sender, EventArgs e)
        {
            Logs formlog = new Logs();
            formlog.Show();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //client.CloseThread();
        }

        private void button4_Click(object sender, EventArgs e)
        {   //---------------读取置----------;
            WoDeWenJian wwj = new WoDeWenJian();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "D:\\";
            openFileDialog.Filter = "文件(*.FON)|*.FON";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            string stl = "";
            string stm = "";
           
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(openFileDialog.FileName);
                string command = wwj.Read(openFileDialog.FileName);
                if (command.IndexOf("+主机设防[") != -1)
                {
                    string zs = "";
                    zs = command.Substring((command.IndexOf("开始设定[") + 5), 7);
                   // MessageBox.Show(zs);
                    JieXiMingLing(zs);
                    //--------------------------------------------------
                    if (command.IndexOf("[常闭]") != -1) { radioButton2.Checked = true; radioButton1.Checked = false; }
                    if (command.IndexOf("[常开]") != -1) { radioButton1.Checked = true; radioButton2.Checked = false; }
                    stl = "授权[";
                    stm = "]报";
                    zs = ZiFuChn(command, stl, stm);

                        textBox15.Text = zs;
                //---------------------------------------------------------
                        stl = "报警[";
                        stm = "]地";
                        zs = ZiFuChn(command, stl, stm);
                        //MessageBox.Show(zs);
                        string[]sAry = zs.Split(';');
                        int i=0;
                        foreach (string we in sAry)
                        {
                         i = i + 1;
                        }
                        //MessageBox.Show("MPF[" + i+"]");
                        textBox14.Text = "";
                        textBox13.Text = "";
                        textBox10.Text = "";
                        textBox18.Text = "";
                        textBox16.Text = "";
                        for (int x = 0; x < i; x++)
                        {
                            switch (x)
                            {
                                case 0:
                                    textBox14.Text=sAry[x];
                                    break;
                                case 1:
                                    textBox13.Text = sAry[x];
                                    break;
                                case 2:
                                    textBox10.Text = sAry[x];
                                    break;
                                case 3:
                                    textBox18.Text = sAry[x];
                                    break;
                                case 4:
                                    textBox16.Text = sAry[x];
                                    break;

                            }
                        }
//-------------------------------------------------------------------------
  
                    stl = "温度[";
                    stm = "]延";
                    zs = ZiFuChn(command, stl, stm);
                    textBox2.Text = zs;
//-------------------------------------------------------------------------
                    stl = "延时[";
                    stm = "]响";
                    zs = ZiFuChn(command, stl, stm);
                    textBox4.Text = zs;
//------------------------------------------------------------------------------
                    stl = "响声[";
                    stm = "]发";
                    zs = ZiFuChn(command, stl, stm);
                    textBox3.Text = zs;
//---------------------------------------------------------------------------
                    stl = "发送[";
                    stm = "]修";
                    zs = ZiFuChn(command, stl, stm);
                    textBox6.Text = zs;
//------------------------------------------------------------------------
                  stl="地址[";
                  stm="][";
                  zs=ZiFuChn(command,stl,stm);
                  if (zs != "")
                  {
                      textBox1.Text = frmMain.Decode(zs);//
                  }
                  else
                  {
                      textBox1.Text = zs;//
                  }
//------------------------------------------------------------
                    
                } 
                
            }

        }
        //--------------------------------------------------
        public static string ZiFuChn(string Bufm ,string stl, string stm)
        {           string lsz1 = stl;
                    string lsz2 = stm;
                    string command=Bufm;
                    string zs="";
                    int i = lsz1.Length;
                    int i1 = lsz2.Length;
                    int IndexofA = command.IndexOf(lsz1);
                    int IndexofB = command.IndexOf(lsz2);
                    zs = command.Substring(IndexofA + i, IndexofB - IndexofA - (i1+1));
                    if (zs != lsz2)
                    {

                        return zs;
                
                    }
                    else { return zs;}
                    
                    
            
        }
        //--------------------------------------------------

        private void button2_Click(object sender, EventArgs e)
        {        //保存文件

            SaveFileDialog sfd = new SaveFileDialog();
            WoDeWenJian wwj = new WoDeWenJian();
            string strMsg = formatcommand();//发送设防命令
            sfd.InitialDirectory = "D:\\";
            sfd.Filter = "文件(*.FON)|*.FON";
            if (sfd.ShowDialog() == DialogResult.OK)
            {


                wwj.CreateFile(sfd.FileName,strMsg);
            }
            
           
        }

        private void JieXiMingLing(string srtu)
        {
            Boolean bj;
            string sd = "";
            string command = srtu;
            for(int i=0;i<7;i++){
            sd = command.Substring(i,1);
            if (sd == "1") { bj = true; }
            else { bj = false; }
            switch(i)
            {    case 0:
                    checkBox9.Checked = bj;
                    break;
            case 1:
                    checkBox12.Checked = bj;
                    break;
            case 2:
                    checkBox7.Checked = bj;
                    break;
            case 3:
                    checkBox11.Checked = bj;
                    break;
            case 4:
                    checkBox8.Checked = bj;
                    break;
                    
            case 5:
                    checkBox10.Checked = bj;
                    break;
            case 6:
                    checkBox13.Checked = bj;
                    break;
            
             }
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void 关警报ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 1)
            {
                if (listView1.CheckedItems[0].SubItems[2].Text == "在线")
                {

                    if (!backgroundWorker3.IsBusy)
                    {
                        backgroundWorker3.RunWorkerAsync(backgroundWorker3);
                    }
                    else
                    {
                        MessageBox.Show("正在等待上一次的返回结果，请稍后");
                    }



                }
                else
                {
                    MessageBox.Show("请选择在线设备");
                }
            }
            else
            {
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            string strMsg = "+关闭铃声\r\n";//指令
            //将要发送的字符串 转成 utf8对应的字节数组

            //获得列表中 选中的KEY
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(strMsg.Trim());
            string GJ = "-OK";
            if (client.SendAcy(listView1.CheckedItems[0].SubItems[1].Text, data, GJ))
            {
                MessageBox.Show("成功关闭");
                strMsg = "[成功关闭警报声音]";
                // MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text, "查询操作", DateTime.Now.ToString());
                //insert.insert(info);
            }
            else
            {
                MessageBox.Show("关闭警报操作失败!");
                strMsg = "[关闭警报失败,超时?]";
            }
            MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text, "查询操作", DateTime.Now.ToString());
            insert.insert(info);
        }

        private void 开警报ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 1)
            {
                if (listView1.CheckedItems[0].SubItems[2].Text == "在线")
                {

                    if (!backgroundWorker4.IsBusy)
                    {
                        backgroundWorker4.RunWorkerAsync(backgroundWorker4);
                    }
                    else
                    {
                        MessageBox.Show("正在等待上一次的返回结果，请稍后");
                    }



                }
                else
                {
                    MessageBox.Show("请选择在线设备");
                }
            }
            else
            {
            }
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            string strMsg = "+打开警报\r\n";//指令
            //将要发送的字符串 转成 utf8对应的字节数组

            //获得列表中 选中的KEY
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(strMsg.Trim());
            string GJ = "-OK";
            if (client.SendAcy(listView1.CheckedItems[0].SubItems[1].Text, data, GJ))
            {
                MessageBox.Show("成功打开");
                strMsg = "[成功打开警报声音]";
                // MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text, "查询操作", DateTime.Now.ToString());
                //insert.insert(info);
            }
            else
            {
                MessageBox.Show("打开警报操作失败!");
                strMsg = "[打开警报失败!超时?]";
            }
            MessageInfo info = new MessageInfo(strMsg, listView1.CheckedItems[0].SubItems[1].Text, "查询操作", DateTime.Now.ToString());
            insert.insert(info);
        }

        private void textBox15_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           
        }

        private void textBox15_KeyPress(object sender, KeyPressEventArgs e)
        {    e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}");} 
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {
                
                e.Handled = false; 
            }

        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox18_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}"); }
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
            {

                e.Handled = false;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) { SendKeys.Send("{tab}");}
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView1.SelectedIndices;
                DateTime dt = DateTime.Now;
                string si1 = "";
                string si2 = "";
                string ts = "";
                si1 = listView1.Items[c[0]].SubItems[6].Text;
                if (si1!="")
                {
                    textBox2.Text = si1.Remove(0, 1);
                }
                else { textBox2.Text = ""; }
                 si1 = listView1.Items[c[0]].SubItems[5].Text;
                textBox5.Text = si1;
                textBox1.Text = listView1.Items[c[0]].SubItems[3].Text;
                if (si1.IndexOf("开") != -1) { checkBox13.Checked = true; }
                else { checkBox13.Checked = false; }
                for (int i = 7; i < 13; i++)
                {
                    si1 = listView1.Items[c[0]].SubItems[i].Text;
                    switch(i)
                    {
                        case 7:
                            if (si1.IndexOf("开") != -1) { checkBox10.Checked = true; }
                            else { checkBox10.Checked = false; }
                            ts = "1-外烟[ ";
                            break;
                        case 8:
                            if (si1.IndexOf("开") != -1) { checkBox9.Checked = true; }
                            else { checkBox9.Checked = false; }
                            ts = "2-烟感[ ";
                            break;
                        case 9:
                            if (si1.IndexOf("开") != -1) { checkBox12.Checked = true; }
                            else { checkBox12.Checked = false; }
                            ts = "3-射频[ ";
                            break;
                        case 10:
                            if (si1.IndexOf("开") != -1) { checkBox7.Checked = true; }
                            else { checkBox7.Checked = false; }
                            ts = "4-视频[ ";
                            break;
                        case 11:
                            if (si1.IndexOf("开") != -1) { checkBox8.Checked = true; }
                            else { checkBox8.Checked = false; }
                            ts = "5-市电[ ";
                            break;

                        case 12:
                            if (si1.IndexOf("开") != -1) { checkBox11.Checked = true; }
                            else { checkBox11.Checked = false; }
                            ts = "6-门禁[ ";
                            break;
    
                    }
                    si2 = si2 + ts+si1 + " ]\r\n";
                }
                textBox8.Text = si2 + dt.ToLocalTime().ToString();
                //JieXiMingLing("101010");
            }
        }

        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
        }

        private void SevCsvTxk()
        {
            WoDeWenJian WWJK = new WoDeWenJian();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string[,] str = new string[3, 3]; 
  
            str[0, 0] = "t"; 
            str[0, 1] = "o"; 
            str[0, 2] = "m"; 
            str[1, 0] = "k"; 
            str[1, 1] = "f"; 
            str[1, 2] = "t"; 
            str[2, 0] = "y"; 
            str[2, 1] = "u"; 
            str[2, 2] = "n"; 
  
            string line = string.Empty; 
            for (int i = 0; i < 3; i++) 
            { 
                  
                for (int j = 0; j < 3; j++) 
                { 
                    line += str[i, j] + ","; 
                } 
                line = line.TrimEnd(','); 
                line += "\r\n"; 
            }
            File.WriteAllText(@"F:\data.CSV", line); 
     

           
        
            openFileDialog1.Filter = "CSV文件|*.CSV";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                //this.dt.DataSource = null;
               // this.dgvShow.DataSource = null;
                //string fileName = openFileDialog1.FileName;
               // this.dgvShow.DataSource = OpenCSV(fileName);
             
                MessageBox.Show("成功显示CSV数据！");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
           /* listView1.Focus();
            listView1.Items[0].Checked = true;
            if(listView1.CheckedItems[0].SubItems[2].Text=="在线")
            {
                Client sdt = new Client();
                string command = "KXUYFSJJGDGSJ12358";
                byte[] bytearray = Encoding.Unicode.GetBytes(command);
                string Ip = listView1.CheckedItems[0].SubItems[1].Text;
                sdt.send(Ip, bytearray);
            }*/
        //-------------------------------------------
            button5.Text = "正在调用";
            button5.Enabled = false;
            try
            {
                
                if (p == null)
                {  
                    p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = @"D:\模块设置\TCPIP.exe";
                    p.Start();
                    p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                }
                else
                {
                    if (p.HasExited) //是否正在运行
                    {
                        p.Start();
                        p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                    }
                }
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                button5.Enabled = true;
                button5.Text = "模块设置";
            }
            catch (Exception ex)
            {
                //MessageBox.Show("系统找不到只定文件[" + p.StartInfo.FileName +"]");
                MessageBox.Show("调用程序时出错!原因:" + ex.Message + "[" + p.StartInfo.FileName + "]");
                button5.Enabled = true;
                button5.Text = "模块设置";
            }
        //-------------------------------------------
        }
        private void button6_Click(object sender, EventArgs e)
        {
            //button6.Enabled = false;
            GengXin_OK();
           // button6.Enabled =true;
          
        }
       
      
        public void GengXin_OK()
        {
            UIShow ushow = new UIShow();
            for (int i = 1; i <= 8 && i < listView1.Items.Count; i++)
            {

                ushow.Led_XianShiGngXin(i, listView1);
                Thread.Sleep(1000);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
           

        }
   
    public DataTable OpenCSV(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }

/// 将DataTable中数据写入到CSV文件中
       /// 
        /// 提供保存数据的DataTable
        /// CSV的文件路径
 public void SaveCSV(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            MessageBox.Show("CSV文件保存成功！");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GengXin_OK();
            timer1.Stop();
        }

        private void tssbSend_ButtonClick(object sender, EventArgs e)
        {
            tssbSend.ShowDropDown();
        }









        //-------------------------------------------------------------------
    }
}

