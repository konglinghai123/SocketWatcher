using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SqlClient;
using SAS.ClassSet.MemberInfo;
namespace SAS.ClassSet.FunctionTools
{
    class SqlHelper
    {
        public OleDbConnection conn;
        DataSet tableset;
      public  OleDbDataAdapter oledbda;
       
        static string strDatabasePath
        {
            get
            {
                return Common.Common.strDatabasePath;
            }
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string connString
        {
            get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDatabasePath; }
        }
       
        #region  建立数据库连接
        /// <summary>
        /// 建立数据库连接.
        /// </summary>
        /// <returns>返回SqlConnection对象</returns>
        public  OleDbConnection getcon()
        {
            conn = new OleDbConnection(connString);
            if (conn.State != ConnectionState.Open)
            {
               
                conn.Open();  //打开数据库连接
                return conn;  //返回SqlConnection对象的信息
            }
            else
            {
                return conn;  //返回SqlConnection对象的信息
            }
           
        }
        #endregion
        #region  关闭数据库连接
        /// <summary>
        /// 关闭于数据库的连接.
        /// </summary>
        public void con_close()
        {
            if (conn.State == ConnectionState.Open)   //判断是否打开与数据库的连接
            {
                conn.Close();   //关闭数据库的连接
                conn.Dispose();   //释放My_con变量的所有空间
            }
        }
        #endregion
     
       
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="tablename">表名</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int delete(string tablename,string condition)
        {
            string deletecommand = "delete " +" * "+ " from"+" "+ tablename +" "+ condition;
           return Oledbcommand(deletecommand);
        }
       
        #region 选择记录
        public OleDbDataReader select(string table,string condition)
        {   
            string selectcommand = "select"+"*"+ "from" + table + condition;
            return getDataReader(selectcommand);
        }
        #endregion

        #region 分页显示
        public int totalpage(string sqlcommand,int pagesize,string tablename)//返回总的页数
        {
            DataTable dt = getDs(sqlcommand,tablename).Tables[0];
            int Linenumber = dt.Rows.Count;
            if ((Linenumber % pagesize) > 0)
            {
                int allpage = (Linenumber / pagesize) + 1;
                return allpage;
            }
            else
            {
                int allpage = Linenumber / pagesize;
                return allpage;
            }
           
        }
        private  DataTable datapageview(DataTable dt,int currentpage,int pagesize)//返回指定页数的记录
        {
            if (currentpage == 0)
                return dt;
            DataTable newdt = dt.Clone();
            //newdt.Clear();
            int rowbegin = (currentpage - 1) * pagesize;
            int rowend = currentpage * pagesize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }

            return newdt;
        }
        public DataTable ListviewShow(string selectcommand,int currentpage,int pagesize,string tablename)//Listview显示的内容
        {
            DataTable dt = getDs(selectcommand, tablename).Tables[0];
            DataTable Target = datapageview(dt,currentpage,pagesize);
            return Target;
        }
        #endregion
        #region 执行SQL语句返回DataReader
        public OleDbDataReader getDataReader(string commandtext)
        {

            OleDbCommand cmd = new OleDbCommand(commandtext, conn);

            if (conn.State != ConnectionState.Open) conn.Open();
           OleDbDataReader DtReader = cmd.ExecuteReader();
           return DtReader;

           
            
        }
        #endregion
        #region 执行SQL语句返回受影响行数
        public int Oledbcommand(string command)
        {
            try
            {
                int SuccessFlag;
                conn = new OleDbConnection(connString);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(command, conn);
                    SuccessFlag = cmd.ExecuteNonQuery();
                    conn.Dispose();
                    return SuccessFlag;
                }
                else
                {
                    OleDbCommand cmd = new OleDbCommand(command, conn);
                    SuccessFlag = cmd.ExecuteNonQuery();
                    conn.Dispose();
                    return SuccessFlag;
                }
            }
            catch (OleDbException e)
            {
                MessageBox.Show(e.ToString());
                return 0;
            }
        }
        #endregion
        #region 返回dataset
        public DataSet getDs(string strCon, string tbname)
        {

            conn = getcon();
            oledbda = new OleDbDataAdapter(strCon, conn);
            OleDbCommandBuilder cb = new OleDbCommandBuilder(oledbda);
            tableset = new DataSet();
            oledbda.Fill(tableset, tbname);
            conn.Close();
            return tableset;


        }
        #endregion
        #region 合并表格
        public void mergetable(DataTable dt,string sqlcommand)
        {
            OleDbConnection conn1 = new OleDbConnection(connString);
           
            OleDbDataAdapter mergetadapter = new OleDbDataAdapter(sqlcommand,conn1);
            OleDbCommandBuilder cb = new OleDbCommandBuilder(mergetadapter);
            DataTable oldtable = new DataTable();
            mergetadapter.Fill(oldtable);
        
            oldtable.Merge(dt,true);
            mergetadapter.Update(dt);
        }
        #endregion
        public OleDbDataAdapter adapter(string strCon)
        {
            OleDbConnection conn1 = new OleDbConnection(connString);
            OleDbDataAdapter oledbda = new OleDbDataAdapter(strCon, conn1);
            OleDbCommandBuilder cb = new OleDbCommandBuilder(oledbda);
            return oledbda; 
        }
        public void insertToStockDataByBatch(List<string> sqlArray,ProgressBar pb)
        {
            try
            {
                OleDbConnection aConnection = new OleDbConnection(connString);
               aConnection.Open();
                OleDbTransaction transaction = aConnection.BeginTransaction();
              

                OleDbCommand aCommand = new OleDbCommand();
                aCommand.Connection = aConnection;
                aCommand.Transaction = transaction;
                pb.Maximum = sqlArray.Count;
                pb.Step = 1;
                for (int i = 0; i < sqlArray.Count; i++)
                {
                    pb.PerformStep();
                    aCommand.CommandText = sqlArray[i].ToString();
                    aCommand.ExecuteNonQuery();
                    
                }
             
                transaction.Commit();
                aConnection.Close();
              
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
               
            }
        }
        public static object ExcuteScalar(string SQl, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SQl;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();//返回一行object
                }
            }
        }
        public static DataTable ExecuteDataTable(string SQl, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SQl;
                    cmd.Parameters.AddRange(parameters);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataSet dataset = new DataSet();
                    adapter.Fill(dataset);
                    return dataset.Tables[0];
                }
            }
        }//用来执行查询结果较少的sql
        public static int ExcuteNonQuery(string SQL, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();//返回值为影响的数据行数
                }
            }
        }

    }
    
}

        