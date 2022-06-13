using Newtonsoft.Json;
using PEPPOLSyncProgram.CustomDB;
using PEPPOLSyncProgram.Json;
using PEPPOLSyncProgram.SAPDB;
using PEPPOLSyncProgram.SAPDB.Entities;
using PEPPOLSyncProgram.XMLSerialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Collections.Specialized;

namespace PEPPOLSyncProgram
{

    public partial class Form1 : Form
    {
        public static Processes currentProcess;
        public Form1()
        {
            InitializeComponent();
        }
        private void GetSAPCompanyObject(ref SAPbobsCOM.Company SAPCompany)
        {
            try
            {
                SAPCompany.Server = ConfigurationManager.AppSettings["Company_Server"];
                SAPCompany.LicenseServer = ConfigurationManager.AppSettings["Company_Server"];

                SAPCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;

                SAPCompany.CompanyDB = ConfigurationManager.AppSettings["Company_DB"];
                SAPCompany.UserName = ConfigurationManager.AppSettings["Company_Username"];
                SAPCompany.Password = ConfigurationManager.AppSettings["Company_Password"];
                SAPCompany.DbUserName = ConfigurationManager.AppSettings["DB_Username"];
                SAPCompany.DbPassword = ConfigurationManager.AppSettings["DB_Password"];

                string MSSQLVersion = ConfigurationManager.AppSettings["MSSQLVersion"];               
                //SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017;

                switch (MSSQLVersion)
                {
                    case "SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005":
                        SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005;
                        break;
                    case "SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008":
                        SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
                        break;
                    case "SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012":
                        SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
                        break;
                    case "SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014":
                        SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
                        break;
                    case "SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016":
                        SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016;
                        break;
                    case  "SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017":
                        SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017;
                        break;
                    //case "SAPbobsCOM.BoDataServerTypes.dst_MSSQL2019":
                      //  SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2019;
                       // break;

                    default:
                        SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017;
                        break;
                }

                SAPCompany.UseTrusted = false;

                int SAPConnectionStatus = 0;
                string errMsg = string.Empty;
                int err = 0;

                if (SAPCompany.Connected == false)
                {
                    SAPConnectionStatus = SAPCompany.Connect();
                    if (SAPConnectionStatus != 0)
                    {

                        SAPCompany.GetLastError(out err, out errMsg);
                        SAPCompany = null;
                        throw new Exception(err + "-" + errMsg);
                    }
                }

            }
            catch (Exception ex)
            {
                AppSpecificFunc.WriteLog(ex);
            }
        }
        private void ProcessTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ProcessTimer.Enabled = false;
                if (currentProcess == Processes.ReceiveCreditNote)
                {

                    currentProcess = Processes.SendInvoices;
                }
                else
                {
                    currentProcess = currentProcess + 1;
                }

