using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("OCTG")]
    public class OCTG
    {
        public OCTG(){
            this.ARInvoices = new HashSet<OINV>();
        }
        [Key]
        public short GroupNum { get; set; }
        public string PymntGroup { get; set; }
        public virtual ICollection<OINV> ARInvoices { get; set; }
    }
}
