using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PEPPOLSyncProgram.XMLSerialization
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader", IsNullable = false)]
    public class StandardBusinessDocumentHeader
    {

    }
    [Serializable]
    [XmlRoot(Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]    
    public class StandardBusinessDocument
    {
        public StandardBusinessDocumentHeader StandardBusinessDocumentHeader { get; set; }
        
        [XmlElement(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")]
        public Invoice Invoice { get; set; }
       [XmlElement(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2")]
        public CreditNote CreditNote { get; set; }
    }
    
}
