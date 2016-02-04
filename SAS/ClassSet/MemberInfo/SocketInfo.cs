using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SAS.ClassSet.MemberInfo
{
    class SocketInfo
    {
        public Socket Socket { set; get; }
        public bool Isconnect { set; get; }
        public string Ip { set; get; }
        public string Port { set; get; }
        public Byte[] Bufffer { set; get; }

        public string Command { set; get; }
        public SocketInfo()
        {
            this.Bufffer = new Byte[1024];
            this.Ip = "";
            this.Port = "";
            this.Socket = null;
            this.Isconnect = false;
            this.Command = "";

        }
             
    }
}
