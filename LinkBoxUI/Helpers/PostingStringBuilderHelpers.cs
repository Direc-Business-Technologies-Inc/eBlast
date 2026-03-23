using DataAccessLayer.Class;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LinkBoxUI.Helpers
{
    public class PostingStringBuilderHelpers
    {
        public static string BuildJsonForInvoice(PostingViewModel model, DataRow Header, DataTable Rows, DataColumnCollection column)
        {
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine($@" ""CardCode"" : ""MAIN"",");
            json.AppendLine($@" ""U_Remarks"" : ""Uploaded by LinkBox {DateTime.Now}"",");
            //Header
            foreach (DataColumn col in column)
            {
                var field = model.HeaderFields.Where(x => x.AddonField == col.ColumnName).FirstOrDefault();
                if (field != null && col.ColumnName.ToLower().Contains("date"))
                {
                    var docdate = DateTime.ParseExact(Header[field.AddonField].ToString(), "MM/dd/yyyy", null);
                    json.AppendLine($@" ""{field.SAPFieldId}"" : ""{Convert.ToDateTime(docdate).ToString("yyyy-MM-dd")}"",");
                }
                else if (field != null)
                {
                    json.AppendLine($@" ""{field.SAPFieldId}"" : ""{Header[field.AddonField].ToString()}"",");
                }

            }

            //Rows

            json.AppendLine($@" ""DocumentLines"" : [");
            ;
            foreach (DataRow row in Rows.Rows)
            {
                json.AppendLine("   {");
                foreach (DataColumn col in Rows.Columns)
                {
                    var field = model.RowFields.Where(x => x.AddonField == col.ColumnName).FirstOrDefault();
                    if (field != null && col.ColumnName.ToLower().Contains("price"))
                    {
                        var discount = string.IsNullOrEmpty(row[model.RowFields.Where(x => x.AddonField.ToLower().Contains("discount")).FirstOrDefault().AddonField].ToString()) ? 0 :
                            Convert.ToDouble(Regex.Replace(row[model.RowFields.Where(x => x.AddonField.ToLower().Contains("discount")).FirstOrDefault().AddonField].ToString(), "[^0-9.]", ""));
                        var price = Convert.ToDouble(row[field.AddonField].ToString()) -
                            ((Convert.ToDouble(row[field.AddonField].ToString()) / 100) * discount);

                        json.AppendLine($@" ""PriceAfterVAT"" : ""{price}"",");

                    }
                    else if (field != null && col.ColumnName.ToLower().Contains("discount"))
                    {
                        var discount = string.IsNullOrEmpty(row[field.AddonField].ToString()) ? "0" : row[field.AddonField].ToString().Replace("%", "");
                        json.AppendLine($@" ""DiscountPercent"" : ""{discount}"",");
                        json.AppendLine($@" ""FreeText"" : ""{row[field.AddonField].ToString()}"",");

                    }
                    else if (field != null && !col.ColumnName.ToLower().Contains("return") && !column.Contains(col.ColumnName))
                    {
                        json.AppendLine($@" ""{field.SAPFieldId}"" : ""{row[field.AddonField].ToString()}"",");
                    }

                }
                json.AppendLine("   },");
            }
            json.AppendLine("]");
            json.AppendLine("}");
            return json.ToString();
        }

        public static string BuildJsonForMeMo(PostingViewModel model, DataRow Header, DataTable Rows, DataColumnCollection column, string DocEntry)
        {
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine($@" ""CardCode"" : ""MAIN"",");
            json.AppendLine($@" ""U_Remarks"" : ""Uploaded by LinkBox {DateTime.Now}"",");
            //Header
            foreach (DataColumn col in column)
            {
                var field = model.HeaderFields.Where(x => x.AddonField == col.ColumnName).FirstOrDefault();
                if (field != null && col.ColumnName.ToLower().Contains("date"))
                {
                    var docdate = DateTime.ParseExact(Header[field.AddonField].ToString(), "MM/dd/yyyy", null);
                    json.AppendLine($@" ""{field.SAPFieldId}"" : ""{Convert.ToDateTime(docdate).ToString("yyyy-MM-dd")}"",");
                }
                else if (field != null)
                {
                    json.AppendLine($@" ""{field.SAPFieldId}"" : ""{Header[field.AddonField].ToString()}"",");
                }

            }

            //Rows

            json.AppendLine($@" ""DocumentLines"" : [");

            foreach (DataRow row in Rows.Rows)
            {
                json.AppendLine("   {");
                foreach (DataColumn col in Rows.Columns)
                {


                    var field = model.RowFields.Where(x => x.AddonField == col.ColumnName).FirstOrDefault();
                    if (field != null && col.ColumnName.ToLower().Contains("price"))
                    {
                        //var discount = string.IsNullOrEmpty(row[model.RowFields.Where(x => x.AddonField.ToLower().Contains("discount")).FirstOrDefault().AddonField].ToString()) ? 0 :
                        //    Convert.ToDouble(row[model.RowFields.Where(x => x.AddonField.ToLower().Contains("discount")).FirstOrDefault().AddonField].ToString().Replace("%", ""));
                        //var price = Convert.ToDouble(row[field.AddonField].ToString()) -
                        //    ((Convert.ToDouble(row[field.AddonField].ToString()) / 100) * discount);

                        json.AppendLine($@" ""PriceAfterVAT"" : ""{row[field.AddonField].ToString()}"",");
                    }
                    else if (field != null && !col.ColumnName.ToLower().Contains("sold") && !column.Contains(col.ColumnName))
                    {
                        json.AppendLine($@" ""{field.SAPFieldId}"" : ""{row[field.AddonField].ToString()}"",");
                    }

                }
                //json.AppendLine($@" ""BaseEntry"" : ""{DocEntry}"",");
                //json.AppendLine($@" ""BaseType"" : ""13"",");
                json.AppendLine("   },");
            }
            json.AppendLine("]");
            json.AppendLine("}");
            return json.ToString();
        }

        public static string BuildJsonForPayment(PostingViewModel model, DataRow Header, DataTable Cash, DataTable Bank, DataColumnCollection column, string PaymentRef, string RefundRef,string paymentsum,string refundsum)
        {
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine($@" ""CardCode"" : ""MAIN"",");
            json.AppendLine($@" ""U_Remarks"" : ""Uploaded by LinkBox {DateTime.Now}"",");
            //Header
            foreach (DataColumn col in column)
            {
                var field = model.HeaderFields.Where(x => x.AddonField == col.ColumnName).FirstOrDefault();
                if (field != null && col.ColumnName.ToLower().Contains("date"))
                {
                    var docdate = DateTime.ParseExact(Header[field.AddonField].ToString(), "MM/dd/yyyy", null);
                    json.AppendLine($@" ""{field.SAPFieldId}"" : ""{Convert.ToDateTime(docdate).ToString("yyyy-MM-dd")}"",");
                }
                else if (field != null)
                {
                    //var value = field.SAPField.ToLower().Contains("docdate") ? Convert.ToDateTime(Header[field.AddonField].ToString()).ToString("yyyy-MM-dd") : Header[field.AddonField].ToString();
                    //if(field.SAPField.ToLower().Contains("docdate"))
                    //{
                    //     x = Convert.ToDateTime(Header[field.AddonField].ToString()).ToString("yyyy-MM-dd");
                    //}
                    json.AppendLine($@" ""{field.SAPFieldId}"" : ""{Header[field.AddonField].ToString()}"",");
                }

            }
            if (Cash.Rows.Count != 0)
            {
                json.AppendLine($@" ""CashAccount"" : ""110130"",");
                json.AppendLine($@" ""CashSum"" : ""{Cash.Rows[0].ItemArray[0].ToString()}"",");
            }
            //Rows
            if (!string.IsNullOrEmpty(PaymentRef) || !string.IsNullOrEmpty(RefundRef))
            {
                json.AppendLine($@" ""PaymentInvoices"" : [");

                if (!string.IsNullOrEmpty(PaymentRef))
                {
                    json.AppendLine("   {");
                    json.AppendLine($@" ""DocEntry"" : ""{PaymentRef}"",");
                    json.AppendLine($@" ""InvoiceType"" : ""it_Invoice"",");
                    json.AppendLine($@" ""SumApplied"" : ""{paymentsum}"",");
                    
                    json.AppendLine("   },");
                }
                if (!string.IsNullOrEmpty(RefundRef))
                {
                    json.AppendLine("   {");
                    json.AppendLine($@" ""DocEntry"" : ""{PaymentRef}"",");
                    json.AppendLine($@" ""InvoiceType"" : ""it_CredItnote"",");
                    json.AppendLine($@" ""SumApplied"" : ""{refundsum}"",");
                    json.AppendLine("   },");
                }

                json.AppendLine("],");
            }
            if (Bank.Rows.Count != 0)
            {

                json.AppendLine($@" ""PaymentCreditCards"" : [");

                foreach (DataRow row in Bank.Rows)
                {
                    json.AppendLine("   {");
                    json.AppendLine($@" ""CreditCard"" : ""1"",");
                    json.AppendLine($@" ""CreditAcct"" : ""110250"",");
                    json.AppendLine($@" ""CardValidUntil"" : ""2021-12-31"",");
                    json.AppendLine($@" ""CreditSum"" : ""{row["PaymentAmount"].ToString()}"",");
                    json.AppendLine($@" ""CreditCur"" : ""PHP"",");
                    json.AppendLine($@" ""VoucherNum"" : ""1"",");
                    json.AppendLine("   },");
                }
                json.AppendLine("]");
            }
            json.AppendLine("}");
            return json.ToString();
        }

        public static string BuildJsonAPIPost(DataTable headerdata, DataTable rowsdata, List<PostingViewModel.Fields> DataHeader, List<PostingViewModel.Fields> DataRows)
        {
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine($@" ""DocObjectCode"": ""17"",");
            json.AppendLine($@" ""DocDate"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",");
            json.AppendLine($@" ""DocDueDate"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",");
            json.AppendLine($@" ""CardCode"" : ""CDIST5"",");  //C00000
            json.AppendLine($@" ""CardName"" : ""DKT Shopify"",");
            json.AppendLine($@" ""U_Remarks"" : ""Uploaded by LinkBox {DateTime.Now}"",");
            //Header
            foreach (var head in DataHeader)
            {
                if (ColumnExist(headerdata, head.AddonField) && !string.IsNullOrEmpty(head.SAPFieldId))
                {
                    json.AppendLine($@" ""{head.SAPFieldId}"" : ""{headerdata.Rows[0][$@"{head.AddonField}"].ToString()}"",");
                }                
            }

            //Rows
            json.AppendLine($@" ""DocumentLines"" : [");
            foreach (DataRow dr in rowsdata.Rows)
            {
                json.AppendLine("   {");
                foreach (var rows in DataRows)
                {
                    if (ColumnExist(rowsdata, rows.AddonField) && !string.IsNullOrEmpty(rows.SAPFieldId) && rows.AddonField != "sku")
                    {
                        json.AppendLine($@" ""{rows.SAPFieldId}"" : ""{dr[$@"{rows.AddonField}"].ToString()}"",");
                    }
                    else if (rows.AddonField == "sku")
                    {
                        json.AppendLine($@" ""{rows.SAPFieldId}"" : ""61041"","); //ITSH-02483 sample only for dbti
                    }
                }
                json.AppendLine("   },");
            }            
            json.AppendLine("]");
            json.AppendLine("}");         

            return json.ToString();
        }

        public static bool ColumnExist(DataTable data, string ColumnName)
        {
            try
            {                
                return data.Rows[0][$@"{ColumnName}"].ToString().Any();
            }
            catch
            {
                return false;
            }
        }
    }
}