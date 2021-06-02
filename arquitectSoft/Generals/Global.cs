using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Generals
{
    static class Global
    {
        private static string _nameconnect = "";

        public static string NameConnect
        {
            get { return _nameconnect; }
            set { _nameconnect = value; }
        }
    }
}
