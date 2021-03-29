using System;
using System.Collections.Generic;
using System.Text;

namespace Pro_schedule
{
    public class Constants
    {

        public string conStringAuth = @"Data Source=DESKTOP-CP86CV1;Initial Catalog=DLI_IT;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string conStringList = @"Data Source=DESKTOP-CP86CV1;Initial Catalog=DLIMARK;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        //public string conStringAuth = @"Data Source=SQL.dli.local;Initial Catalog=DLI_IT;User ID=sa;Password=MZ1+4;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //public string conStringList = @"Data Source=SQL.dli.local;Initial Catalog=DLIMARK;User ID=sa;Password=MZ1+4;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public bool logined         = false;
        public string empID         = "";
        public string empName       = "";
        public int proLevel         = 0;
        public int session          = 0;
        public string logError      = "";
        public string WO_ID         = "";

        public string curTime       = "";
    }
}
