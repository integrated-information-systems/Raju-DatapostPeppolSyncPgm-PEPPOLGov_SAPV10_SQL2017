using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB.Entities
{
    [Table("ADM1")]
    
    public class ADM1
    {
        [Key]
        public int code { get; set; }
        public string Street { get; set; }
        public string Block { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        public string IntrntAdrs { get; set; }
    }
}
