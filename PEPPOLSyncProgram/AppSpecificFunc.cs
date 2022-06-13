using PEPPOLSyncProgram.CustomDB;
using PEPPOLSyncProgram.SAPDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PEPPOLSyncProgram
{
    public static  class AppSpecificFunc
    {
        public static void WriteLog(Exception ex )
        {
            try
            {

          
            SyncErrLog errLog = new SyncErrLog();
            StackTrace stackTrace = new StackTrace(ex, true);
            errLog.errMsg = ex.Message;
            if(ex.InnerException != null)
            {
                errLog.InnerException = ex.InnerException.Message;            
            }
            int frameCount = stackTrace.FrameCount;
            for(int i=0;i<frameCount;i++)
            {
                if(stackTrace.GetFrame(i).Equals(null))
                {
                    errLog.FileName = stackTrace.GetFrame(i).GetFileName().ToString();
                    errLog.LineNumber = stackTrace.GetFrame(i).GetFileLineNumber().ToString();
                }
            }
            errLog.CreatedOn = DateTime.Now;
            using (var dbcontext = new PEPPOLEntities())
            {
                dbcontext.SyncErrLog.Add(errLog);
                dbcontext.SaveChanges();
            }
            }
            catch(Exception excep)
            {
                Console.Write(excep);
            }
        }
        public static bool IsDocNumAlreadyExist(string DocNum, string objType)  
        {
            bool result = true;
            using (var dbcontext = new EFSapDbContext())
            {
                int count = dbcontext.DraftDocs.Where(x => x.U_peppolref.Equals(DocNum) && x.ObjType.Equals(objType)).Count();
                result = count > 0 ? true : false;
            }
            return result;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static void SendCustomerPEPPOLApprovalEMail(CustomerRegistration customerRegistration, string PEPPOLID)
        {
            try
            {
                string strSMTP_Server = ConfigurationManager.AppSettings["SMTP_Server"];
                string strSMTP_NC_User = ConfigurationManager.AppSettings["SMTP_NC_User"];
                string strSMTP_NC_Password = ConfigurationManager.AppSettings["SMTP_NC_Password"];
                string strFrom = ConfigurationManager.AppSettings["From"];
                string strCC_ApprovalEmail = ConfigurationManager.AppSettings["CC_ApprovalEmail"];

                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(customerRegistration.AuthorizedPersonnelEmail);
                mailMessage.CC.Add(strCC_ApprovalEmail);
                mailMessage.From = new MailAddress(strFrom);
                mailMessage.Subject = "Mail from PEPPOL Customer Registration Portal";
                mailMessage.Body = "Hi " + FirstCharToUpper(customerRegistration.AuthorizedPersonnelName) + ", <br/>" +
                                   "<p>Your PEPPOL registration completed, Successfully.<p>" +
                                   "<p>Please find your PEPPOL ID, below. Thanks.</p>" +
                                   "<p>PEPPOL ID:" + PEPPOLID;
                mailMessage.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient(strSMTP_Server);
                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;
                NetworkCredential nc = new NetworkCredential(strSMTP_NC_User, strSMTP_NC_Password);
                smtpClient.Credentials = nc;
                smtpClient.Send(mailMessage);
            }
            catch(Exception ex)
            {
                WriteLog(ex);
            }
        }
        public static void SendInvoiceEMail(string ToEmail, string InvoiceNo, string CompanyName, string DocType)
        {
            try
            {

                string strSMTP_Server = ConfigurationManager.AppSettings["SMTP_Server"];
                string strSMTP_NC_User = ConfigurationManager.AppSettings["SMTP_NC_User"];
                string strSMTP_NC_Password = ConfigurationManager.AppSettings["SMTP_NC_Password"];
                string strFrom = ConfigurationManager.AppSettings["From"];
                

                if (!ToEmail.Equals(string.Empty) && !InvoiceNo.Equals(string.Empty)) { 
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.To.Add(ToEmail);                
                    mailMessage.From = new MailAddress(strFrom);
                    mailMessage.Subject = "New " + DocType + " from "  + CompanyName;
                    mailMessage.Body = "Dear Customer, <br/>" +
                                       "<p>You have a New " + DocType + ", " + DocType + " No:" + InvoiceNo +
                                       " from " + CompanyName + " </p>" +
                                       "<br/><br/>Thanks & Regard,<br/><br/>Invoicing Team";  
                    mailMessage.IsBodyHtml = true;

                    SmtpClient smtpClient = new SmtpClient(strSMTP_Server);
                    smtpClient.Port = 587;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    NetworkCredential nc = new NetworkCredential(strSMTP_NC_User, strSMTP_NC_Password);
                    smtpClient.Credentials = nc;
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
        }
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
