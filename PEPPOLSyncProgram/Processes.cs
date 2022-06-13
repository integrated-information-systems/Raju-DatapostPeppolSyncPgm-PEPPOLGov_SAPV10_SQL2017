using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram
{
    public enum Processes
    {
        SendInvoices = 0,
        ReceiveInvoice = 1,
        RegisterCustomer = 2,
        RegisteredCustomerConfirmation = 3,
        SendCreditNote = 4,
        ReceiveCreditNote=5
    }
}
