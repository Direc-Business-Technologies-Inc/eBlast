
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DataAccessLayer.Class
{
    public class DataAccess
    {
        //Saving of added udf fields in created table
        //public static void AddHeaderUdf(int Check, List<string[]> TableFields, string TableName, int UserId, int MapId, int UpdateId)
        //{
        //    foreach (var item in TableFields)
        //    {
        //        Header header = new Header();
        //        header.MapId = Check == 1 ? MapId : Convert.ToInt32(UpdateId);
        //        header.TableName = TableName;
        //        header.SAPHeaderField = item[0].ToString();
        //        header.AddonHeaderField = item[1].ToString();
        //        header.DataType = item[2].ToString();
        //        header.Length = item[3].ToString();
        //        header.IsRequired = false;
        //        header.CreateDate = DateTime.Now;
        //        header.CreateUserID = UserId;
        //        //Header.Add(header);
        //    }
        //}

        //public static void AddRowUdf(int Check, List<string[]> TableFields, string TableName, int UserId, int MapId, int UpdateId)
        //{
        //    foreach (var item in TableFields)
        //    {
        //        Row row = new Row();
        //        row.MapId = Check == 1 ? MapId : Convert.ToInt32(UpdateId);
        //        row.TableName = TableName;
        //        row.SAPRowField = item[0].ToString();
        //        row.AddonRowField = item[1].ToString();
        //        row.DataType = item[2].ToString();
        //        row.Length = item[3].ToString();
        //        row.IsRequired = false;
        //        row.CreateDate = DateTime.Now;
        //        row.CreateUserID = UserId;
        //        //Row.Add(row);
        //    }
        //}

        public static DataTable Select(string ConString, string query)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    using (SqlConnection con = new SqlConnection(ConString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            con.Open();
                            da.Fill(dt);
                            con.Close();
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //frmMain.NotiMsg(ex.Message, Color.Red);
                return null;
            }
        }

        public static DataTable SelectHana(string ConString, string query)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    using (HanaConnection con = new HanaConnection(ConString))
                    {
                        using (HanaCommand cmd = new HanaCommand(query, con))
                        {
                            HanaDataAdapter da = new HanaDataAdapter(cmd);
                            con.Open();
                            da.Fill(dt);
                            con.Close();
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //frmMain.NotiMsg(ex.Message, Color.Red);
                return null;
            }
        }

        public static Boolean Execute(string ConString, string query)
        {
            Boolean _bool = false;
            try
            {

                using (SqlConnection con = new SqlConnection(ConString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd = con.CreateCommand();
                    con.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    _bool = true;
                }
            }
            catch (Exception ex)
            {
                //frmMain.NotiMsg(ex.Message, Color.Red);
            }
            return _bool;
        }


        public static void SaveToFTP(MemoryStream stream, string FilePath, string user, string pass)
        {
            WebRequest request = WebRequest.Create(FilePath);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(user, pass);
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch(Exception ex)
            {

            }
            
            request.Abort();
            request = WebRequest.Create(FilePath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(user, pass);
        
            using (Stream ftpStream = request.GetRequestStream())
            {
                stream.WriteTo(ftpStream);

                ftpStream.Close();
            }             
        }

        public static List<string> GetFileListFTP(string FilePath, string user, string pass)
        {
            try
            {
                WebRequest request = WebRequest.Create(FilePath);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user, pass);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

               return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Stream ReadFromFTP(string FilePath,string user,string pass)
        {
            WebClient request = new WebClient();
            request.Credentials = new NetworkCredential(user, pass);

            byte[] newFileData = request.DownloadData(FilePath);
            MemoryStream stream = new MemoryStream(newFileData);
                
            return stream;

        }

        public string ConcatDecimal(string value)
        {
            if (value == "" || value == string.Empty || value == null)
            {
                return "0";
            }
            else if (value.Contains('.'))
            {
                var number = value.Split('.');
                var r = new Regex($@"^\d*[1-9]\d*$");
                if (r.IsMatch(number[1]))
                {
                    return string.Format(new NumberFormatInfo() { NumberDecimalDigits = 2 },
                        "{0:F}", new decimal(Convert.ToDouble(value))).ToString();
                }
                else
                {
                    return number[0];
                }
            }
            else
            {
                return value;
            }


        }
    }
}