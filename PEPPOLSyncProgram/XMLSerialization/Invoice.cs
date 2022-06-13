using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PEPPOLSyncProgram.XMLSerialization
{
    public class ID
    {
        [XmlAttribute(AttributeName = "schemeID")]
        public string schemeID { get; set; }
        [XmlText]
        public string Value { get; set; }
        [XmlElement(Namespace = CreditNote.cbcNameSpace)]
        public string Percent { get; set; }
    }
    public class PartyIdentification
    {

        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }
    }
    public class EndpointID
    {
        [XmlAttribute(AttributeName = "schemeID")]
        public string schemeID { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
    public class PartyName
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public string Name { get; set; }
    }
    public class PostalAddress
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public StreetName StreetName { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public AdditionalStreetName AdditionalStreetName { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public CityName CityName { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public PostalZone PostalZone { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public CountrySubentity CountrySubentity { get; set; }
        public Country Country { get; set; }
        
    }
    public class StreetName
    {
        [XmlText]
        public string Value { get; set; }
    }
    
    public class Country
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public string IdentificationCode { get; set; }
    }
    public class PartyLegalEntity
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public RegistrationName RegistrationName { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public CompanyID CompanyID { get; set; }
    }
    public class RegistrationName
    {
        [XmlText]
        public string Value { get; set; }
    }
    public class CompanyID
    {
        [XmlText]
        public string Value { get; set; }
    }
    public class PartyTaxScheme
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public CompanyID CompanyID { get; set; }
        public TaxScheme TaxScheme { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }
    }
    public class Contact
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public string Name { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public string ElectronicMail { get; set; }
    }

    public class Party
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public EndpointID EndpointID { get; set; }
        [XmlElement(Namespace = Invoice.cacNameSpace)]
        public PartyName PartyName { get; set; }
        public PartyIdentification PartyIdentification { get; set; }
        public PostalAddress PostalAddress { get; set; }
        public PartyTaxScheme PartyTaxScheme { get; set; }
        public PartyLegalEntity PartyLegalEntity { get; set; }
        
        public Contact Contact { get; set; }
    }
    public class AccountingSupplierParty
    {
        public Party Party { get; set; }
    }
    public class AccountingCustomerParty
    {
        public Party Party { get; set; }
    }
    public class PaymentTerms
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public Note Note { get; set; }
    }
    public class Note
    {
        [XmlText]
        public string Value { get; set; }
    }
    public class TaxAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class TaxScheme
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }
        
    }
    public class TaxCategory
    {
        public TaxAmount TaxAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public decimal Percent { get; set; }
        public TaxScheme TaxScheme { get; set; }
    }
    public class TaxSubtotal
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public TaxableAmount TaxableAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public TaxAmount TaxAmount { get; set; }
        public TaxCategory TaxCategory { get; set; }
    }
    public class TaxableAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class TaxTotal
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public TaxAmount TaxAmount { get; set; }
        [XmlElement(Namespace = Invoice.cacNameSpace)]
        public TaxSubtotal[] TaxSubtotal { get; set; }
    }
    #region LegalMonetaryTotal
    public class LineExtensionAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class TaxExclusiveAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class TaxInclusiveAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class AllowanceTotalAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class ChargeTotalAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class PayableAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }

    }
    public class LegalMonetaryTotal
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public LineExtensionAmount LineExtensionAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public TaxExclusiveAmount TaxExclusiveAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public TaxInclusiveAmount TaxInclusiveAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public AllowanceTotalAmount AllowanceTotalAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ChargeTotalAmount ChargeTotalAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public PayableAmount PayableAmount { get; set; }
    }
    #endregion
    #region InvoiceLine
    public class BaseQuantity
    {
        [XmlAttribute(AttributeName = "unitCode")]
        public string unitCode { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class PriceAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string currencyID { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class Price
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public PriceAmount PriceAmount { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public BaseQuantity BaseQuantity { get; set; }
    }
    public class ClassifiedTaxCategory
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public decimal Percent { get; set; }
        public TaxScheme TaxScheme { get; set; }
    }
    public class BuyersItemIdentification
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }
    }
    public class SellersItemIdentification
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }

    }
    public class Item
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public string Name { get; set; }
        [XmlElement(Namespace = Invoice.cacNameSpace)]
        public BuyersItemIdentification BuyersItemIdentification { get; set; }
        [XmlElement(Namespace = Invoice.cacNameSpace)]
        public SellersItemIdentification SellersItemIdentification { get; set; }

        public ClassifiedTaxCategory ClassifiedTaxCategory { get; set; }
    }
    public class InvoicedQuantity
    {
        [XmlAttribute(AttributeName = "unitCode")]
        public string unitCode { get; set; }
        [XmlText]
        public decimal Value { get; set; }
    }
    public class InvoiceLine
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public InvoicedQuantity InvoicedQuantity { get; set; }
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public LineExtensionAmount LineExtensionAmount { get; set; }
        public Item Item { get; set; }
        public Price Price { get; set; }
    }
    #endregion
    public class OrderReference
    {
        [XmlElement(Namespace = Invoice.cbcNameSpace)]
        public ID ID { get; set; }

    }
    [Serializable]
    [XmlRoot(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")]
    public class Invoice
    {

        //public const string xmlns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
        public const string cacNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
        public const string cbcNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        public const string cctsNameSpace = "urn:un:unece:uncefact:documentation:2";
        public const string extNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
        public const string qdtNameSpace = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2";
        public const string udtNameSpace = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2";
        public const string xsdNameSpace = "http://www.w3.org/2001/XMLSchema";
        public const string xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";

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
        public string InvoiceTypeCode { get; set; }
       

        [XmlElement(Namespace = cbcNameSpace)]
        public Note Note { get; set; }

        [XmlElement(Namespace = cbcNameSpace)]
        public string DocumentCurrencyCode { get; set; }

        [XmlElement(Namespace = cbcNameSpace)]
        public string BuyerReference { get; set; }

        [XmlElement(Namespace = cacNameSpace)]
        public OrderReference OrderReference { get; set; }

        [XmlElement(Namespace = cacNameSpace)]
        public AccountingSupplierParty AccountingSupplierParty { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public AccountingCustomerParty AccountingCustomerParty { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public PaymentTerms PaymentTerms { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public TaxTotal TaxTotal { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public LegalMonetaryTotal LegalMonetaryTotal { get; set; }
        [XmlElement(Namespace = cacNameSpace)]
        public InvoiceLine[] InvoiceLine { get; set; }

    }
}
