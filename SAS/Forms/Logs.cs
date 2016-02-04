using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAS.ClassSet.FunctionTools;

namespace SAS.Forms
{
    public partial class Logs : Form

    {


        private SqlHelper helper = new SqlHelper();
        private int StartTime = 0;
        private int EndTime = 0;
        private string SelectCommand = "";
        /// 设置Listviewd的属性
        /// </summary>
        private void SetListviewBorder()
        {
            ImageList il = new ImageList();

            //设置高度
            il.ImageSize = new Size(1, 20);
            //绑定listView控件
            
            //listView1.SmallImageList = il;
            listView1.SmallImageList = il;
        }
        private void InitCombox()
        {
            comboBox1.Items.Add("全部");
            comboBox2.Items.Add("全部");
            DataTable DtType = helper.getDs("select distinct Type from Logs_Data", "Logs_Data").Tables[0];
            DataTable DtName = helper.getDs("select distinct CName from Logs_Data", "Logs_Data").Tables[0];
            for (int i=0;i<DtType.Rows.Count;i++)
            {
                comboBox1.Items.Add(DtType.Rows[i][0].ToString());
            }
            for (int i = 0; i < DtName.Rows.Count; i++)
            {
                comboBox2.Items.Add(DtName.Rows[i][0].ToString());
            }
        }
        private void InitData()
        {
          
            CreatSelectCommand();
            DataTable dt= helper.getDs(SelectCommand, "Logs_Data").Tables[0];
            Datatable2ListView(dt);
        }

        public Logs()
        {
            
            InitializeComponent();
            SetListviewBorder();
            StartTime = Insert2DataBase.DateTimeToStamp(dateTimePicker1.Value.ToShortDateString());
            EndTime = Insert2DataBase.DateTimeToStamp(dateTimePicker2.Value.ToShortDateString());
            InitCombox();


        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem s in listView1.Items)
            {
                s.Checked = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
          

            if (MessageBox.Show("是否确认删除? 注意!删除后将无法恢复!", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }
            if (listView1.CheckedItems.Count > 0)
            {
                foreach (ListViewItem LVI in listView1.CheckedItems)
                {
                    string strCMD = "delete from Logs_Data where CTime = " + Insert2DataBase.DateTimeToStamp(LVI.SubItems[3].Text) + "";
                    helper.Oledbcommand(strCMD);
                }
            }

        //-------------------------------------------
            listView1.Items.Clear();
            InitData();
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            InitData();
            //ListViewShow(comboBox1.Text);
        }
        /// <summary>
        /// 将数据库当前的数据显示在listview
        /// </summary>
      

        private void button4_Click(object sender, EventArgs e)
        {
            InitData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string savepath = sfd.FileName;
                ExcelHelper.UWriteListViewToExcel(listView1, savepath, comboBox1.Text);
            }
            else
            {
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("KEY-  "+e.ToString());
            
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            Point curPos = this.listView1.PointToClient(Control.MousePosition);
            ListViewItem lvwItem = this.listView1.GetItemAt(curPos.X, curPos.Y);
            
            if (lvwItem != null)
            {
              
                lvwItem.Selected = true;
               /* if (lvwItem.Selected == true) { lvwItem.Selected = false; }
                else { lvwItem.Selected = true; }*/
               // if (e.X > 16) lvwItem.Checked = true;
                if (e.X > 16){ if(lvwItem.Checked == true){lvwItem.Checked = false;}
                                else{lvwItem.Checked = true;}
                    }

            }
            else { }
            

        }

   

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            StartTime = Insert2DataBase.DateTimeToStamp(dateTimePicker1.Value.ToShortDateString());
            InitData();
        }
        
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
           EndTime=Insert2DataBase.DateTimeToStamp(dateTimePicker2.Value.ToShortDateString());
            InitData();
        }
        private void Datatable2ListView(DataTable dt)
        {
            listView1.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] str = new string[]{
                    (i+1).ToString(),
                    dt.Rows[i][0].ToString(),
                      dt.Rows[i][3].ToString(),
                       Insert2DataBase.StampToDateTime(dt.Rows[i][2].ToString()),
                          dt.Rows[i][1].ToString()
                    };
                ListViewItem lit = new ListViewItem(str);
                listView1.Items.Add(lit);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            InitData();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitData();
        }
        private void CreatSelectCommand()
        {
            string name = "";
            string type = "";
            if (EndTime == StartTime)
            {
                EndTime = EndTime + 86400;

            }
            SelectCommand = string.Format("select * from Logs_Data where CTime>={0} and CTime<={1}", StartTime, EndTime);
            if (comboBox2.Text == "全部")
            {
                name = "";
            }
            else
            {
                name = string.Format("CName='{0}'", comboBox2.Text);
                SelectCommand = SelectCommand + " and " + name;
            }
            if (comboBox1.Text == "全部")
            {
                type = "";
            }
            else
            {
                type = string.Format("Type='{0}'", comboBox1.Text);
                SelectCommand = SelectCommand + " and " + type;
            }

        }
    }
}
