using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DomainLayer.ViewModels;
using LinkBoxUI.Helpers;
using MSXML2;
using Newtonsoft.Json.Linq;

namespace LinkBoxUI.Helpers
{
    public static class PostingHelpers
    {
        public static XMLHTTP60 ServiceLayer = new XMLHTTP60();
        public static SqlHelpers sql = new SqlHelpers();
        public static bool LoginAction(PostingViewModel Creds)
        {
            try
            {

                string err;
                bool result = true;
                var json = new StringBuilder();
                json.AppendLine("{");
                json.AppendLine($@" ""CompanyDB"" : ""{Creds.CredentialDetails.SAPDBName}"",");
                json.AppendLine($@" ""UserName"" : ""{Creds.CredentialDetails.SAPUser}"",");
                json.AppendLine($@" ""Password"" : ""{Creds.CredentialDetails.SAPPassword}""");
                json.AppendLine("}");

                ServiceLayer.open("POST", $@"{ServiceURL(Creds.CredentialDetails.SAPIPAddress, Creds.CredentialDetails.SAPLicensePort.ToString())}Login");
                //ServiceLayer.setOption(SERVERXMLHTTP_OPTION.SXH_OPTION_IGNORE_SERVER_SSL_CERT_ERROR_FLAGS, 13056);
                try
                {
                    ServiceLayer.send(json.ToString());
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    result = false;
                    return result;
                }

                string ret = GetJsonValue(ServiceLayer.responseText, "SessionId");

                if (string.IsNullOrEmpty(ret))
                {
                    err = GetJsonError(ServiceLayer.responseText);
                    result = false;
                }
                else
                {
                    result = ret.Contains("-");
                    err = ret;
                }

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static string ServiceURL(string Server,string Port)
        {
            var url = $"http://{Server}:{Port}/b1s/v1/";
            const string httpStr = "http://";
            const string httpsStr = "https://";
            if (!url.StartsWith(httpStr, true, null) &&
                !url.StartsWith(httpsStr, true, null))
            {
                url = httpStr + url;
            }

            if (ServiceLayer == null)
            { ServiceLayer = new XMLHTTP60(); }

            return url;
        }

        public static string GetJsonValue(string json, string value)
        {
            try
            {
                if (json != null)
                {
                    JObject err = JObject.Parse(json);
                    if (err.ToString().Contains("error"))
                    {
                        return $"error : {GetJsonError(err.ToString())}";
                    }
                    else
                    {
                        return (string)err[value];
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                if (json.Contains("error"))
                {
                    string retJson = GetJsonString(json, "");
                    var sbJson = new StringBuilder();
                    sbJson.Append("{" + retJson + "}}}");
                    return GetJsonError(sbJson.ToString());
                }
                else { return "Operation completed successfully"; }
            }
        }


        public static string GetJsonError(string json)
        {
            JObject err = JObject.Parse(json);
            return (string)err["error"]["message"]["value"];
        }

        public static string GetJsonString(string ret, string tag)
        {
            var startTag = "{";
            int startIndex = ret.IndexOf(startTag) + startTag.Length;
            int endIndex = ret.IndexOf("}", startIndex);
            return ret.Substring(startIndex, endIndex - startIndex);
        }

        public static string SBOResponse(string sMethod, string sModule, string sJson, string sRetValue, PostingViewModel Creds)
        {
            //var output = true;
            string output = "";
            string url = ServiceURL(Creds.CredentialDetails.SAPIPAddress, Creds.CredentialDetails.SAPLicensePort.ToString());
            ServiceLayer.open(sMethod, $"{url}{sModule}");
            //ServiceLayer.setOption(SERVERXMLHTTP_OPTION.SXH_OPTION_IGNORE_SERVER_SSL_CERT_ERROR_FLAGS, 13056);
            //ServiceLayer.setTimeouts(1000000, 1000000, 1000000, 1000000);
            if (sModule.Contains("Attachments2"))
            {
                ServiceLayer.setRequestHeader("Content-Type", "multipart/form-data");
            }

            try
            {
                ServiceLayer.send(sJson);
                var response = ServiceLayer.responseText;
                output = sMethod == "GET" ? response : GetJsonValue(response, sRetValue);
            }
            catch (Exception ex)
            {
                output = $"error : {ex.Message}";
            }
            return output;
        }
    }
}