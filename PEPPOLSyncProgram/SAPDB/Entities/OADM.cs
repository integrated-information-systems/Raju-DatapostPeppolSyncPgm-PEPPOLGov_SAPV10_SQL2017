using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("OADM")]
    public class OADM
    {
        
        [Key]
        public string CompnyName { get; set; }

        public string AliasName { get; set; }
        public string E_Mail { get; set; }
        
    }
}
