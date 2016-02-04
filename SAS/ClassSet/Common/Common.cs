using System;
using System.Collections.Generic;

using System.Text;


using System.IO;

namespace SAS.ClassSet.Common
{
    class Common
    {
             /// <summary>
        /// 数据库名称
        /// </summary>
        const string strDatabaseName = "DataBase.mdb";
        public static string TempPath = System.IO.Path.GetTempPath() + "Setting";
        static string MyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        /// <summary>
        /// 数据库路径
        /// </summary>
        public static string strDatabasePath
        {
            get
            {
                return MyDocumentsPath + "\\ContorlSetting\\" + strDatabaseName;
            }
        }

        public static string strEmailResultPath
        {
            get
            {
                return MyDocumentsPath + "\\ContorlSetting\\EmailResult\\";
            }
        }
        public static string strAddfilesPath
        {
            get
            {
                return MyDocumentsPath + "\\ContorlSetting\\Addfiles\\";
            }
        }
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string ConfPath
        {
            get
            {
                return MyDocumentsPath + "\\ContorlSetting\\UserData.xml";
            }
        }





      
      
        #region 输出数据库   
         public static bool load()
        {
            //---------------------------------输出数据库---------------------------

            if (!(File.Exists(strDatabasePath)))
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ContorlSetting\\";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                FileStream fs = new FileStream(strDatabasePath, FileMode.OpenOrCreate, FileAccess.Write);

                try
                {
                    Byte[] b = SAS.Properties.Resources.DataBase;

                    fs.Write(b, 0, b.Length);
                    if (fs != null)
                        fs.Close();
                }
                catch
                {
                    if (fs != null)
                        fs.Close();
                    return false;
                }
            }
            // -----------------------------第一次加载代码，------------------------------
            //if (conn.State != ConnectionState.Open)
            //    conn.Open();

            ////-----------------------------操作完成后记得关闭连接------------------------------
            //conn.Close();
            return true;
        }
        #endregion
      
    }
 }

