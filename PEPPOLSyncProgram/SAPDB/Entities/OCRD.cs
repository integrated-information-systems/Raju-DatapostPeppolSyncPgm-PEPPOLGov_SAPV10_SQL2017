using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("OCRD")]
    public class OCRD
    {
        public OCRD()
        {
            this.ARInvoices = new HashSet<OINV>();
            this.ARMemos = new HashSet<ORIN>();
            //this.APInvoices = new HashSet<OPCH>();
            //this.APMemos = new HashSet<ORPC>();
            //this.Transactions = new HashSet<OINM>();
            //this.CustomerAddress = new HashSet<CRD1>();
        }
        [Key]
        [Display(Name = "Customer Code")]
        public string CardCode { get; set; }
        [Display(Name = "Customer Name")]
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Block { get; set; }
        public string Country { get; set; }
        public string e_mail { get; set; }
        public string ZipCode { get; set; }
        public string U_custpeppolid { get; set; }
        public string LicTradNum { get; set; }

        public virtual ICollection<OINV> ARInvoices { get; set; }
        public virtual ICollection<ORIN> ARMemos { get; set; }
    }
}
