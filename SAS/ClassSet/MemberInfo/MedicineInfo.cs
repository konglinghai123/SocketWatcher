using System;
using System.Collections.Generic;
using System.Text;

namespace SAS.ClassSet.MemberInfo
{
    class MedicineInfo
    {
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
        private string ip;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
        public MedicineInfo(string address,string type ,string ip,string port)
        {
            this.address = address;
            this.type = type;
            this.ip = ip;
            this.port = port;

        }
        private string port;

        public string Port
        {
            get { return port; }
            set { port = value; }
        }
        public MedicineInfo()
        {

        }
    }
}
