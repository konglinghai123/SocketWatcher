using System;
using System.Collections.Generic;

using System.Text;

namespace SAS.ClassSet.MemberInfo
{   //查询信息的结构体封装
    class SelectionInfo
    {   //温度
        private string temperature;

        public string Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }
        //温度报警
        private string temperaturealarm;

        public string Temperaturealarm
        {
            get { return temperaturealarm; }
            set { temperaturealarm = value; }
        }
        //外烟
        private string leaves;

        public string Leaves
        {
            get { return leaves; }
            set { leaves = value; }
        }
        //烟感
        private string smoke;

        public string Smoke
        {
            get { return smoke; }
            set { smoke = value; }
        }
        //射频
        private string rf;

        public string Rf
        {
            get { return rf; }
            set { rf = value; }
        }
        //门禁
        private string dooralarm;

        public string Dooralarm
        {
            get { return dooralarm; }
            set { dooralarm = value; }
        }
        //市电
        private string electric;

        public string Electric
        {
            get { return electric; }
            set { electric = value; }
        }
        //门铃延时
        private string alarmdalay;

        public string Alarmdalay
        {
            get { return alarmdalay; }
            set { alarmdalay = value; }
        }
        //发送延时
        private string alaremtime;

        public string Alaremtime
        {
            get { return alaremtime; }
            set { alaremtime = value; }
        }
        //设备状态
        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        //有效值
        private string effective;

        public string Effective
        {
            get { return effective; }
            set { effective = value; }
        }
        private string video;

        public string Video
        {
            get { return video; }
            set { video = value; }
        }
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public SelectionInfo()
        {

        }
        
    }
}
