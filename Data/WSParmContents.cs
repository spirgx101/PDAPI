using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDAPI.Data
{
    public class WSParmContents
    {
        public string str_no { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public string mac { get; set; }
        public string fcode { get; set; }
        public string last_date { get; set; }
        public string file_name { get; set; }
        public string gotno { get; set; }
        public string xsc { get; set; }
        public string xsc_data { get; set; }
        public string trans_type { get; set; } //2021.09.08 mdho 新增傳輸類別：HT、PDA
    }
}
