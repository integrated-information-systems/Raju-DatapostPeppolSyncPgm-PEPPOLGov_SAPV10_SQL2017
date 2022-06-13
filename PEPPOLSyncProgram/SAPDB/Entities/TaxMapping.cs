using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("@Tax")]
    public class TaxMapping
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
