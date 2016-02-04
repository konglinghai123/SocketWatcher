using System;
using System.Collections.Generic;

using System.Text;

namespace SAS.ClassSet.MemberInfo
{
    class ChangeDataInfo
    {   //设防状态
        private string configstatus;

        public string Configstatus
        {
            get { return configstatus; }
            set { configstatus = value; }
        }
        //工作状态
        private string workstatus;

        public string Workstatus
        {
            get { return workstatus; }
            set { workstatus = value; }
        }
        //温度状态
        private string temperature;

        public string Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }
        public ChangeDataInfo()
        {

        }


    }
}
