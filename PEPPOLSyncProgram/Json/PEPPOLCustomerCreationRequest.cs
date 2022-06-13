using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.Json
{
    public class authorizedPersonnel
    {
        
        public string name { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
    }
    public class PEPPOLCustomerCreationRequest
    {
        public string uen { get; set; }
        public string companyName { get; set; }
        public string gstRegistrationNumber { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string postalCode { get; set; }
        public authorizedPersonnel authorizedPersonnel { get; set; }

        public string serviceCapability { get; set; } = "send only";
    }
}
