using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.CustomDB
{
    public class ReceivedResponse
    {
        public HttpResponseMessage Response { get; set; }
        //public IRestResponse restResponse { get; set; }
        public string ResponseContent { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
