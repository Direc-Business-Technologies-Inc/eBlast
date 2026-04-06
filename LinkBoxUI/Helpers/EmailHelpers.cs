using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DataAccessLayer.Class;
using DataCipher;
using DomainLayer.ViewModels;
using LinkBoxUI.Services;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace LinkBoxUI.Helpers
{
    public class EmailHelpers
    {
        public SqlHelpers sql = new SqlHelpers();
        GlobalServices globalServices = new GlobalServices();
        public string Send(EmailViewModel creds)
        {
            try
            {
                if (creds.ToTable != null && creds.ToTable.Rows.Count != 0)
                {
                    List<string> columns = (from DataColumn x in creds.ToTable.Columns
                                            select x.ColumnName).ToList();
                    foreach (DataRow item in creds.ToTable.Rows)
                    {
                        var safas = item.ItemArray[0];

                        SmtpClient smtpClient = new SmtpClient(creds.EmailCreds.EmailHost);
                        string text = Cryption.Decrypt($"{creds.EmailCreds.EmailPassword}");
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(creds.EmailCreds.EmailFrom, text.Replace(creds.EmailCreds.EmailFrom, ""));
                        smtpClient.Port = Convert.ToInt32(creds.EmailCreds.EmailPort);
                        smtpClient.EnableSsl = true;
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(creds.EmailCreds.EmailFrom, creds.EmailCreds.EmailDesc);
                        mailMessage.To.Add(new MailAddress(item[string.Format("{0}", columns.Where((string x) => x.ToLower().Contains("mail")).FirstOrDefault())].ToString()));
                        if (creds.CcTable != null && creds.CcTable.Rows.Count != 0)
                        {
                            try
                            {
                                List<string> cccolumns = (from DataColumn x in creds.CcTable.Columns
                                                          select x.ColumnName).ToList();
                                List<DataRow> list = (from DataRow x in creds.CcTable.Rows
                                                      where x[string.Format("{0}", cccolumns.Where((string y) => y.ToLower().Contains("to")).FirstOrDefault())].ToString() == item[string.Format("{0}", columns.Where((string y) => y.ToLower().Contains("mail")).FirstOrDefault())].ToString()
                                                      select x).ToList();
                                foreach (DataRow item3 in list)
                                {
                                    mailMessage.CC.Add(new MailAddress(item3[string.Format("{0}", cccolumns.Where((string x) => x.ToLower().Contains("cc")).FirstOrDefault())].ToString()));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        if (!string.IsNullOrEmpty(creds.EmailCreds.EmailCc))
                        {
                            string[] array = creds.EmailCreds.EmailCc.Split(',');
                            foreach (string text2 in array)
                            {
                                mailMessage.CC.Add(new MailAddress(text2.ToString()));
                            }
                        }
                        string emailSubject = creds.EmailCreds.EmailSubject;
                        string body = creds.EmailCreds.Body;
                        try
                        {
                            mailMessage.Subject = emailSubject.Replace("#CUSTOMERNAME#", item["CardName"].ToString());
                        }
                        catch (Exception)
                        {
                            mailMessage.Subject = emailSubject;
                        }
                        try
                        {
                            mailMessage.Subject = mailMessage.Subject.Replace("#CUSTOMERNAME#", item["CardCode"].ToString());
                        }
                        catch (Exception)
                        {
                            mailMessage.Subject = mailMessage.Subject;
                        }
                        mailMessage.Body = body.Replace("#ATTACHMENT#", "");
                        mailMessage.IsBodyHtml = true;
                        try
                        {
                            mailMessage.Body = mailMessage.Body.Replace("#CUSTOMERNAME#", item["CardName"].ToString());
                        }
                        catch (Exception)
                        {
                            mailMessage.Body = mailMessage.Body;
                        }
                        try
                        {
                            if (body.Contains("QUERY"))
                            {
                                string queryString = creds.QueryDetails.QueryString;
                                queryString = queryString.Replace("#CARDCODE#", item["CardCode"].ToString());
                                creds.QueryTable = sql.Fill_DataTable(creds, queryString);
                                StringBuilder stringBuilder = new StringBuilder();
                                foreach (DataRow row in creds.QueryTable.Rows)
                                {
                                    stringBuilder.AppendLine("<tr>");
                                    foreach (DataColumn column in creds.QueryTable.Columns)
                                    {
                                        stringBuilder.AppendLine($"<td>{row[column.ColumnName].ToString()}</td>");
                                    }
                                    stringBuilder.AppendLine("</tr>");
                                }
                                StringBuilder stringBuilder2 = new StringBuilder();
                                foreach (DataColumn column2 in creds.QueryTable.Columns)
                                {
                                    stringBuilder2.AppendLine($"<th>{column2.ColumnName}</th>");
                                }
                                string newValue = $"<table style ='text-align:center;width:100%;height: 50px;' border ='1' >\r\n                                                            <tr>\r\n                                                                {stringBuilder2.ToString()}\r\n                                                            </tr>\r\n                                                            <tbody>\r\n                                                                {stringBuilder.ToString()}\r\n                                                            </tbody>\r\n                                                            </table>\r\n                                                            <br/><br/>";
                                mailMessage.Body = mailMessage.Body.Replace("#EMAILQUERY#", newValue);
                                mailMessage.IsBodyHtml = true;
                            }
                        }
                        catch (Exception)
                        {
                        }
                        string text3 = "";
                        ReportDocument reportDocument = new ReportDocument();
                        if (creds.EmailCreds.FileName.ToLower().Contains("rpt"))
                        {
                            reportDocument.Load(creds.EmailCreds.FilePath);
                            reportDocument.Refresh();
                            reportDocument.SetDatabaseLogon(creds.FileCredentials.SapUser, creds.FileCredentials.SapPassword, creds.FileCredentials.ServerName, creds.FileCredentials.DbName);
                            reportDocument.SetParameterValue("CardCode", item["CardCode"].ToString());
                            text3 = string.Format("{0}{1}_{2}.pdf", creds.EmailCreds.SavePath, item["CardCode"].ToString(), DateTime.Now.ToString("yyyyMMddHHmmssFFFFF").Replace("/", ""));
                            reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, text3);
                            Attachment item2 = new Attachment(text3);
                            mailMessage.Attachments.Add(item2);
                            mailMessage.IsBodyHtml = true;
                        }
                        else if (!string.IsNullOrEmpty(creds.EmailCreds.FilePath))
                        {
                            Attachment item2 = new Attachment(creds.EmailCreds.FilePath);
                            mailMessage.Attachments.Add(item2);
                            mailMessage.IsBodyHtml = true;
                        }
                        DateTime now = DateTime.Now;
                        mailMessage.Body = mailMessage.Body.Replace("#MM#", now.ToString("MM"));
                        mailMessage.Body = mailMessage.Body.Replace("#DD#", now.ToString("dd"));
                        mailMessage.Body = mailMessage.Body.Replace("#YYYY#", now.ToString("yyyy"));
                        DateTime dateTime = DateTime.Now.AddDays(7.0);
                        mailMessage.Body = mailMessage.Body.Replace("#DUEMM#", now.ToString("MM"));
                        mailMessage.Body = mailMessage.Body.Replace("#DUEDD#", now.ToString("dd"));
                        mailMessage.Body = mailMessage.Body.Replace("#DUEYYYY#", now.ToString("yyyy"));
                        if (creds.CompanyDetails != null)
                        {
                            mailMessage.Body = mailMessage.Body.Replace("#MOBILENO#", creds.CompanyDetails.MobileNo);
                            mailMessage.Body = mailMessage.Body.Replace("#TELNO#", creds.CompanyDetails.TelNo);
                            mailMessage.Body = mailMessage.Body.Replace("#COMPANYNAME#", creds.CompanyDetails.CompanyName);
                            mailMessage.Body = mailMessage.Body.Replace("#COMPANYADDRESS#", creds.CompanyDetails.Address);
                            mailMessage.AlternateViews.Add(GetEmbeddedImage(creds.CompanyDetails.FilePath, mailMessage.Body));
                        }
                        smtpClient.Send(mailMessage);
                        reportDocument.Dispose();
                    }
                    return "Success";
                }
                return "No Data Fetch.";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string SendNew(EmailViewModel creds, string doc)
        {
            try
            {
                if (creds.ToTable != null && creds.ToTable.Rows.Count != 0)
                {
                    List<string> columns = (from DataColumn x in creds.ToTable.Columns
                                            select x.ColumnName).ToList();
                    foreach (DataRow item in creds.ToTable.Rows)
                    {
                        var docEntry = item["DocEntry"].ToString();

                        SmtpClient smtpClient = new SmtpClient(creds.EmailCreds.EmailHost);
                        string text = Cryption.Decrypt($"{creds.EmailCreds.EmailPassword}");
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(creds.EmailCreds.EmailFrom, text.Replace(creds.EmailCreds.EmailFrom, ""));
                        smtpClient.Port = Convert.ToInt32(creds.EmailCreds.EmailPort);
                        smtpClient.EnableSsl = true;
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(creds.EmailCreds.EmailFrom, creds.EmailCreds.EmailDesc);
                        mailMessage.To.Add(new MailAddress(item[string.Format("{0}", columns.Where((string x) => x.ToLower().Contains("mail")).FirstOrDefault())].ToString()));
                        if (creds.CcTable != null && creds.CcTable.Rows.Count > 0)
                        {
                            try
                            {
                                //string toColumn = creds.CcTable.Columns.Cast<DataColumn>()
                                //    .Select(c => c.ColumnName)
                                //    .FirstOrDefault(c => c.ToLower().Contains("to"));

                                //string mailColumn = columns.FirstOrDefault(c => c.ToLower().Contains("mail"));

                                var list = creds.CcTable.AsEnumerable().ToList();

                                foreach (DataRow row in list)
                                {
                                    mailMessage.CC.Add(new MailAddress(row["EmailCC"].ToString()));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        if (!string.IsNullOrEmpty(creds.EmailCreds.EmailCc))
                        {
                            string[] array = creds.EmailCreds.EmailCc.Split(',');
                            foreach (string text2 in array)
                            {
                                mailMessage.CC.Add(new MailAddress(text2.ToString()));
                            }
                        }
                        string emailSubject = creds.EmailCreds.EmailSubject;
                        string body = creds.EmailCreds.Body;

                        mailMessage.Subject = ApplyPlaceholders(emailSubject, item);

                        mailMessage.Body = ApplyPlaceholders(body, item);
                        try
                        {
                            if (body.Contains("QUERY"))
                            {
                                string queryString = creds.QueryDetails.QueryString;
                                queryString = queryString.Replace("#CARDCODE#", item["CardCode"].ToString());
                                creds.QueryTable = sql.Fill_DataTable(creds, queryString);
                                StringBuilder stringBuilder = new StringBuilder();
                                foreach (DataRow row in creds.QueryTable.Rows)
                                {
                                    stringBuilder.AppendLine("<tr>");
                                    foreach (DataColumn column in creds.QueryTable.Columns)
                                    {
                                        stringBuilder.AppendLine($"<td>{row[column.ColumnName].ToString()}</td>");
                                    }
                                    stringBuilder.AppendLine("</tr>");
                                }
                                StringBuilder stringBuilder2 = new StringBuilder();
                                foreach (DataColumn column2 in creds.QueryTable.Columns)
                                {
                                    stringBuilder2.AppendLine($"<th>{column2.ColumnName}</th>");
                                }
                                string newValue = $"<table style ='text-align:center;width:100%;height: 50px;' border ='1' >\r\n                                                            <tr>\r\n                                                                {stringBuilder2.ToString()}\r\n                                                            </tr>\r\n                                                            <tbody>\r\n                                                                {stringBuilder.ToString()}\r\n                                                            </tbody>\r\n                                                            </table>\r\n                                                            <br/><br/>";
                                mailMessage.Body = mailMessage.Body.Replace("#EMAILQUERY#", newValue);
                                mailMessage.IsBodyHtml = true;
                            }
                        }
                        catch (Exception)
                        {
                        }
                        string text3 = "";
                        ReportDocument reportDocument = new ReportDocument();
                        if (creds.EmailCreds.FileName.ToLower().Contains("rpt"))
                        {
                            reportDocument.Load(creds.EmailCreds.FilePath);
                            reportDocument.Refresh();
                            reportDocument.SetDatabaseLogon(creds.FileCredentials.SapUser, creds.FileCredentials.SapPassword, creds.FileCredentials.ServerName, creds.FileCredentials.DbName);
                            reportDocument.SetParameterValue("DocKey@", docEntry);
                            reportDocument.SetParameterValue("UserCode@", item["PrepBy"].ToString());
                            text3 = string.Format("{0}{1}_{2}.pdf", creds.EmailCreds.SavePath, item["CardCode"].ToString(), DateTime.Now.ToString("yyyyMMddHHmmssFFFFF").Replace("/", ""));
                            reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, text3);
                            Attachment item2 = new Attachment(text3);
                            mailMessage.Attachments.Add(item2);
                            mailMessage.IsBodyHtml = true;
                        }
                        else if (!string.IsNullOrEmpty(creds.EmailCreds.FilePath))
                        {
                            Attachment item2 = new Attachment(creds.EmailCreds.FilePath);
                            mailMessage.Attachments.Add(item2);
                            mailMessage.IsBodyHtml = true;
                        }
                        DateTime now = DateTime.Now;
                        mailMessage.Body = mailMessage.Body.Replace("#MM#", now.ToString("MM"));
                        mailMessage.Body = mailMessage.Body.Replace("#DD#", now.ToString("dd"));
                        mailMessage.Body = mailMessage.Body.Replace("#YYYY#", now.ToString("yyyy"));
                        DateTime dateTime = DateTime.Now.AddDays(7.0);
                        mailMessage.Body = mailMessage.Body.Replace("#DUEMM#", now.ToString("MM"));
                        mailMessage.Body = mailMessage.Body.Replace("#DUEDD#", now.ToString("dd"));
                        mailMessage.Body = mailMessage.Body.Replace("#DUEYYYY#", now.ToString("yyyy"));
                        //if (creds.CompanyDetails != null)
                        //{
                        //    mailMessage.Body = mailMessage.Body.Replace("#MOBILENO#", creds.CompanyDetails.MobileNo);
                        //    mailMessage.Body = mailMessage.Body.Replace("#TELNO#", creds.CompanyDetails.TelNo);
                        //    mailMessage.Body = mailMessage.Body.Replace("#COMPANYNAME#", creds.CompanyDetails.CompanyName);
                        //    mailMessage.Body = mailMessage.Body.Replace("#COMPANYADDRESS#", creds.CompanyDetails.Address);
                        //    mailMessage.AlternateViews.Add(GetEmbeddedImage(creds.CompanyDetails.FilePath, mailMessage.Body));
                        //}
                        smtpClient.Send(mailMessage);
                        
                        UpdateDocument(docEntry, doc);
                        
                        reportDocument.Dispose();
                    }
                    return "Success";
                }

                return "No Data Fetch.";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        string GetItemValue(DataRow row, string column, string fallback = "")
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value
                ? row[column].ToString()
                : fallback;
        }

        string ApplyPlaceholders(string template, DataRow item)
        {
            var placeholders = new Dictionary<string, string>
            {
                { "#CUSTOMERNAME#", GetItemValue(item, "CardName") },
                { "#SCSONO#",       GetItemValue(item, "SCSONO") },
                { "#SINO#",         GetItemValue(item, "SINO") },

                { "#ATTACHMENT#",         GetItemValue(item, "") },
            };

            foreach (var kv in placeholders)
                template = template.Replace(kv.Key, kv.Value);

            return template;
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

        public bool UpdateDocument(string docEntry, string doc)
        {
            try
            {
                var getConnectionAndQuery = globalServices.UpdateDocumentQuery(doc);

                var con = getConnectionAndQuery.SAPConnection;
                var query = getConnectionAndQuery.Query.Replace("@DocEntry", docEntry.ToString());

                return DataAccess.Execute(con, query);
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}