using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.Json
{
    public class PEPPOLInvoiceReceipt
    {
       // public string id { get; set; }
       // public header header { get; set; }
      //  public string status { get; set; }
       // public body body { get; set; }
        public info[] info { get; set; }
        public int total { get; set; }
        public int lastIndex { get; set; }
    }
    public class info
    {
        public string documentNo { get; set; }
        public string sender { get; set; }
    }
}
