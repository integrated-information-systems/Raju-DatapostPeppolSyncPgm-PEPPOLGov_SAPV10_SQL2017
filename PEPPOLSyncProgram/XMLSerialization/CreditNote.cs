using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace PEPPOLSyncProgram.XMLSerialization
{
   
    public class CreditNoteLine
    {
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public ID ID { get; set; }
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public CreditedQuantity CreditedQuantity { get; set; }
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public InvoicedQuantity InvoicedQuantity { get; set; }
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public LineExtensionAmount LineExtensionAmount { get; set; }
        public OrderLineReference OrderLineReference { get; set; }
        public Item Item { get; set; }
        public Price Price { get; set; }
    }
    public class CreditedQuantity
    {
        [XmlAttribute(AttributeName = "unitCode")]
        public string unitCode { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class OrderLineReference
    {
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public string LineID { get; set; }
    }
    [Serializable]
    [XmlRoot(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2")]
    public class CreditNote
    {
        public const string cacNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
        public const string cbcNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        // public const string xmlns = "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2";

        //public const string cctsNameSpace = "urn:un:unece:uncefact:documentation:2";
        //  public const string extNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
        //  public const string qdtNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2";
        // public const string udtNameSpace = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";
        //public const string xsdNameSpace = "http://www.w3.org/2001/XMLSchema";
        // public const string xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
        

        [XmlElement(Namespace = cbcNameSpace)]
        public string UBLVersionID { get; set; } = "2.1";
        [XmlElement(Namespace = cbcNameSpace)]
        public string CustomizationID { get; set; } = "urn:cen.eu:en16931:2017#conformant#urn:fdc:peppol.eu:2017:poacc:billing:international:sg:3.0";
        [XmlElement(Namespace = cbcNameSpace)]
        public string ProfileID { get; set; } = "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0";
        [XmlElement(Namespace = cbcNameSpace)]
        public ID ID { get; set; }
        [XmlElement(Namespace = cbcNameSpace)]
        public string IssueDate { get; set; }

        [XmlElement(Namespace = cbcNameSpace)]
        public string DueDate { get; set; }
        [XmlElement(Namespace = cbcNameSpace)]
        public string CreditNoteTypeCode { get; set; }

        [XmlElement(Namespace = cbcNameSpace)]
        public Note Note { get; set; }

        [XmlElement(Namespace = cbcNameSpace)]
        public string DocumentCurrencyCode { get; set; }
        [XmlElement(Namespace = cbcNameSpace)]
        public string BuyerReference { get; set; }            

        [XmlElement(Namespace = cacNameSpace)]
        public BillingReference BillingReference { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public AccountingSupplierParty AccountingSupplierParty { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public AccountingCustomerParty AccountingCustomerParty { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public AllowanceCharge AllowanceCharge { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public TaxTotal TaxTotal { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public LegalMonetaryTotal LegalMonetaryTotal { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public CreditNoteLine[] CreditNoteLine { get; set; }
    }
    public class BillingReference
    {
        public InvoiceDocumentReference InvoiceDocumentReference { get; set; }
    }
    public class InvoiceDocumentReference
    {
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public ID ID { get; set; }
    }
    public class AdditionalStreetName
    {
        [XmlText]
        public string Value { get; set; }
    }
    public class CityName
    {
        [XmlText]
        public string Value { get; set; }
    }
    public class PostalZone
    {
        [XmlText]
        public string Value { get; set; }
    }
    public class CountrySubentity
    {
        [XmlText]
        public string Value { get; set; }
    }
    public class AllowanceCharge
    {
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public Boolean ChargeIndicator { get; set; }
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public string AllowanceChargeReasonCode { get; set; }
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public string AllowanceChargeReason { get; set; }
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public Amount Amount { get; set; }
        public TaxCategory TaxCategory { get; set; }
    }
    public class Amount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public string Value { get; set; }
    }

}
