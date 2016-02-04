using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAS.ClassSet.MemberInfo
{
    class ClientInfo
    {
        private string ip;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

      
        private string port;

        public string Port
        {
            get { return port; }
            set { port = value; }
        }
      
        public ClientInfo()
        {

        }
        public ClientInfo(string ip,string port)
        {
            this.ip = ip;
            this.port = port;
        }
    }
}
