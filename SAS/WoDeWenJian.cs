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
namespace SAS
{   
    class WoDeWenJian
    {
        //public string[] DataTable;
       // DataTable dt_AX = new DataTable();
       // Create a DataTable();
      DataTable dtK = new DataTable();
 
//------------读取文件-------------------------
    public string  Read(string path)
        {
            string sz="";
           // Encoding fileEncoding = new Encoding();
            //fileEncoding = TxtFileEncoding.GetEncoding(path, Encoding.GetEncoding("GB2312"));//取得这txt文件的编码 
             //Console.WriteLine("这个文本文件的编码为：" + fileEncoding.EncodingName); 
            //StreamReader sr = new StreamReader(path, fileEncoding);//用该编码创建StreamReader 
            // Console.WriteLine("这个文本文件的内容为：" + sr.ReadToEnd());
            // Console.ReadLine();
            //sr.Close();

            StreamReader sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            String line;
            while ((line = sr.ReadLine()) != null) 
            {
               // Console.WriteLine(line.ToString());
                sz = sz+line;
            }
            //MessageBox.Show(sz);
            sr.Close();
           // System.Web.HttpUtility.UrlEncode(sz, System.Text.Encoding.GetEncoding("GBK"));  
           // sz.Encoding.GetEncoding("UTF-8").GetString(System.Text.Encoding.GetEncoding("GB2312").GetBytes(str));  



            //line = sz.SYS
            //line=utf8_gb2312(sz);
            return sz;
        }
        public void BaoCunWenDang()
        {
            //保存文件

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = "E:\\";
            sfd.Filter = "TXT文件(*.txt)|*.jtxt|文件(*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //textBox2.Text = sfd.FileName;
                MessageBox.Show(sfd.FileName);
            }

        }
        public void DaKaiWenDang()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "E:\\";
            openFileDialog.Filter = "Md1 File(*.md1)|*.md1|TXT 文档(*.TXT)|*.txt|所有文档(*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show( openFileDialog.FileName);
                //Read(openFileDialog.FileName);
            }
        
        }

        //二、创建txt，并往里面填写信息。
        public void CreateFile(string path, string txt)
        {
           // FolderBrowserDialog folder = new FolderBrowserDialog();
           // folder.ShowDialog();
           // string path = folder.SelectedPath;
            //string name = "\\" + "aa.txt";
            if (!File.Exists(path))
            {
                FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(txt);//要写入的信息。 
                sw.Close();
                fs1.Close();
                MessageBox.Show("保存成功[1]！");
            }
            else 
            {
                FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(txt);//要写入的信息。 
                sw.Close();
                fs1.Close();
                MessageBox.Show("保存成功[2]！");
            }
        }



        //----------创建文件夹-----------------------------------
        public void ChuangJianWenJia(string path, string mCin)
        {
            string filename = "";
            filename = path + mCin;    //组合路径
            if (Directory.Exists(filename))//
            { //MessageBox.Show("文件夹存在"); 
            }
            else
            {
                Directory.CreateDirectory(filename);              //创建文件夹

               // MessageBox.Show("文件夹创建成功!");
            }
        }
 


        //------------------------------------------------------------
        public void ZhuiJiaNeiRong(string phed, string mytxt)
        {
            string filename = phed;
            if (!File.Exists(filename))
            {
               // FileStream fs1 = new FileStream(filename, FileMode.Create, FileAccess.Write);//创建写入文件 
               // StreamWriter sw = new StreamWriter(fs1);
               // sw.WriteLine(mytxt);//开始写入值
                //sw.Close();
                //fs1.Close();
                StreamWriter sw = new StreamWriter(filename, false, Encoding.BigEndianUnicode);
                sw.Write(mytxt); 
                sw.Close();
            }
            else
            {
                StreamWriter sw = File.AppendText(filename);
                sw.WriteLine(mytxt);
                sw.Close();

            }

        }
    //--------------------------------------------------------------
        public void ZhuiJiaNeiRong_OK(string nFame,string mytxt,string Eyt)
        {
            DateTime dt = DateTime.Now;
            string phed = "D:\\";
            string Fame = nFame;
            string filename = "";
            filename = phed + Fame + "\\" + dt.ToShortDateString().ToString() + "." + Eyt;//TXT
            ChuangJianWenJia(phed, Fame);

            if (!File.Exists(filename))
            {
                FileStream fs1 = new FileStream(filename, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(mytxt);//开始写入值
                sw.Close();
                fs1.Close();
            }
            else
            {
               StreamWriter sw = File.AppendText(filename);
               sw.WriteLine(mytxt);
               sw.Close();

            }

        }
    //---------------------------------------------------
        public  string gb2312_utf8(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = gb2312.GetBytes(text);
            gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }

    //----------------------------------------------------
        public string utf8_gb2312(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf);
        }
    //--------------------------------------------------------------
        /// 将DataTable中数据写入到CSV文件中
        /// 
        /// 提供保存数据的DataTable
        /// CSV的文件路径
        public void SaveCSV(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";
           // DataTable dt = new DataTable();

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                    ;
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
//------------------------------------------------------------
        /// 将CSV文件的数据读取到DataTable中
        /// 
        /// CSV文件路径
        /// 返回读取了CSV数据的DataTable
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
     //-------------------------------------------------------------

    }
}

