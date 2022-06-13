using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("INV1")]
    public class INV1
    {
        [Key]
        [Column(Order = 1)]
        public Int32 DocEntry { get; set; }
        public string ItemCode { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string unitMsr { get; set; }
        [Key]
        [Column(Order = 2)]
        public Int32 LineNum { get; set; }        
        public string SubCatNum { get; set; }
        public string Dscription { get; set; }
        public decimal LineTotal { get; set; }
        public decimal VatPrcnt { get; set; }        
        public string VatGroup { get; set; }
        public decimal VatSum { get; set; }
        public string Currency { get; set; }
        public virtual OINV InvoiceHeader { get; set; }
    }
}
