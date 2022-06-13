using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("ODRF")]
    public class ODRF
    {
        [Key]
        public string U_peppolref { get; set; }

        public string ObjType { get; set; }
    }
}
