using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDAPI.Models
{
    public class ODPPRD
    {
        public string vcitocod { get; set; }
        public string prdtcode { get; set; }
        public string plu_no { get; set; }
        public string space { get; set; }
        public string indc1 { get; set; }
        public string indc2 { get; set; }
        public string sup_no { get; set; }
        public string vcitqty { get; set; }
        public string prdtslpr { get; set; }
        public string prdtcisd { get; set; }
        public string prdtmlqy { get; set; }
        public string prdtmiqy { get; set; }

        public ODPPRD(string vcitocod, string prdtcode, string plu_no, string space, string indc1, string indc2,
                    string sup_no, string vcitqty, string prdtslpr, string prdtcisd, string prdtmlqy, string prdtmiqy)
        {
            this.vcitocod = vcitocod;
            this.prdtcode = prdtcode;
            this.plu_no = plu_no;
            this.space = space;
            this.indc1 = indc1;
            this.indc2 = indc2;
            this.sup_no = sup_no;
            this.vcitqty = vcitqty;
            this.prdtslpr = prdtslpr;
            this.prdtcisd = prdtcisd;
            this.prdtmlqy = prdtmlqy;
            this.prdtmiqy = prdtmiqy;
        }
    }
}