                SelectProcess();
                ProcessTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                AppSpecificFunc.WriteLog(ex);
            }

        }
        private void SelectProcess()
        {
            
            try
            {
                switch (currentProcess)
                {
                    case Processes.SendInvoices:
                        SendInvoice();
                        break;
                    case Processes.ReceiveInvoice:
                        ReceiveInvoice_Service();
                        break;

                   
                    case Processes.SendCreditNote:
                        SendCreditNote();
                        break;
                    case Processes.ReceiveCreditNote:
                        ReceiveCreditNote_Service();
                        break;
                }
            }
            catch (Exception ex)
            {
                AppSpecificFunc.WriteLog(ex);
            }


        }
            
        #region "Receive Ap Invoice Draft"
        private void ReceiveInvoice_Service()
        {
            try
            {

                string fromPEPPOLID = string.Empty;
                string toPEPPOLID = string.Empty;

                OADM companyObject = null;

                string DocNum = string.Empty;

                OACT GLCodeObject = null;
                using (var dbcontext = new EFSapDbContext())
                {
                    companyObject = dbcontext.CompanyDetails.FirstOrDefault();
                    GLCodeObject = dbcontext.GLCodes.Where(x => x.U_peppol.Equals("Y")).FirstOrDefault();

                }
                //toPEPPOLID = "0195:SGTSTDPSPTEST01";
                toPEPPOLID = companyObject.AliasName;

                string invoiceId = string.Empty;

                // -----Call API to get document nos for the peppolid-----

                int pageNum = 1;
                int totalRecords = 1;
                int lastIndex = -1;

                var objType = ConfigurationManager.AppSettings["AP_Invoice_ObjectType"];

                do
                {
                    var getDocNumApiCall = Task.Run(() => RetrivedDocNo(toPEPPOLID, "invoices", pageNum));
                    getDocNumApiCall.Wait();

                    ReceivedResponse getDocNumApiResponse = getDocNumApiCall.Result;
                    PEPPOLInvoiceReceipt pEPPOLInvoiceReceipt = Newtonsoft.Json.JsonConvert.DeserializeObject<PEPPOLInvoiceReceipt>(getDocNumApiResponse.ResponseContent);
                    totalRecords = pEPPOLInvoiceReceipt.total;
                    lastIndex = pEPPOLInvoiceReceipt.lastIndex;
                    pageNum = pageNum + 1;

                  
                    foreach (var docList in pEPPOLInvoiceReceipt.info)
                    {
                        DocNum = docList.documentNo.ToString();
                          
                        bool isDocExistAlready = AppSpecificFunc.IsDocNumAlreadyExist(DocNum, objType);
                        if (!isDocExistAlready)
                        {

                            var XmlFileCall = Task.Run(() => RetrivedXMLDoc(DocNum, "invoices"));
                            XmlFileCall.Wait();

                            ReceivedResponse XmlFileResponse = XmlFileCall.Result;

                            if (XmlFileResponse.StatusCode == HttpStatusCode.OK)
                            { 
                                XmlSerializer serializer = new XmlSerializer(typeof(StandardBusinessDocument));

                                StandardBusinessDocument receviedAPInvoice = null;
                                using (TextReader reader = new StringReader(XmlFileResponse.ResponseContent))
                                {
                                    receviedAPInvoice = (StandardBusinessDocument)serializer.Deserialize(reader);
                                }


                                if (receviedAPInvoice.Invoice != null)
                                {
                                    //invoiceId = aPInvoiceNotifications.primarysubjectid;
                                    invoiceId = DocNum;
                                    string responseContent = string.Empty;

                                    fromPEPPOLID = docList.sender.ToString();


                                    SAPbobsCOM.Company SAPCompany = new SAPbobsCOM.Company();
                                    GetSAPCompanyObject(ref SAPCompany);
                                    if (SAPCompany != null)
                                    {
                                        OCRD Supplier = null;
                                        using (var dbcontext = new EFSapDbContext())
                                        {
                                            Supplier = dbcontext.Customers.Where(x => x.U_custpeppolid.Equals(fromPEPPOLID) && x.CardType == "S").FirstOrDefault();
                                            //Supplier.CardCode = (Supplier.CardCode != null) ? Supplier.CardCode : "4000/I0023";
                                        }
                                        if (Supplier != null)
                                        {
                                            SAPbobsCOM.Documents aPInvoiceDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                            aPInvoiceDocument.CardCode = Supplier.CardCode;
                                            aPInvoiceDocument.CardName = receviedAPInvoice.Invoice.AccountingSupplierParty.Party.PartyLegalEntity.RegistrationName.Value;

                                            DateTime ValidDocDate = Convert.ToDateTime(receviedAPInvoice.Invoice.IssueDate);
                                            DateTime ValidDueDate = Convert.ToDateTime(receviedAPInvoice.Invoice.DueDate);
                                            aPInvoiceDocument.DocDate = ValidDocDate;
                                            aPInvoiceDocument.DocDueDate = ValidDueDate;
                                            aPInvoiceDocument.TaxDate = ValidDocDate;
                                            aPInvoiceDocument.UserFields.Fields.Item("U_peppolref").Value = invoiceId;
                                            aPInvoiceDocument.NumAtCard = receviedAPInvoice.Invoice.ID.Value;
                                            aPInvoiceDocument.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                                            aPInvoiceDocument.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseInvoices;



                                            List<TaxMapping> TaxMappingList = null;
                                            using (var dbcontext = new EFSapDbContext())
                                            {
                                                TaxMappingList = dbcontext.TaxMappings.ToList();
                                            }

                                            foreach (InvoiceLine Item in receviedAPInvoice.Invoice.InvoiceLine)
                                            {
                                                if (GLCodeObject != null)
                                                {
                                                    aPInvoiceDocument.Lines.AccountCode = GLCodeObject.AcctCode;
                                                }

                                                aPInvoiceDocument.Lines.ItemDescription = Item.Item.Name;
                                                aPInvoiceDocument.Lines.Quantity = Convert.ToDouble(Item.InvoicedQuantity.Value);
                                                aPInvoiceDocument.Lines.UserFields.Fields.Item("U_qty").Value = Convert.ToDouble(Item.InvoicedQuantity.Value);
                                                aPInvoiceDocument.Lines.UnitPrice = Convert.ToDouble(Item.Price.PriceAmount.Value);
                                                TaxMapping foundTaxMappping = TaxMappingList.Find(x => x.Code.Equals(Item.Item.ClassifiedTaxCategory.ID.Value));
                                                if (foundTaxMappping != null)
                                                {
                                                    aPInvoiceDocument.Lines.VatGroup = foundTaxMappping.Name;
                                                }
                                                aPInvoiceDocument.Lines.Add();
                                            }

                                            long docStatus = aPInvoiceDocument.Add();
                                            if (docStatus == 0)
                                            {
                                                /*using (var dbcontext = new PEPPOLEntities())
                                                {
                                                    Notifications aPInvoiceNotificationsNew = dbcontext.Notifications.Find(aPInvoiceNotifications.Idkey);
                                                    aPInvoiceNotificationsNew.SubmitToSAP = false;
                                                    dbcontext.SaveChanges();
                                                }*/
                                            }
                                            else
                                            {
                                                string syncMessage = string.Empty;
                                                string errMsg = string.Empty;

                                                SAPCompany.GetLastError(out int err, out errMsg);
                                                syncMessage = "AP Invoice Draft creation failed for DO:   sync error : " + err + "-" + errMsg;
                                            }
                                        }

                                        SAPCompany.Disconnect();

                                    }

                                }
                            }

                        }
                    }
                } while (totalRecords != lastIndex);

               

            }
            catch (Exception ex)
            {
                AppSpecificFunc.WriteLog(ex);
                //Receive AP Invoice
            }
           
        }

        #region "Receive AP CreditNote Draft"
        private void ReceiveCreditNote_Service()
        {
            try
            {

                string fromPEPPOLID = string.Empty;
                string toPEPPOLID = string.Empty;

                OADM companyObject = null;

                string DocNum = string.Empty;

                OACT GLCodeObject = null;
                using (var dbcontext = new EFSapDbContext())
                {
                    companyObject = dbcontext.CompanyDetails.FirstOrDefault();
                    GLCodeObject = dbcontext.GLCodes.Where(x => x.U_peppol.Equals("Y")).FirstOrDefault();

                }
                //toPEPPOLID = "0195:SGTSTDPSPTEST01";
                toPEPPOLID = companyObject.AliasName;

                string invoiceId = string.Empty;


                // -----Call API to get document nos for the peppolid-----
                int pageNum = 1;
                int totalRecords = 1;
                int lastIndex = -1;

                var objType = ConfigurationManager.AppSettings["AP_Credit_Note_ObjectType"];

                do
                {
                    var documentnocall = Task.Run(() => RetrivedDocNo(toPEPPOLID, "credit-notes", pageNum));
                    documentnocall.Wait();

                    ReceivedResponse documentcallResponse = documentnocall.Result;
                    PEPPOLInvoiceReceipt pEPPOLInvoiceReceipt = Newtonsoft.Json.JsonConvert.DeserializeObject<PEPPOLInvoiceReceipt>(documentcallResponse.ResponseContent);
                    totalRecords = pEPPOLInvoiceReceipt.total;
                    lastIndex = pEPPOLInvoiceReceipt.lastIndex;
                    pageNum = pageNum + 1;

                    foreach (var docList in pEPPOLInvoiceReceipt.info)
                    {
                        DocNum = docList.documentNo.ToString();
                        bool isDocExistAlready = AppSpecificFunc.IsDocNumAlreadyExist(DocNum, objType);

                        if (!isDocExistAlready)
                        {
                            // ----Call API to get xml file content by using Docnum-----

                            var XmlFileCall = Task.Run(() => RetrivedXMLDoc(DocNum, "credit-notes"));
                            XmlFileCall.Wait();

                            ReceivedResponse XmlFileResponse = XmlFileCall.Result;

                            if (XmlFileResponse.StatusCode == HttpStatusCode.OK)
                            { 

                                CreditNote CreditNote = null;

                                XmlDocument xmlDoc = new XmlDocument();
                                if (XmlFileResponse.ResponseContent.Contains("StandardBusinessDocument"))
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(StandardBusinessDocument));

                                    StandardBusinessDocument receviedAPInvoice = null;
                                    using (TextReader reader = new StringReader(XmlFileResponse.ResponseContent))
                                    {
                                        receviedAPInvoice = (StandardBusinessDocument)serializer.Deserialize(reader);
                                        CreditNote = receviedAPInvoice.CreditNote;
                                    }
                                }
                                else
                                {
                                    string CreditNoteTempXMLPath = ConfigurationManager.AppSettings["CreditNoteTempXMLPath"];

                                    xmlDoc.LoadXml(XmlFileResponse.ResponseContent.Replace("xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2\"", ""));
                                    xmlDoc.Save(CreditNoteTempXMLPath);
                                    XmlSerializer serializer1 = new XmlSerializer(typeof(CreditNote), new XmlRootAttribute("CreditNote"));

                                    using (FileStream fileStream = new FileStream(CreditNoteTempXMLPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        CreditNote = (CreditNote)serializer1.Deserialize(fileStream);

                                    }
                                }



                                if (CreditNote != null)
                                {
                                    //invoiceId = aPInvoiceNotifications.primarysubjectid;
                                    invoiceId = DocNum;
                                    string responseContent = string.Empty;

                                    fromPEPPOLID = docList.sender.ToString();

                                    //fromPEPPOLID = "0195:SGTSTDPSPTEST01";

                                    SAPbobsCOM.Company SAPCompany = new SAPbobsCOM.Company();
                                    GetSAPCompanyObject(ref SAPCompany);
                                    if (SAPCompany != null)
                                    {
                                        OCRD Supplier = null;
                                        using (var dbcontext = new EFSapDbContext())
                                        {
                                            Supplier = dbcontext.Customers.Where(x => x.U_custpeppolid.Equals(fromPEPPOLID) && x.CardType.Equals("S")).FirstOrDefault();
                                            //Supplier.CardCode = (Supplier.CardCode != null) ? Supplier.CardCode : "4000/I0023";
                                        }
                                        if (Supplier != null)
                                        {
                                            SAPbobsCOM.Documents aPInvoiceDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                            aPInvoiceDocument.CardCode = Supplier.CardCode;
                                            aPInvoiceDocument.CardName = CreditNote.AccountingSupplierParty.Party.PartyLegalEntity.RegistrationName.Value;

                                            DateTime ValidDocDate = Convert.ToDateTime(CreditNote.IssueDate);
                                            DateTime ValidDueDate = Convert.ToDateTime(CreditNote.DueDate);
                                            aPInvoiceDocument.DocDate = ValidDocDate;
                                            aPInvoiceDocument.DocDueDate = ValidDueDate;
                                            aPInvoiceDocument.TaxDate = ValidDocDate;
                                            aPInvoiceDocument.UserFields.Fields.Item("U_peppolref").Value = invoiceId;
                                            aPInvoiceDocument.NumAtCard = CreditNote.ID.Value;
                                            aPInvoiceDocument.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                                            aPInvoiceDocument.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes;



                                            List<TaxMapping> TaxMappingList = null;
                                            using (var dbcontext = new EFSapDbContext())
                                            {
                                                TaxMappingList = dbcontext.TaxMappings.ToList();
                                            }

                                            foreach (CreditNoteLine Item in CreditNote.CreditNoteLine)
                                            {
                                                if (GLCodeObject != null)
                                                {
                                                    aPInvoiceDocument.Lines.AccountCode = GLCodeObject.AcctCode;
                                                }

                                                aPInvoiceDocument.Lines.ItemDescription = Item.Item.Name;
                                                aPInvoiceDocument.Lines.Quantity = Convert.ToDouble(Item.CreditedQuantity.Value);
                                                aPInvoiceDocument.Lines.UserFields.Fields.Item("U_qty").Value = Convert.ToDouble(Item.CreditedQuantity.Value);
                                                aPInvoiceDocument.Lines.UnitPrice = Convert.ToDouble(Item.Price.PriceAmount.Value);
                                                TaxMapping foundTaxMappping = TaxMappingList.Find(x => x.Code.Equals(Item.Item.ClassifiedTaxCategory.ID.Value));
                                                if (foundTaxMappping != null)
                                                {
                                                    aPInvoiceDocument.Lines.VatGroup = foundTaxMappping.Name;
                                                }
                                                aPInvoiceDocument.Lines.Add();
                                            }

                                            long docStatus = aPInvoiceDocument.Add();
                                            if (docStatus == 0)
                                            {
                                                /*using (var dbcontext = new PEPPOLEntities())
                                                {
                                                    Notifications aPInvoiceNotificationsNew = dbcontext.Notifications.Find(aPInvoiceNotifications.Idkey);
                                                    aPInvoiceNotificationsNew.SubmitToSAP = false;
                                                    dbcontext.SaveChanges();
                                                }*/
                                            }
                                            else
                                            {
                                                string syncMessage = string.Empty;
                                                string errMsg = string.Empty;

                                                SAPCompany.GetLastError(out int err, out errMsg);
                                                syncMessage = "AP Creditnote Draft creation failed for DO:   sync error : " + err + "-" + errMsg;
                                            }
                                        }

                                        SAPCompany.Disconnect();

                                    }

                                }
                            }
                        }
                    }

                } while (totalRecords != lastIndex);
            }
            catch (Exception ex)
            {
                AppSpecificFunc.WriteLog(ex);
                //Receive AP credit
            }
            //Console.WriteLine(t.Result);
            //Console.ReadLine();

        }
        #endregion

        public static async Task<ReceivedResponse> RetrivedDocNo(string peppolId, string docType, int pageNum)
        {

            var client = new HttpClient();
            HttpResponseMessage response;

            string Peppol_URL = ConfigurationManager.AppSettings["Peppol_URL"];
            string Peppol_Authorization = ConfigurationManager.AppSettings["Peppol_Authorization"];



            // Call API to get document nos for the peppolid
            //string url = "https://peppol.datapost.com.sg/services/rest/peppol/business/v10/" + docType + ".json?receiver=" + peppolId + "&status=Receive Success" + "&pageNum=" + pageNum;
            string url = Peppol_URL + docType + ".json?receiver=" + peppolId + "&status=Receive Success" + "&pageNum=" + pageNum;

            //client.DefaultRequestHeaders.Add("Authorization", "Basic ZGIzOTdjODkyOWEwNDkxNWFhMzgxMWU2NmUwZTZiODI6ODM3NDllODQzNGRjNDM1YzkzZDA5NGY3NWJkMTRiNTA=");

            //client.DefaultRequestHeaders.Add("Authorization", "Basic ZGIzOTdjODkyOWEwNDkxNWFhMzgxMWU2NmUwZTZiODI6OTU1YmVlYTBiNWI5NDZjNjg5NTcxZGY2Y2ZmOTYxNjE=");
            client.DefaultRequestHeaders.Add("Authorization", Peppol_Authorization);


            response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            return new ReceivedResponse
            {
                Response = response,
                ResponseContent = content
            };
        }

        public static async Task<ReceivedResponse> RetrivedXMLDoc(string DocNum, string docType)
        {

            var client = new HttpClient();
            HttpResponseMessage response;

            string Peppol_URL = ConfigurationManager.AppSettings["Peppol_URL"];
            string Peppol_Authorization = ConfigurationManager.AppSettings["Peppol_Authorization"];


            // Call API to get document nos for the peppolid
            //string url = "https://peppol.datapost.com.sg/services/rest/peppol/business/v10/" + docType + "/" + DocNum + ".xml";
            string url = Peppol_URL + docType + "/" + DocNum + ".xml";

            //client.DefaultRequestHeaders.Add("Authorization", "Basic ZGIzOTdjODkyOWEwNDkxNWFhMzgxMWU2NmUwZTZiODI6OTU1YmVlYTBiNWI5NDZjNjg5NTcxZGY2Y2ZmOTYxNjE=");

            client.DefaultRequestHeaders.Add("Authorization", Peppol_Authorization);

            response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();

            return new ReceivedResponse
            {
                Response = response,
                ResponseContent = content,
                StatusCode = response.StatusCode
            };
        }


        #endregion
        #region "Send CreditNote"
        private void SendCreditNote()
        {
            try
            {
                string fromPEPPOLIDStr = string.Empty;
                string toPEPPOLIDStr = string.Empty;

               
                ORIN creditNoteHeaderObject = null;
                OADM companyObject = null;
                ADM1 companyDetailsObj = null;
                OCPR contactPersonObj = null;

                String companyName = "-";
                string SupplierstreetName = "-";
                string SupplierBlockName = "-";
                string SupplierCityName = "-";
                string SupplierCountry = "-";
                string SupplierZipCode = "-";

                string fromPeppolId = string.Empty;
                string fromPeppolIdName = string.Empty;


                List<UOMMapping> UOMMappingList = null;
                using (var dbcontext = new EFSapDbContext())
                {
                    UOMMappingList = dbcontext.UomMappings.ToList();
                }


                using (var dbcontext = new EFSapDbContext())
                {
                    creditNoteHeaderObject = dbcontext.CreditNoteHeaders.Include("CreditNoteLines").Include("Customer").AsNoTracking().Where(x => x.U_peppolsubmit.Equals("Y")).FirstOrDefault();
                    companyObject = dbcontext.CompanyDetails.FirstOrDefault();
                    companyDetailsObj = dbcontext.CompanyInfo.FirstOrDefault();
                    if(creditNoteHeaderObject != null)
                        contactPersonObj = dbcontext.ContactPerson.Where(x => x.CntctCode.Equals(creditNoteHeaderObject.CntctCode)).FirstOrDefault();
                    
                }

                if (companyObject != null)
                {
                    companyName = companyObject.CompnyName;
                    fromPEPPOLIDStr = companyObject.AliasName;

                    string[] fromPEPPOLIDArr = fromPEPPOLIDStr.Split(':');
                    fromPeppolId = fromPEPPOLIDArr[0];
                    fromPeppolIdName = fromPEPPOLIDArr[1];

                    SupplierstreetName = (companyDetailsObj.Street != null) ? companyDetailsObj.Street : SupplierstreetName;
                    SupplierCityName = (companyDetailsObj.City != null) ? companyDetailsObj.City : SupplierCityName;
                    SupplierBlockName = (companyDetailsObj.Block != null) ? companyDetailsObj.Block : SupplierBlockName;
                    SupplierCountry = (companyDetailsObj.Country != null) ? companyDetailsObj.Country : SupplierCountry;
                    SupplierZipCode = (companyDetailsObj.ZipCode != null) ? companyDetailsObj.ZipCode : SupplierZipCode;

                }
                if (creditNoteHeaderObject != null)
                {

                    string[] VatGroups = creditNoteHeaderObject.CreditNoteLines.Select(x => x.VatGroup).Distinct().ToArray();
                    int totalTaxGroups = VatGroups.Count();
                    toPEPPOLIDStr = creditNoteHeaderObject.Customer.U_custpeppolid;

                    if (toPEPPOLIDStr == null)
                    {
                        toPEPPOLIDStr = "0195:SGTSTDPSPTEST01";
                    }

                    string[] toPePPOLIDArr = toPEPPOLIDStr.Split(':');
                    string peppolId = toPePPOLIDArr[0];
                    string peppolIdName = toPePPOLIDArr[1];

                    if (creditNoteHeaderObject.Comments != null)
                        creditNoteHeaderObject.Comments = (creditNoteHeaderObject.Comments.Trim() != "") ? creditNoteHeaderObject.Comments : "-";//Need confirmation

                    // Creates the Invoice Document.
                    CreditNote creditNoteDocument = new CreditNote
                    {
                        ID = new ID
                        {
                            Value = creditNoteHeaderObject.DocNum.ToString()
                        },

                        IssueDate = creditNoteHeaderObject.DocDate.ToString("yyyy-MM-dd"),
                        CreditNoteTypeCode = "381", //Need confirmation
                        DocumentCurrencyCode = creditNoteHeaderObject.DocCur, //Need confirmation
                                                                              // BuyerReference = "-", //Need confirmation
 
                        Note = new Note
                        {
                            Value = (creditNoteHeaderObject.Comments != null) ? creditNoteHeaderObject.Comments : "-" //Need confirmation
                        },            

                                               
                        BuyerReference = (creditNoteHeaderObject.U_bu != null) ? creditNoteHeaderObject.U_bu : "-",


                        BillingReference = new BillingReference
                        {
                            InvoiceDocumentReference = new InvoiceDocumentReference
                            {
                                ID = new ID
                                {
                                    // Value = creditNoteHeaderObject.DocNum.ToString()
                                    Value = (creditNoteHeaderObject.U_inv != null) ? creditNoteHeaderObject.U_inv : "-"
                                    
                                }
                            }
                        },
                        AccountingSupplierParty = new AccountingSupplierParty
                        {
                            Party = new Party
                            {
                                EndpointID = new EndpointID
                                {
                                    schemeID = fromPeppolId,
                                    Value = fromPeppolIdName //Need confirmation                                     
                                },
                                PartyIdentification = new PartyIdentification
                                {
                                    ID = new ID
                                    {
                                        schemeID = fromPeppolId,
                                        Value = fromPeppolIdName //Need confirmation
                                    }

                                },
                                PostalAddress = new PostalAddress
                                {
                                    StreetName = new StreetName
                                    {
                                        Value = SupplierstreetName  //Need confirmation
                                    },
                                    AdditionalStreetName = new AdditionalStreetName
                                    {
                                        Value = SupplierBlockName  //Need confirmation
                                    },
                                    CityName = new CityName
                                    {
                                        Value = SupplierCityName  //Need confirmation
                                    },
                                    PostalZone = new PostalZone
                                    {
                                        Value = SupplierZipCode //Need confirmation
                                    },
                                    CountrySubentity = new CountrySubentity
                                    {
                                        Value = "-"  //Need confirmation
                                    },
                                    Country = new Country
                                    {
                                        IdentificationCode = SupplierCountry
                                    }
                                },
                                PartyLegalEntity = new PartyLegalEntity
                                {
                                    //RegistrationName = companyName  //Need confirmation
                                    RegistrationName = new RegistrationName
                                    {
                                        Value = companyName
                                    },
                                    CompanyID = new CompanyID
                                    {
                                        //Value = fromPEPPOLIDStr  //Need confirmation
                                        Value = (creditNoteHeaderObject.Customer.LicTradNum != null) ? creditNoteHeaderObject.Customer.LicTradNum : "-",
                                    }
                                },
                                Contact = new Contact
                                {
                                    Name = "-", //Need confirmation
                                    ElectronicMail = (companyDetailsObj.IntrntAdrs != null) ? companyDetailsObj.IntrntAdrs : "-"
                                }
                            }
                        },
                        AccountingCustomerParty = new AccountingCustomerParty
                        {
                            Party = new Party
                            {
                                EndpointID = new EndpointID
                                {
                                    schemeID = peppolId,
                                    Value = peppolIdName //Need confirmation                                     
                                },

                                PostalAddress = new PostalAddress
                                {
                                    StreetName = new StreetName
                                    {
                                        Value = creditNoteHeaderObject.Customer.Address  //Need confirmation
                                    },
                                    AdditionalStreetName = new AdditionalStreetName
                                    {
                                        Value = (creditNoteHeaderObject.Customer.Block != null) ? creditNoteHeaderObject.Customer.Block : "-"  //Need confirmation
                                    },
                                    CityName = new CityName
                                    {
                                        Value = (creditNoteHeaderObject.Customer.City != null) ? creditNoteHeaderObject.Customer.City : "-"  //Need confirmation
                                    },
                                    PostalZone = new PostalZone
                                    {
                                        Value = (creditNoteHeaderObject.Customer.ZipCode != null) ? creditNoteHeaderObject.Customer.ZipCode : "-"  //Need confirmation
                                    },
                                    CountrySubentity = new CountrySubentity
                                    {
                                        Value = "-"  //Need confirmation
                                    },
                                    Country = new Country
                                    {
                                        IdentificationCode = (creditNoteHeaderObject.Customer.Country != null) ? creditNoteHeaderObject.Customer.Country : "-"  //Need confirmation
                                    }
                                },
                                PartyTaxScheme = new PartyTaxScheme
                                {
                                    CompanyID = new CompanyID
                                    {
                                        Value = "-"  //Need confirmation
                                    },
                                    TaxScheme = new TaxScheme
                                    {
                                        ID = new ID
                                        {
                                            Value = "GST"
                                        }
                                    }

                                },
                                PartyLegalEntity = new PartyLegalEntity
                                {
                                    RegistrationName = new RegistrationName
                                    {
                                        Value = creditNoteHeaderObject.CardName //Need confirmation
                                    },
                                    CompanyID = new CompanyID
                                    {
                                        Value = toPEPPOLIDStr  //Need confirmation
                                    }
                                },
                                Contact = new Contact
                                {
                                    Name = (contactPersonObj.Name != null) ? contactPersonObj.Name : "-", //Need confirmation
                                    ElectronicMail = "-"

                                }

                            }
                        },
                        AllowanceCharge = new AllowanceCharge
                        {
                            ChargeIndicator = true,

                            AllowanceChargeReasonCode = "DL",

                            AllowanceChargeReason = "N/A",

                            Amount = new Amount
                            {
                                currencyID = "SGD",
                                Value = "0"
                            },
                            TaxCategory = new TaxCategory
                            {
                                ID = new ID
                                {
                                    Value = "SR"

                                },
                                Percent = 7,
                                TaxScheme = new TaxScheme
                                {
                                    ID = new ID
                                    {
                                        Value = "GST"
                                    }
                                }

                            }
                        },
                        TaxTotal = new TaxTotal
                        {
                            TaxAmount = new TaxAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur,
                                Value = Math.Round(creditNoteHeaderObject.CreditNoteLines.Sum(x => x.VatSum), 2)
                            },
                            TaxSubtotal = new TaxSubtotal[totalTaxGroups]
                        },
                        LegalMonetaryTotal = new LegalMonetaryTotal
                        {
                            LineExtensionAmount = new LineExtensionAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur,
                                Value = Math.Round(creditNoteHeaderObject.CreditNoteLines.Sum(x => x.LineTotal), 2)
                            },
                            TaxExclusiveAmount = new TaxExclusiveAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur,
                                Value = Math.Round(creditNoteHeaderObject.CreditNoteLines.Sum(x => x.LineTotal), 2)
                            },
                            TaxInclusiveAmount = new TaxInclusiveAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur,
                                Value = Math.Round(creditNoteHeaderObject.DocTotal, 2)
                            },
                            ChargeTotalAmount = new ChargeTotalAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur,
                                Value = 0
                            },
                            PayableAmount = new PayableAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur,
                                Value = Math.Round(creditNoteHeaderObject.DocTotal, 2)
                            }
                        },
                        CreditNoteLine = new CreditNoteLine[creditNoteHeaderObject.CreditNoteLines.Count]
                    };

                    //Tax SubTotal Assignments - Stars Here
                    for (int i = 0; i < totalTaxGroups; i++)
                    {

                        creditNoteDocument.TaxTotal.TaxSubtotal[i] = new TaxSubtotal
                        {
                            TaxableAmount = new TaxableAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur, //Need confirmation
                                Value = Math.Round(creditNoteHeaderObject.CreditNoteLines.Where(x => x.VatGroup.Equals(VatGroups[i])).Sum(x => x.LineTotal), 2)
                            },
                            TaxAmount = new TaxAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur, //Need confirmation
                                Value = Math.Round(creditNoteHeaderObject.CreditNoteLines.Where(x => x.VatGroup.Equals(VatGroups[i])).Sum(x => x.VatSum), 2)
                            },
                            TaxCategory = new TaxCategory
                            {
                                ID = new ID
                                {
                                    Value = VatGroups[i] //Need Confirmation

                                },
                                Percent = Math.Round(creditNoteHeaderObject.CreditNoteLines.Where(x => x.VatGroup.Equals(VatGroups[i])).Max(x => x.VatPrcnt), 2),
                                TaxScheme = new TaxScheme
                                {
                                    ID = new ID
                                    {
                                        Value = "GST"   //Need confirmation
                                    }
                                }
                            }
                        };
                    }
                    //Tax SubTotal Assignments - End Here
                    //Line Information - stars Here
                    int j = 1;
                    foreach (RIN1 creditNoteLine in creditNoteHeaderObject.CreditNoteLines)
                    {
                        UOMMapping foundUOMMapping = null;
                        if (creditNoteLine.unitMsr != null)
                        {
                            foundUOMMapping = UOMMappingList.Find(x => x.Name.ToUpper().Trim().Equals(creditNoteLine.unitMsr.ToString().ToUpper().Trim()));
                        }
                        
                        string foundUOM = "H87";
                        if (foundUOMMapping != null)
                        {
                            foundUOM = foundUOMMapping.Code;
                        }
                        creditNoteDocument.CreditNoteLine[j - 1] = new CreditNoteLine
                        {
                            ID = new ID
                            {
                                Value = j.ToString()
                            },
                            CreditedQuantity = new CreditedQuantity
                            {
                                unitCode = foundUOM, // Need Confirmation
                                Value = Math.Round(creditNoteLine.Quantity, 2)
                            },
                            LineExtensionAmount = new LineExtensionAmount
                            {
                                currencyID = creditNoteHeaderObject.DocCur, // Need Confirmation
                                Value = Math.Round(creditNoteLine.LineTotal, 2)
                            },
                            OrderLineReference = new OrderLineReference
                            {
                                LineID = "2"
                            },
                            Item = new Item
                            {
                                Name = creditNoteLine.Dscription,
                                SellersItemIdentification = new SellersItemIdentification
                                {
                                    ID = new ID
                                    {
                                        Value = creditNoteLine.ItemCode
                                    }
                                },
                                /*BuyersItemIdentification = new BuyersItemIdentification
                                {
                                    ID = new ID
                                    {
                                        Value = creditNoteLine.SubCatNum
                                    }
                                },*/
                                ClassifiedTaxCategory = new ClassifiedTaxCategory
                                {
                                    ID = new ID
                                    {
                                        Value = creditNoteLine.VatGroup
                                    },
                                    Percent = Math.Round(creditNoteLine.VatPrcnt, 2),
                                    TaxScheme = new TaxScheme
                                    {
                                        ID = new ID
                                        {
                                            Value = "GST" // Need Confirmation
                                        }
                                    }
                                }
                            },
                            Price = new Price
                            {
                                PriceAmount = new PriceAmount
                                {
                                    currencyID = creditNoteLine.Currency, // Need Confirmation
                                    Value = Math.Round(creditNoteLine.Price, 2)
                                },
                                /* BaseQuantity = new BaseQuantity
                                 {
                                     unitCode = foundUOM, // Need Confirmation
                                     Value = Math.Round(1.0m, 2)
                                 }*/
                            }
                        };
                        j++;
                    }
                    //Line Information - Ends Here

                    XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
                    xmlNameSpace.Add("cac", Invoice.cacNameSpace);
                    xmlNameSpace.Add("cbc", Invoice.cbcNameSpace);
                    xmlNameSpace.Add("ccts", Invoice.cctsNameSpace);
                    xmlNameSpace.Add("ext", Invoice.extNameSpace);
                    xmlNameSpace.Add("qdt", Invoice.qdtNameSpace);
                    xmlNameSpace.Add("udt", Invoice.udtNameSpace);
                    xmlNameSpace.Add("xsi", Invoice.xsiNameSpace);
                    xmlNameSpace.Add("xsd", Invoice.xsdNameSpace);



                    XmlSerializer serializer = new XmlSerializer(typeof(CreditNote));

                    string CreditNoteXMLPath = ConfigurationManager.AppSettings["CreditNoteXMLPath"];

                    TextWriter writer = new StreamWriter(CreditNoteXMLPath);

                    serializer.Serialize(writer, creditNoteDocument, xmlNameSpace);
                    writer.Close();

                    // string xmlString = "";
                    //  using (StringWriter textWriter = new StringWriter())
                    //  {
                    //      serializer.Serialize(textWriter, creditNoteDocument, xmlNameSpace);
                    //      xmlString = textWriter.ToString().Replace("utf-16", "utf-8");
                    // }

                    string filePath = CreditNoteXMLPath;
                    var t = Task.Run(() => uploadFileAsync("peppol-credit-note-2", "credit-notes", filePath));
                    t.Wait();

                    ReceivedResponse receivedResponse = t.Result;
                    PEPPOLResponse pEPPOLResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PEPPOLResponse>(receivedResponse.ResponseContent);

                    if (receivedResponse.Response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString))
                        {
                            string UpdateQuery = "UPDATE ORIN SET" +
                                " U_peppolsubmit = @U_peppolsubmit, " +
                                " U_peppolstatus = @U_peppolstatus, " +
                                " U_peppolref = @U_peppolref " +
                                " WHERE DocEntry = @DocEntry" +
                                "";

                            connection.Open();
                            using (SqlCommand command = new SqlCommand(UpdateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@U_peppolsubmit", "N");
                                command.Parameters.AddWithValue("@U_peppolstatus", "Submitted");
                                command.Parameters.AddWithValue("@U_peppolref", pEPPOLResponse.clientRef);

                                command.Parameters.AddWithValue("@DocEntry", creditNoteHeaderObject.DocEntry);
                                command.ExecuteNonQuery();
                            }

                        }
                        AppSpecificFunc.SendInvoiceEMail(creditNoteHeaderObject.Customer.e_mail, creditNoteHeaderObject.DocNum.ToString(), companyName, "Credit Note");
                       
                    }
                    //Console.WriteLine(t.Result);
                    //Console.ReadLine();
                }


            }
            catch (Exception ex)
            {
                AppSpecificFunc.WriteLog(ex);
                //send credit note
            }
        }
        #endregion
        #region "Send Invoice"
        private void SendInvoice()
        {
            try
            {
                string fromPEPPOLIDStr = string.Empty;
                string toPEPPOLIDStr = string.Empty;

                OINV invoiceHeaderObject = null;
                OADM companyObject = null;
                ADM1 companyDetailsObj = null;
                OCPR contactPersonObj = null;

                String companyName = "-";
                string SupplierstreetName = "-";
                string SupplierBlockName = "-";
                string SupplierCityName = "-";
                string SupplierCountry = "-";

                string fromPeppolId = string.Empty;
                string fromPeppolIdName = string.Empty;

                List<UOMMapping> UOMMappingList = null;
                using (var dbcontext = new EFSapDbContext())
                {
                    UOMMappingList = dbcontext.UomMappings.ToList();
                }

                using (var dbcontext = new EFSapDbContext())
                {
                    invoiceHeaderObject = dbcontext.InvoiceHeaders.Include("InvoiceLines").Include("Customer").Include("PaymentTerms").AsNoTracking().Where(x => x.U_peppolsubmit.Equals("Y")).FirstOrDefault();
                    companyObject = dbcontext.CompanyDetails.FirstOrDefault();
                    companyDetailsObj = dbcontext.CompanyInfo.FirstOrDefault();
                    if (invoiceHeaderObject != null)
                        contactPersonObj = dbcontext.ContactPerson.Where(x => x.CntctCode.Equals(invoiceHeaderObject.cntctcode)).FirstOrDefault();
                }

                if (companyObject != null)
                {
                    companyName = companyObject.CompnyName;
                    fromPEPPOLIDStr = companyObject.AliasName;

                    string[] fromPEPPOLIDArr = fromPEPPOLIDStr.Split(':');
                    fromPeppolId = fromPEPPOLIDArr[0];
                    fromPeppolIdName = fromPEPPOLIDArr[1];

                    SupplierstreetName = (companyDetailsObj.Street != null) ? companyDetailsObj.Street : SupplierstreetName;
                    SupplierCityName = (companyDetailsObj.City != null) ? companyDetailsObj.City : SupplierCityName;
                    SupplierBlockName = (companyDetailsObj.Block != null) ? companyDetailsObj.Block : SupplierBlockName;
                    SupplierCountry = (companyDetailsObj.Country != null) ? companyDetailsObj.Country : SupplierCountry;

                }
                if (invoiceHeaderObject != null)
                {

                    string[] VatGroups = invoiceHeaderObject.InvoiceLines.Select(x => x.VatGroup).Distinct().ToArray();
                    int totalTaxGroups = VatGroups.Count();
                    toPEPPOLIDStr = invoiceHeaderObject.Customer.U_custpeppolid;

                    string[] toPePPOLIDArr = toPEPPOLIDStr.Split(':');
                    string topeppolId = toPePPOLIDArr[0];
                    string topeppolIdName = toPePPOLIDArr[1];

                    string OrderRef = "-";
                    if (invoiceHeaderObject.NumAtCard != null)
                    {
                        OrderRef = invoiceHeaderObject.NumAtCard.ToString();
                        OrderRef = (OrderRef != string.Empty) ? OrderRef : "-";
                    }


                    if (invoiceHeaderObject.Comments != null)
                        invoiceHeaderObject.Comments = (invoiceHeaderObject.Comments.Trim() != "") ? invoiceHeaderObject.Comments : "-";//Need confirmation



                    // Creates the Invoice Document.
                    Invoice invoiceDocument = new Invoice
                    {
                        ID = new ID
                        {
                            Value = invoiceHeaderObject.DocNum.ToString()
                        },

                        IssueDate = invoiceHeaderObject.DocDate.ToString("yyyy-MM-dd"),
                        DueDate = invoiceHeaderObject.DocDueDate.ToString("yyyy-MM-dd"),
                        InvoiceTypeCode = "380", //Need confirmation
                        DocumentCurrencyCode = invoiceHeaderObject.DocCur, //Need confirmation
                        //BuyerReference = (invoiceHeaderObject.u_bu != null) ? invoiceHeaderObject.u_bu : "-",
                       // Note = (invoiceHeaderObject.Comments != null) ? invoiceHeaderObject.Comments : "-",

                        Note = new Note
                        {
                            Value = (invoiceHeaderObject.Comments != null) ? invoiceHeaderObject.Comments : "-" //Need confirmation
                        },

                        BuyerReference = (invoiceHeaderObject.u_bu != null) ? invoiceHeaderObject.u_bu : "-",

                        OrderReference = new OrderReference
                        {
                            ID = new ID
                            {
                                Value = OrderRef
                            }
                        },
                        AccountingSupplierParty = new AccountingSupplierParty
                        {
                            Party = new Party
                            {
                                EndpointID = new EndpointID
                                {
                                    schemeID = fromPeppolId,
                                    Value = fromPeppolIdName
                                },

                                PartyName = new PartyName
                                {
                                    Name = fromPeppolIdName //Need confirmation                                     
                                },
                                PostalAddress = new PostalAddress
                                {
                                    StreetName = new StreetName
                                    {
                                        Value = SupplierstreetName  //Need confirmation
                                    },
                                    Country = new Country
                                    {
                                        IdentificationCode = SupplierCountry  //Need confirmation
                                    }
                                },
                                PartyTaxScheme = new PartyTaxScheme
                                {
                                    TaxScheme = new TaxScheme
                                    {
                                        ID = new ID
                                        {
                                            Value = "GST"
                                        }
                                    }

                                },
                                PartyLegalEntity = new PartyLegalEntity
                                {
                                    //RegistrationName = companyName  //Need confirmation
                                    RegistrationName = new RegistrationName
                                    {
                                        Value = companyName  //Need confirmation
                                    },
                                    CompanyID = new CompanyID
                                    {
                                        //Value = fromPEPPOLIDStr  //Need confirmation
                                        Value = (invoiceHeaderObject.Customer.LicTradNum != null) ? invoiceHeaderObject.Customer.LicTradNum : "-",
                                    }
                                },

                                Contact = new Contact
                                {
                                    Name = "-",
                                    //ElectronicMail = "-"

                                    //Name = invoiceHeaderObject.cntctcode.ToString(),
                                    ElectronicMail = (companyDetailsObj.IntrntAdrs != null) ? companyDetailsObj.IntrntAdrs : "-"

                                }
                            }
                        },
                        AccountingCustomerParty = new AccountingCustomerParty
                        {
                            Party = new Party
                            {
                                EndpointID = new EndpointID
                                {
                                    schemeID = topeppolId,
                                    Value = topeppolIdName //Need confirmation                                     
                                },
                                PartyName = new PartyName
                                {
                                    Name = invoiceHeaderObject.CardName //Need confirmation                                     
                                },
                                PostalAddress = new PostalAddress
                                {
                                    StreetName = new StreetName
                                    {
                                        Value = (invoiceHeaderObject.Customer.Address != null) ? invoiceHeaderObject.Customer.Address : "-"  //Need confirmation
                                    },
                                    Country = new Country
                                    {
                                        IdentificationCode = (invoiceHeaderObject.Customer.Country != null) ? invoiceHeaderObject.Customer.Country : "-"  //Need confirmation
                                    }
                                },
                                PartyTaxScheme = new PartyTaxScheme
                                {
                                    TaxScheme = new TaxScheme
                                    {
                                        ID = new ID
                                        {
                                            Value = "GST"
                                        }
                                    }

                                },
                                PartyLegalEntity = new PartyLegalEntity
                                {
                                    //RegistrationName = companyName  //Need confirmation
                                    RegistrationName = new RegistrationName
                                    {
                                        Value = invoiceHeaderObject.CardName  //Need confirmation
                                    },
                                    CompanyID = new CompanyID
                                    {
                                        Value = toPEPPOLIDStr  //Need confirmation
                                    }
                                },
                                Contact = new Contact
                                {
                                    Name = (contactPersonObj.Name != null) ? contactPersonObj.Name : "-" //Need confirmation
                                    //Name = invoiceHeaderObject.cntctcode.ToString(),
                                    //ElectronicMail = "-"

                                    //Name = invoiceHeaderObject.cntctcode.ToString(),
                                    //ElectronicMail = (companyDetailsObj.IntrntAdrs != null) ? companyDetailsObj.IntrntAdrs : "-"

                                }


                            }
                        },
                        PaymentTerms = new PaymentTerms
                        {
                            Note = new Note
                            {
                                Value = invoiceHeaderObject.PaymentTerms.PymntGroup.ToString() //Need confirmation
                            }
                        },
                        TaxTotal = new TaxTotal
                        {
                            TaxAmount = new TaxAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur,
                                Value = Math.Round(invoiceHeaderObject.InvoiceLines.Sum(x => x.VatSum), 2)
                            },
                            TaxSubtotal = new TaxSubtotal[totalTaxGroups]
                        },
                        LegalMonetaryTotal = new LegalMonetaryTotal
                        {
                            LineExtensionAmount = new LineExtensionAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur,
                                Value = Math.Round(invoiceHeaderObject.InvoiceLines.Sum(x => x.LineTotal), 2)
                            },
                            TaxExclusiveAmount = new TaxExclusiveAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur,
                                Value = Math.Round(invoiceHeaderObject.InvoiceLines.Sum(x => x.LineTotal), 2)
                            },
                            TaxInclusiveAmount = new TaxInclusiveAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur,
                                Value = Math.Round(invoiceHeaderObject.DocTotal, 2)
                            },
                            AllowanceTotalAmount = new AllowanceTotalAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur,
                                //Value = Math.Round(invoiceHeaderObject.InvoiceLines.Sum(x => x.LineTotal), 2)
                                Value = 0
                            },
                            ChargeTotalAmount = new ChargeTotalAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur,
                                //Value = Math.Round(invoiceHeaderObject.DocTotal, 2)
                                Value = 0
                            },
                            PayableAmount = new PayableAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur,
                                Value = Math.Round(invoiceHeaderObject.DocTotal, 2)
                            }
                        },
                        InvoiceLine = new InvoiceLine[invoiceHeaderObject.InvoiceLines.Count]
                    };

                    //Tax SubTotal Assignments - Stars Here
                    for (int i = 0; i < totalTaxGroups; i++)
                    {

                        invoiceDocument.TaxTotal.TaxSubtotal[i] = new TaxSubtotal
                        {
                            TaxableAmount = new TaxableAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur, //Need confirmation
                                Value = Math.Round(invoiceHeaderObject.InvoiceLines.Where(x => x.VatGroup.Equals(VatGroups[i])).Sum(x => x.LineTotal), 2)
                            },
                            TaxAmount = new TaxAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur, //Need confirmation
                                Value = Math.Round(invoiceHeaderObject.InvoiceLines.Where(x => x.VatGroup.Equals(VatGroups[i])).Sum(x => x.VatSum), 2)
                            },
                            TaxCategory = new TaxCategory
                            {
                                ID = new ID
                                {
                                    Value = VatGroups[i] //Need Confirmation

                                },
                                Percent = Math.Round(invoiceHeaderObject.InvoiceLines.Where(x => x.VatGroup.Equals(VatGroups[i])).Max(x => x.VatPrcnt), 2),
                                TaxScheme = new TaxScheme
                                {
                                    ID = new ID
                                    {
                                        Value = "GST"   //Need confirmation
                                    }
                                }
                            }
                        };
                    }
                    //Tax SubTotal Assignments - End Here
                    //Line Information - stars Here
                    int j = 1;
                    foreach (INV1 invoiceLine in invoiceHeaderObject.InvoiceLines)
                    {
                        UOMMapping foundUOMMapping = null;
                        if (invoiceLine.unitMsr != null)
                        {
                            foundUOMMapping = UOMMappingList.Find(x => x.Name.ToUpper().Trim().Equals(invoiceLine.unitMsr.ToString().ToUpper().Trim()));
                        }
                        
                        string foundUOM = "H87";
                        if (foundUOMMapping != null)
                        {
                            foundUOM = foundUOMMapping.Code;
                        }
                        invoiceDocument.InvoiceLine[j - 1] = new InvoiceLine
                        {
                            ID = new ID
                            {
                                Value = j.ToString()
                            },
                            InvoicedQuantity = new InvoicedQuantity
                            {
                                unitCode = foundUOM, // Need Confirmation


                                Value = Math.Round(invoiceLine.Quantity, 2)
                            },
                            LineExtensionAmount = new LineExtensionAmount
                            {
                                currencyID = invoiceHeaderObject.DocCur, // Need Confirmation
                                Value = Math.Round(invoiceLine.LineTotal, 2)
                            },
                            Item = new Item
                            {
                                Name = invoiceLine.Dscription,

                                ClassifiedTaxCategory = new ClassifiedTaxCategory
                                {
                                    ID = new ID
                                    {
                                        Value = invoiceLine.VatGroup

                                    },
                                    Percent = Math.Round(invoiceLine.VatPrcnt, 2),
                                    TaxScheme = new TaxScheme
                                    {
                                        ID = new ID
                                        {
                                            Value = "GST" // Need Confirmation
                                        }
                                    }
                                }
                            },
                            Price = new Price
                            {
                                PriceAmount = new PriceAmount
                                {
                                    currencyID = invoiceLine.Currency, // Need Confirmation
                                    Value = Math.Round(invoiceLine.Price, 2)
                                },
                                BaseQuantity = new BaseQuantity
                                {
                                    unitCode = foundUOM, // Need Confirmation
                                    Value = Math.Round(1.0m, 2)
                                }
                            }
                        };
                        j++;
                    }
                    //Line Information - Ends Here

                    XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
                    xmlNameSpace.Add("cac", Invoice.cacNameSpace);
                    xmlNameSpace.Add("cbc", Invoice.cbcNameSpace);
                    xmlNameSpace.Add("ccts", Invoice.cctsNameSpace);
                    xmlNameSpace.Add("ext", Invoice.extNameSpace);
                    xmlNameSpace.Add("qdt", Invoice.qdtNameSpace);
                    xmlNameSpace.Add("udt", Invoice.udtNameSpace);
                    xmlNameSpace.Add("xsi", Invoice.xsiNameSpace);
                    xmlNameSpace.Add("xsd", Invoice.xsdNameSpace);


                    string strInvoiceXMLPath = ConfigurationManager.AppSettings["InvoiceXMLPath"];

                    XmlSerializer serializer = new XmlSerializer(typeof(Invoice));
                    //TextWriter writer = new StreamWriter(@"D:\invoicexml.xml");
                    TextWriter writer = new StreamWriter(strInvoiceXMLPath);
                    serializer.Serialize(writer, invoiceDocument, xmlNameSpace);
                    writer.Close();


                    //string filePath = @"D:\invoicexml.xml";
                    string filePath = strInvoiceXMLPath;
                    var t = Task.Run(() => uploadFileAsync("peppol-invoice-2", "invoices", filePath));

                    t.Wait();

                    ReceivedResponse receivedResponse = t.Result;
                    PEPPOLResponse pEPPOLResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PEPPOLResponse>(receivedResponse.ResponseContent);
                    if (receivedResponse.Response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        //string DocNum = pEPPOLResponse.info[0].documentNo;
                        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString))
                        {
                            string UpdateQuery = "UPDATE OINV SET" +
                                " U_peppolsubmit = @U_peppolsubmit, " +
                                " U_peppolstatus = @U_peppolstatus, " +
                                 " U_peppolref = @U_peppolref " +
                                " WHERE DocEntry = @DocEntry" +
                                "";

                            connection.Open();
                            using (SqlCommand command = new SqlCommand(UpdateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@U_peppolsubmit", "N");
                                command.Parameters.AddWithValue("@U_peppolstatus", "Submitted");
                                command.Parameters.AddWithValue("@U_peppolref", pEPPOLResponse.clientRef);
                                command.Parameters.AddWithValue("@DocEntry", invoiceHeaderObject.DocEntry);
                                command.ExecuteNonQuery();
                            }

                        }

                        AppSpecificFunc.SendInvoiceEMail(invoiceHeaderObject.Customer.e_mail, invoiceHeaderObject.DocNum.ToString(), companyName, "Invoice");

                      

                        // Console.WriteLine(t.Result);
                        // Console.ReadLine();
                    }
                }
            }

            catch (Exception ex)
            {
                AppSpecificFunc.WriteLog(ex);
                //send  Invoice
            }
        }
       

        public static async Task<ReceivedResponse> uploadFileAsync(string InvoiceOrCredit, string docType, string filePath)
        {

            string url = "";
            Guid ClientRef = Guid.NewGuid();
            string ClientRefNum = ClientRef.ToString();

            HttpResponseMessage response;
            string responseContent = string.Empty;
            String headerValue = "";


            string Peppol_URL = ConfigurationManager.AppSettings["Peppol_URL"];
            string Peppol_Authorization = ConfigurationManager.AppSettings["Peppol_Authorization"];


            //url = "https://peppol.datapost.com.sg/services/rest/peppol/business/v10/" + docType + "/" + InvoiceOrCredit + "/" + ClientRefNum;
            url = Peppol_URL + docType + "/" + InvoiceOrCredit + "/" + ClientRefNum;

            if (docType == "invoices")
            {
                // filePath = @"D:\invoicexml.xml";
                headerValue = "form-data; name=\"document\"; filename=\"" + "invoicexml.xml" + "\"";
            }
            else if (docType == "credit-notes")
            {
                //filePath = @"D:\creditnotexml.xml";
                headerValue = "form-data; name=\"document\"; filename=\"" + "creditnotexml.xml" + "\"";

            }

            using (var httpClient = new HttpClient())
            {
                //httpClient.DefaultRequestHeaders.Add("Authorization", "Basic ZGIzOTdjODkyOWEwNDkxNWFhMzgxMWU2NmUwZTZiODI6OTU1YmVlYTBiNWI5NDZjNjg5NTcxZGY2Y2ZmOTYxNjE=");
                httpClient.DefaultRequestHeaders.Add("Authorization", Peppol_Authorization);

                using (var form = new MultipartFormDataContent())
                {
                    using (var fs = File.OpenRead(filePath))
                    {
                        using (var streamContent = new StreamContent(fs))
                        {
                            using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
                            {
                                //String headerValue = "form-data; name=\"document\"; filename=\"" + "PO4.xml" + "\"";
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                                fileContent.Headers.Add("Content-Disposition", headerValue);
                                // "file" parameter name should be the same as the server side input parameter name
                                form.Add(fileContent, "file", Path.GetFileName(filePath));
                                response = await httpClient.PutAsync(url, form);
                                responseContent = await response.Content.ReadAsStringAsync();
                                if (response != null)
                                {

                                }

                            }
                        }
                    }
                }
            }

            return new ReceivedResponse
            {
                Response = response,
                ResponseContent = responseContent
            };

        }
        #endregion
    }
}
