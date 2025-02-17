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
        private static string _AnalisisType = "";
        private static string _SwSegmentadoUbi = "";

        public static string NameConnect
        {
            get { return _nameconnect; }
            set { _nameconnect = value; }
        }

        public static string AnalisisType
        {
            get { return _AnalisisType; }
            set { _AnalisisType = value; }
        }

        public static string SwSegmentadoUbi
        {
            get { return _SwSegmentadoUbi; }
            set { _SwSegmentadoUbi = value; }
        }
    }
}
