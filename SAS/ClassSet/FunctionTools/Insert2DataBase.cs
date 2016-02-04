using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAS.ClassSet.MemberInfo;
using System.Data;
using System.Data.OleDb;
using SAS.Forms;
namespace SAS.ClassSet.FunctionTools
{
    class Insert2DataBase
    {
        SqlHelper helper = new SqlHelper();

        public void insert(MessageInfo info)
        {
            DataTable dt = helper.getDs("select * from Logs_Data", "Logs_Data").Tables[0];
            DataRow dr = dt.NewRow();
            dr[0] = frmMain.IpAndName[info.Address];
            dr[1] = info.Allmessage;
            dr[2] = DateTimeToStamp(info.Time);
            dr[3] = info.Type;
            dt.Rows.Add(dr);
            OleDbDataAdapter oledbadapter = helper.adapter("select * from Logs_Data");
            oledbadapter.Update(dt);
        }

        public static int DateTimeToStamp(string strtime)
        {
            DateTime time = Convert.ToDateTime(strtime);
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        // 时间戳转为C#格式时间
        public static string StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow).ToString();
        }
    }
}
