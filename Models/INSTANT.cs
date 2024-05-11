using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDAPI.Models
{
    public class INSTANT
    {
        public string prdtcode { get; set; }
        public string prdtmpqy { get; set; }
        public string prdtmlqy { get; set; }
        public string prdtmiqy { get; set; }

        public INSTANT(string prdtcode, string prdtmpqy, string prdtmlqy, string prdtmiqy)
        {
            this.prdtcode = prdtcode;
            this.prdtmpqy = prdtmpqy;
            this.prdtmlqy = prdtmlqy;
            this.prdtmiqy = prdtmiqy;
        }
    }
}
