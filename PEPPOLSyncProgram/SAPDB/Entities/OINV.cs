using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("OINV")]
    public class OINV
    {
        public OINV()
        {
            this.InvoiceLines = new HashSet<INV1>();
           
        }
        [Key]
        public Int32 DocEntry { get; set; }
        public Int32 DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public string U_peppolsubmit { get; set; }
        public string U_peppolstatus { get; set; }
        public string U_peppolref { get; set; }
        public DateTime DocDueDate { get; set; }
        public string DocType { get; set; }
        public string DocCur { get; set; }
        public string NumAtCard { get; set; }        
        public decimal DocTotal { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CANCELED { get; set; }
        public string DocStatus { get; set; }
        public short GroupNum { get; set; }

        public string u_bu { get; set; }

        public string u_inv { get; set; }

        public string Comments { get; set; }

        public Int32 cntctcode { get; set; }


        public virtual ICollection<INV1> InvoiceLines { get; set; }
        public virtual OCRD Customer { get; set; }

        public virtual OCTG PaymentTerms { get; set; }
    }
}
