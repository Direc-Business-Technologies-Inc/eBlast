using CrystalDecisions.CrystalReports.Engine;
using DataCipher;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace LinkBoxUI.Helpers
{
    public class EmailHelpers
    {
        public SqlHelpers sql = new SqlHelpers();
        public string Send(EmailViewModel creds)
        {
            try
            {


                //if (creds.EmailCreds.Body.Contains("ATTACHMENT"))
                //{
                if (creds.ToTable != null && creds.ToTable.Rows.Count != 0)
                {
                    var columns = creds.ToTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                    foreach (DataRow item in creds.ToTable.Rows)
                    {
                        SmtpClient client = new SmtpClient(creds.EmailCreds.EmailHost);
                        //If you need to authenticate
                        var password = Cryption.Decrypt($"{creds.EmailCreds.EmailPassword}");
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(creds.EmailCreds.EmailFrom, password.Replace(creds.EmailCreds.EmailFrom, ""));
                        client.Port = Convert.ToInt32(creds.EmailCreds.EmailPort);
                        client.EnableSsl = true;
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(creds.EmailCreds.EmailFrom, creds.EmailCreds.EmailDesc);
                        //need to enhance
                        mailMessage.To.Add(new MailAddress(item[$@"{columns.Where(x => x.ToLower().Contains("mail")).FirstOrDefault()}"].ToString()));
                        //mailMessage.To.Add(new MailAddress("jameszeta1@gmail.com"));
                        if (creds.CcTable != null && creds.CcTable.Rows.Count != 0)
                        {
                            try
                            {
                                var cccolumns = creds.CcTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                                var ccs = creds.CcTable.Rows.Cast<DataRow>().Where(x => x[$@"{cccolumns.Where(y => y.ToLower().Contains("to")).FirstOrDefault()}"].ToString() == item[$@"{columns.Where(y => y.ToLower().Contains("mail")).FirstOrDefault()}"].ToString()).ToList();
                                foreach (DataRow cc in ccs)
                                {
                                    mailMessage.CC.Add(new MailAddress(cc[$@"{cccolumns.Where(x => x.ToLower().Contains("cc")).FirstOrDefault()}"].ToString()));
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (!string.IsNullOrEmpty(creds.EmailCreds.EmailCc))
                        {
                            foreach (var cc in creds.EmailCreds.EmailCc.Split(','))
                            {
                                mailMessage.CC.Add(new MailAddress(cc.ToString()));
                                //mailMessage.To.Add(new MailAddress("direc.danilo@gmail.com"));
                            }
                        }
                        var subject = creds.EmailCreds.EmailSubject;
                        var ebody = creds.EmailCreds.Body;
                        try { mailMessage.Subject = subject.Replace("#CUSTOMERNAME#", item["CardName"].ToString()); } catch (Exception ex) { mailMessage.Subject = subject; }
                        try { mailMessage.Subject = mailMessage.Subject.Replace("#CUSTOMERNAME#", item["CardCode"].ToString()); } catch (Exception ex) { mailMessage.Subject = mailMessage.Subject; }
                        mailMessage.Body = ebody.Replace("#ATTACHMENT#", "");
                        mailMessage.IsBodyHtml = true;
                        try { mailMessage.Body = mailMessage.Body.Replace("#CUSTOMERNAME#", item["CardName"].ToString()); } catch (Exception ex) { mailMessage.Body = mailMessage.Body; }

                        try
                        {
                            if (ebody.Contains("QUERY"))
                            {
                                var query = creds.QueryDetails.QueryString;
                                query = query.Replace("#CARDCODE#", item["CardCode"].ToString());
                                creds.QueryTable = sql.Fill_DataTable(creds, query);

                                StringBuilder fields = new StringBuilder();

                                foreach (DataRow row in creds.QueryTable.Rows)
                                {
                                    fields.AppendLine($"<tr>");

                                    foreach (DataColumn col in creds.QueryTable.Columns)
                                    {
                                        fields.AppendLine($"<td>{row[col.ColumnName].ToString()}</td>");
                                    }

                                    fields.AppendLine($"</tr>");

                                }
                                var headearFields = new StringBuilder();


                                foreach (DataColumn col in creds.QueryTable.Columns)
                                {
                                    headearFields.AppendLine($"<th>{col.ColumnName}</th>");
                                }

                                var body = $@"<table style ='text-align:center;width:100%;height: 50px;' border ='1' >
                                                            <tr>
                                                                {headearFields.ToString()}
                                                            </tr>
                                                            <tbody>
                                                                {fields.ToString()}
                                                            </tbody>
                                                            </table>
                                                            <br/><br/>";

                                mailMessage.Body = mailMessage.Body.Replace("#EMAILQUERY#", body);
                                mailMessage.IsBodyHtml = true;
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        var attachmentPath = "";
                        //if (creds.EmailCreds.FileName.ToLower().Contains("rpt"))
                        //{
                        //    ReportDocument cryRpt = new ReportDocument();
                        //    cryRpt.Load(creds.EmailCreds.FilePath);
                        //    cryRpt.Refresh();
                        //    cryRpt.SetDatabaseLogon(creds.FileCredentials.SapUser, creds.FileCredentials.SapPassword, creds.FileCredentials.ServerName, creds.FileCredentials.DbName);
                        //    cryRpt.SetParameterValue("CardCode", item["CardCode"].ToString());
                        //    attachmentPath = $@"{creds.EmailCreds.SavePath}{item["CardCode"].ToString()}_{DateTime.Now.ToShortDateString().ToString().Replace("/", "")}.pdf";
                        //    cryRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, attachmentPath);
                        //    Attachment attachment = new Attachment(attachmentPath);
                        //    mailMessage.Attachments.Add(attachment);
                        //    mailMessage.IsBodyHtml = true;
                        //}
                        //else
                        //{
                        //    if (!string.IsNullOrEmpty(creds.EmailCreds.FilePath))
                        //    {
                        //        Attachment attachment = new Attachment(creds.EmailCreds.FilePath);
                        //        mailMessage.Attachments.Add(attachment);
                        //        mailMessage.IsBodyHtml = true;
                        //    }
                        //}

                        var datetoday = DateTime.Now;

                        mailMessage.Body = mailMessage.Body.Replace("#MM#", datetoday.ToString("MM"));
                        mailMessage.Body = mailMessage.Body.Replace("#DD#", datetoday.ToString("dd"));
                        mailMessage.Body = mailMessage.Body.Replace("#YYYY#", datetoday.ToString("yyyy"));

                        var duedate = DateTime.Now.AddDays(7);
                        mailMessage.Body = mailMessage.Body.Replace("#DUEMM#", datetoday.ToString("MM"));
                        mailMessage.Body = mailMessage.Body.Replace("#DUEDD#", datetoday.ToString("dd"));
                        mailMessage.Body = mailMessage.Body.Replace("#DUEYYYY#", datetoday.ToString("yyyy"));

                        if (creds.CompanyDetails != null)
                        {
                            //mailMessage.Body = mailMessage.Body.Replace("#LOGO#", att.ContentId);

                            mailMessage.Body = mailMessage.Body.Replace("#MOBILENO#", creds.CompanyDetails.MobileNo);
                            mailMessage.Body = mailMessage.Body.Replace("#TELNO#", creds.CompanyDetails.TelNo);
                            mailMessage.Body = mailMessage.Body.Replace("#COMPANYNAME#", creds.CompanyDetails.CompanyName);
                            mailMessage.Body = mailMessage.Body.Replace("#COMPANYADDRESS#", creds.CompanyDetails.Address);
                            //mailMessage.AlternateViews.Add(GetEmbeddedImage(creds.CompanyDetails.FilePath, mailMessage.Body));
                        }


                        client.Send(mailMessage);
                    }
                }
                else
                {
                    return "No Data Fetch.";
                }


                //}
                //else
                //{


                //    SmtpClient client = new SmtpClient(creds.EmailCreds.EmailHost);
                //    //If you need to authenticate
                //    var password = Cryption.Decrypt($"{creds.EmailCreds.EmailPassword}");
                //    client.UseDefaultCredentials = false;
                //    client.Credentials = new NetworkCredential(creds.EmailCreds.EmailFrom, password);
                //    client.Port = Convert.ToInt32(creds.EmailCreds.EmailPort);
                //    client.EnableSsl = true;
                //    MailMessage mailMessage = new MailMessage();
                //    mailMessage.From = new MailAddress(creds.EmailCreds.EmailFrom);
                //    foreach (var cc in creds.EmailCreds.EmailTo.Split(','))
                //    {
                //        mailMessage.To.Add(new MailAddress(cc));
                //        //mailMessage.To.Add(new MailAddress("direc.danilo@gmail.com"));
                //    }
                //    foreach (var cc in creds.EmailCreds.EmailCc.Split(','))
                //    {
                //        mailMessage.CC.Add(new MailAddress(cc.ToString()));
                //    }

                //    if (creds.QueryTable != null && creds.QueryTable.Rows.Count != 0)
                //    {

                //        mailMessage.Subject = creds.EmailCreds.EmailSubject;
                //        if (creds.EmailCreds.Body.Contains("QUERY"))
                //        {


                //            StringBuilder fields = new StringBuilder();

                //            foreach (DataRow item in creds.QueryTable.Rows)
                //            {
                //                fields.AppendLine($"<tr>");

                //                foreach (DataColumn col in creds.QueryTable.Columns)
                //                {
                //                    fields.AppendLine($"<td>{item[col.ColumnName].ToString()}</td>");
                //                }

                //                fields.AppendLine($"</tr>");

                //            }
                //            var headearFields = new StringBuilder();


                //            foreach (DataColumn col in creds.QueryTable.Columns)
                //            {
                //                headearFields.AppendLine($"<th>{col.ColumnName}</th>");
                //            }

                //            var body = $@"<table style ='text-align:center;width:100%;height: 50px;' border ='1' >
                //                                <tr>
                //                                    {headearFields.ToString()}
                //                                </tr>
                //                                <tbody>
                //                                    {fields.ToString()}
                //                                </tbody>
                //                                </table>
                //                                <br/><br/>";

                //            mailMessage.Body = creds.EmailCreds.Body.Replace("#EMAILQUERY#", body);
                //            //mailMessage.Body += "<footer>This email was generated automatically." +
                //            //    "Please do NOT reply.<br/> This message was sent to specific and responsible people and intended for these people only.<br/>" +
                //            //    "If this was sent to any unrelated people, please delete it immediately.Thank you</footer>";

                //            mailMessage.IsBodyHtml = true;
                //            client.Send(mailMessage);
                //        }

                //    }

                //}
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public AlternateView GetEmbeddedImage(String filePath, string body)
        {
            LinkedResource res = new LinkedResource(filePath, MediaTypeNames.Image.Jpeg);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = body.Replace("#LOGO#", res.ContentId);
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

    }
}