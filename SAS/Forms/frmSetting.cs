using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAS.ClassSet.Common;
using System.IO;
using SAS.ClassSet.MemberInfo;
using SAS.ClassSet.FunctionTools;
using System.Data.OleDb;
using System.Text.RegularExpressions;
namespace SAS.Forms
{   
    public partial class frmSetting : Form
    {

        public frmSetting()
        {
            InitializeComponent();
        }
        private SqlHelper helper = new SqlHelper();
        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            ListViewShow();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (IsIP(textBox1.Text.Trim())&&textBox2.Text!="")
            {
                if (IsIntger(textBox3.Text))
                {
                    Insert2DataBase();
                    ListViewShow();
                    frmMain.fm.ListViewShow();
                  } 
                 else
                 {
                 MessageBox.Show("端口号数据异常");
                }
               
             } 
             else
             {
                 MessageBox.Show("请填写正确的Ip地址");
             }
           
        }
        /// <summary>
        /// 向数据库添加记录
        /// </summary>
        private void Insert2DataBase()
        {
          try
          {
              DataTable dt = helper.getDs("select * from MedicineInfo", "MedicineInfo").Tables[0];
              DataRow dr = dt.NewRow();
              dr[0] = textBox2.Text;
              dr[1] = comboBox1.SelectedItem.ToString();
              dr[2] = textBox1.Text;
              dr[3] = textBox3.Text;
              dt.Rows.Add(dr);
              OleDbDataAdapter da = helper.adapter("select * from MedicineInfo");
              da.Update(dt);
              MessageBox.Show("添加成功");

            
          }
          catch (Exception)
          {
              MessageBox.Show("请检测是否有重复输入主机地址");

          }
          
               
        }
        /// <summary>
        /// Ip检测
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public  bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 端口号检测
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsIntger(string text)
        {   try
            {
            int a =  Convert.ToInt32(text.Trim());
            if (a<=65535&&a>0)
            {
                return true;
            } 
            else
            {
                return false;
            }
          
            }
             catch (Exception )
            {
            return false;
             }
               
        }
        /// <summary>
        /// 将数据库当前的数据显示在listview
        /// </summary>
        public void ListViewShow(){
            DataTable dt = helper.getDs("select * from MedicineInfo", "MedicineInfo").Tables[0];
            listView1.Items.Clear();
            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                string[] str = new string[]{
                    (i+1).ToString(),
                    dt.Rows[i][0].ToString(),
                      dt.Rows[i][1].ToString(),
                        dt.Rows[i][2].ToString(),
                          dt.Rows[i][3].ToString()
                    };
                ListViewItem lit = new ListViewItem(str);
                listView1.Items.Add(lit);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {    
              string condition =string.Format("where Address='{0}'",textBox2.Text.Trim());
            if (   helper.delete("MedicineInfo", condition)>0)
            {
                MessageBox.Show("删除成功");
                ListViewShow();
                frmMain.fm.ListViewShow();
            }
            else
            {
                MessageBox.Show("请检查是否存在该地址");
            }
          
          
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {

            Point curPos = this.listView1.PointToClient(Control.MousePosition);
            ListViewItem lvwItem = this.listView1.GetItemAt(curPos.X, curPos.Y);
            foreach (ListViewItem s in listView1.Items)
            {
                s.Checked = false;
                s.Selected = false;
            }
            if (lvwItem != null)
            {
                lvwItem.Selected = true;
                if (e.X > 16) lvwItem.Checked = true;
                TextBoxSetText(lvwItem);
            }
            else { }
        }
        private void TextBoxSetText(ListViewItem lvi){
            textBox1.Text = lvi.SubItems[3].Text;
            textBox2.Text = lvi.SubItems[1].Text;
            textBox3.Text = lvi.SubItems[4].Text;
            if (lvi.SubItems[2].Text=="主机")
            {
                comboBox1.SelectedIndex = 0;
            } 
            else
            {
                comboBox1.SelectedIndex = 1;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string updatecommand = string.Format("update MedicineInfo set Address='{0}' ,Type='{1}' ,Ip='{2}' , Port='{3}' where Address='{4}' ", textBox2.Text, comboBox1.Text, textBox1.Text, textBox3.Text, listView1.CheckedItems[0].SubItems[1].Text);

               // string updatecommand = string.Format("update MedicineInfo set Address='{0}' ,Type='{1}' ,Ip='{2}' , Port='{3}' where Address='{4}' ", textBox2.Text, comboBox1.Text,textBox1.Text,textBox3.Text,textBox2.Text);
                if (helper.Oledbcommand(updatecommand) > 0)
                {
                    MessageBox.Show("修改成功");
                    ListViewShow();
                    frmMain.fm.ListViewShow();
                }
                else
                {
                    MessageBox.Show("修改失败");
                }
               

               
            }
            catch (Exception ex)
            {
                MessageBox.Show("请检测是否有重复输入主机地址"+ex.ToString());

            }
        }
        private int IsExistsItem(string text, ListView listView1)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].SubItems[3].Text == text)
                {
                    return i;
                }
            }

            return -1;

        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
           
                helper.delete("MedicineInfo","");
              
                OpenFileDialog openFile = new OpenFileDialog();//打开文件对话框。
                openFile.Multiselect = false;
                openFile.Filter = ("Excel 文件(*.xlsx)|*.xlsx");//后缀名。  
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    string filename = openFile.FileName;
                    string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", filename);
                     OleDbConnection  con = new OleDbConnection(strConn);
                     OleDbDataAdapter Excel_da = new OleDbDataAdapter("select * from [Sheet1$]", con);
                     DataTable  Excel_dt = new System.Data.DataTable();
                     OleDbCommandBuilder cb = new OleDbCommandBuilder(Excel_da);
                     Excel_da.Fill(Excel_dt);
                     DataTable dt = helper.getDs("select * from MedicineInfo", "MedicineInfo").Tables[0];
                     for (int i = 1; i < Excel_dt.Rows.Count;i++ )
                     {
                        
                         if (IsIP(Excel_dt.Rows[i][3].ToString())&&IsIntger(Excel_dt.Rows[i][4].ToString()))
                         {
                             DataRow dr = dt.NewRow();
                             dr[0] = Excel_dt.Rows[i][1].ToString();
                             dr[1] = Excel_dt.Rows[i][2].ToString();
                             dr[2] = Excel_dt.Rows[i][3].ToString();
                             dr[3] = Excel_dt.Rows[i][4].ToString();
                             dt.Rows.Add(dr);
                         } 
                         else
                         {
                           continue;
                         }
                        
                   
                             
                     }

                   OleDbDataAdapter da= helper.adapter("select * from MedicineInfo");
                   da.Update(dt);
                   
                   con.Close();
                   MessageBox.Show("导入成功,如果没有数据，请检查IP是否符合格式");
                   ListViewShow();
                   frmMain.fm.ListViewShow();
                  


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                helper.con_close();
            }  
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string savepath = sfd.FileName;
                ExcelHelper.UWriteListViewToExcel(listView1, savepath,"设置");
            }
            else
            {
            }
        }

        private void frmSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MessageBox.Show("软件即将重启", "系统提示", MessageBoxButtons.OKCancel) == DialogResult.OK)

            {
                Application.Restart();

            }
          
        }
    }
}
