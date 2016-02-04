using System;
using System.Collections.Generic;

using System.Text;
using SAS.ClassSet.MemberInfo;
using System.Windows.Forms;
using System.IO;
namespace SAS.ClassSet.FunctionTools
{
    class HandleCommand
    {   
        /// <summary>
        /// 判断设防状态和工作状态的开关，异常
        /// </summary>
        /// <param name="stauts">设防状态/工作状态</param>
        /// <param name="info">存储结构体</param>
        /// <param name="No">开/正常</param>
        /// <param name="OFF">关/异常</param>
        private void IsOffOrNo(string stauts,SelectionInfo info,string No,string OFF)
        {
            for (int i = 0; i < stauts.Length;i++ )
            {
                if (stauts[i]=='0')
                {
                    switch(i){
                        case 0:
                            info.Smoke = info.Smoke+OFF;
                            break;
                        case 1:
                            info.Dooralarm = info.Dooralarm+OFF;
                            break;
                        case 2:
                            info.Video = info.Video+OFF;
                            break;
                        case 3:
                            info.Rf = info.Rf+OFF;
                            break;
                        case 4:
                            info.Electric = info.Electric+OFF;
                            break;
                        case 5:
                            info.Leaves = info.Leaves+OFF;
                            break;
                    }
                } 
                else
                {
                    switch (i)
                    {
                        case 0:
                            info.Smoke = info.Smoke + No;
                            break;
                        case 1:
                            info.Dooralarm = info.Dooralarm + No;
                            break;
                        case 2:
                            info.Video = info.Video + No;
                            break;
                        case 3:
                            info.Rf = info.Rf + No;
                            break;
                        case 4:
                            info.Electric = info.Electric + No;
                            break;
                        case 5:
                            info.Leaves = info.Leaves + No;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 16进制字符串转换Unicode    
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        public string Decode(string strDecode)
        {
            string sResult = "";
            for (int i = 0; i < strDecode.Length / 4; i++)
            {
                sResult += (char)short.Parse(strDecode.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber);
            }
            return sResult;
        }
        /// <summary>
        /// 查询字符串的处理函数
        /// </summary>
        /// <param name="command">收到的数据</param>
        /// <returns></returns>
        public SelectionInfo QueryHandle(string command)
        {   string stauts="";//设防状态
            string workstauts = "";//工作状态
            SelectionInfo info = new SelectionInfo();
            string[] OneSplit = command.Split(')');
            List<string> property = new List<string>();
            foreach(string a in OneSplit){
                property.Add(a);
            }
          foreach(string data in property){
              if (data.IndexOf("设防状态(")!=-1)
              {
                  stauts = data.Substring(data.IndexOf('(') + 1);
              }
              if (data.IndexOf("报警温度>(") != -1)
              {
                  info.Temperaturealarm = ">"+data.Substring(data.IndexOf('(') + 1);
              }
              if (data.IndexOf("延时(")!=-1)
              {
                  info.Alarmdalay = data.Substring(data.IndexOf('(') + 1);
              }
              if (data.IndexOf("地址(")!=-1)
              {
                  info.Address = Decode(data.Substring(data.IndexOf('(') + 1));
              }
              if (data.IndexOf("响声")!=-1)
              {
                  info.Alaremtime = data.Substring(data.IndexOf('(') + 1);
              }
              if (data.IndexOf("温度(")!=-1)
              {
                  info.Temperature = data.Substring(data.IndexOf('(') + 1);
              }
              if (data.IndexOf("常开")!=-1)
              {
                  info.Effective = "常开";
              }
              if (data.IndexOf("常闭") != -1)
                {
                      info.Effective = "常闭";
                }
                
              if (data.IndexOf("工作状态(")!=-1)
              {
                  workstauts = data.Substring(data.IndexOf('(') + 1);
              }
          }
           if (stauts[stauts.Length-1]=='0')
           {
               info.Temperaturealarm = "关";
           }
            if (workstauts=="111111")
            {
                info.Status = "正常";
            }else{
                info.Status="异常";
            }
            IsOffOrNo(stauts, info, "开", "关");
            IsOffOrNo(workstauts, info, "(正常)", "(异常)");
            return info;
        }

    
    }
}
