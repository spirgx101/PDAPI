using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDAPI.Models
{
    public class PRODBOX
    {
        public string goo_no { get; set; }
        public string plu_no { get; set; }
        public string itf { get; set; }
        public string cs_qty { get; set; }
        public string remark1 { get; set; }
        public string remark2 { get; set; }

        public PRODBOX(string goo_no, string plu_no, string itf, string cs_qty, string remark1, string remark2)
        {
            this.goo_no = goo_no;
            this.plu_no = plu_no;
            this.itf = itf;
            this.cs_qty = cs_qty;
            this.remark1 = remark1;
            this.remark2 = remark2;
        }
    }
}
