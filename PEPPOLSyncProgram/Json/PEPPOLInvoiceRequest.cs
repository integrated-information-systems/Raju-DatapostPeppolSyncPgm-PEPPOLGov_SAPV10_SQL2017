using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.Json
{
    public class header
    {
        public string sender { get; set; }
        public string receiver { get; set; }
    }
    public class body
    {
        public string peppolMessage { get; set; }        
    }
    public class PEPPOLInvoiceRequest
    {
        public header header { get; set; }
        public body body { get; set; }
    }
}
