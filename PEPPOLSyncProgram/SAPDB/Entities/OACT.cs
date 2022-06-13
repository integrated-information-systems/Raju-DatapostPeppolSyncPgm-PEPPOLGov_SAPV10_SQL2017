using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("OACT")]
    public class OACT
    {
        [Key]
        public string AcctCode { get; set; }
        public string U_peppol { get; set; }
    }
}
