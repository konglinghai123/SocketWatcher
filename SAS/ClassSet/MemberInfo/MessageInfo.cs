using System;
using System.Collections.Generic;
using System.Text;

namespace SAS.ClassSet.MemberInfo
{
    class MessageInfo
    {
        private string allmessage;

        public string Allmessage
        {
            get { return allmessage; }
            set { allmessage = value; }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private string time;

        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        public MessageInfo(string msg, string address, string type, string time)
        {
            this.allmessage = msg;
            this.address = address;
            this.type = type;
            this.time = time;
        }
        public MessageInfo()
        {

        }
    }
}