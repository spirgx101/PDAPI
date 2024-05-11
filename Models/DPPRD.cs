namespace PDAPI.Models
{
    public class DPPRD
    {
        public string vcitocod { get; set; }
        public string prdtcode { get; set; }
        public string plu_no { get; set; }
        public string dockcode { get; set; }
        public string vcitqty { get; set; }
        public string prdtslpr { get; set; }
        public string prdtcisd { get; set; }
        public string batno { get; set; }
        public string indc { get; set; }
        public string vcittype { get; set; }
        public string onpack { get; set; }
        public string freshfood { get; set; }

        public DPPRD(string vcitocod, string prdtcode, string plu_no, string dockcode, string vcitqty, string prdtslpr,
                    string prdtcisd, string batno, string indc, string vcittype, string onpack, string freshfood)
        {
            this.vcitocod = vcitocod;
            this.prdtcode = prdtcode;
            this.plu_no = plu_no;
            this.dockcode = dockcode;
            this.vcitqty = vcitqty;
            this.prdtslpr = prdtslpr;
            this.prdtcisd = prdtcisd;
            this.batno = batno;
            this.indc = indc;
            this.vcittype = vcittype;
            this.onpack = onpack;
            this.freshfood = freshfood;
        }
    }
}
