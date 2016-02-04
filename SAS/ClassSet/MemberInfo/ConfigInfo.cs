using System;
using System.Collections.Generic;

using System.Text;

namespace SAS.ClassSet.MemberInfo
{
    class ConfigInfo
    {   //开始设防
        private string beginset;

        public string Beginset
        {
            get { return beginset; }
            set { beginset = value; }
        }
        //授权电话
        private string authorization;
        
        public string Authorization
        {
            get { return authorization; }
            set { authorization = value; }
        }
        //报警电话
        private string alarmtel;
        
        public string Alarmtel
        {
            get { return alarmtel; }
            set { alarmtel = value; }
        }
        private string address;
        //地址
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        private string effective;
        //有效值
        public string Effective
        {
            get { return effective; }
            set { effective = value; }
        }
        private string temperature;
        //温度
        public string Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }
        private string delay;
        //延时
        public string Delay
        {
            get { return delay; }
            set { delay = value; }
        }
        //响铃
        private string alaremcount;
        public string Alaremcount
        {
             get { return alaremcount; }
              set { alaremcount = value; }
        }
        private string senddelay;
        //发送延时
        public string Senddelay
        {
            get { return senddelay; }
            set { senddelay = value; }
        }
        private string[] configname;
        //设置名称
        public string[] Configname
        {
            get { return configname; }
            set { configname = value; }
        }
        public ConfigInfo()
        {

        }


    }
}
