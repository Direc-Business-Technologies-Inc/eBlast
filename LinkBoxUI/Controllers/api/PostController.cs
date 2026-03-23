using DataAccessLayer.Class;
using LinkBoxUI.Context;
using LinkBoxUI.Helpers;
using LinkBoxUI.Services;
using DomainLayer.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;

namespace LinkBoxUI.Controllers.api
{
    [RoutePrefix("Linkbox")]
    public class PostController : ApiController
    {
        public SqlHelpers sql = new SqlHelpers();
        public SAPAccess sapAces = new SAPAccess();
        LinkboxDb _context = new LinkboxDb();
        GlobalServices globalServices = new GlobalServices();
        EmailHelpers eml = new EmailHelpers();
        [HttpPost]
        [Route("Post")]
        public string Post()
        {
            bool status = false;
            string message = "";
            try
            {
                foreach (var task in TaskSchedulerHelpers.getRunningTask())
                {
                    
                    var EmailCreds = globalServices.GetEmailCredentials(task);

                    if (EmailCreds.EmailCreds != null)
                    {
                        var sendEmail = eml.Send(EmailCreds).ToLower();

                        if (!sendEmail.Contains("success"))
                        {
                            status = false;
                            message = sendEmail;
                        }
                        else
                        {
                            status = true;
                            message = sendEmail;
                        }
                    }
                }
                var response = new
                {
                    Message = message,
                    Status = status
                };

                return JsonConvert.SerializeObject(response);
            }
            catch (Exception ex)
            {
                var res = new
                {
                    Message = ex.Message,
                    Status = false
                };

                return JsonConvert.SerializeObject(res);
            }
        }


        [HttpPost]
        [Route("ManualPost")]
        public string ManualPost(string Code)
        {
            try
            {
                bool status = false;
                string message = "";
                var EmailCreds = globalServices.GetEmailCredentials(Code);

                if (EmailCreds.EmailCreds != null)
                {
                    var sendEmail = eml.Send(EmailCreds).ToLower();

                    if (!sendEmail.Contains("success"))
                    {
                        status = false;
                        message = sendEmail;
                    }
                    else
                    {
                        status = true;
                        message = sendEmail;
                    }
                }
                var response = new
                {
                    Message = message,
                    Status = status
                };

                return JsonConvert.SerializeObject(response);
            }
            catch (Exception ex)
            {
                var res = new
                {
                    Message = ex.Message,
                    Status = false
                };

                return JsonConvert.SerializeObject(res);
            }

        }

        [HttpPost]
        [Route("sap/login")]
        public bool Login(AuthenticationCredViewModel auth)
        {
            bool result = false;

            if (auth != null)
            {
                sapAces.SaveCredentials(auth);
                result = SAPAccess.LoginAction();
            }

            return result;
        }

        [HttpPost]
        [Route("sap/post")]
        public string PostSL(AuthenticationCredViewModel auth)
        {
            string ret = "";
            bool blnLogin = false;
            sapAces.SaveCredentials(auth);
            blnLogin = SAPAccess.LoginAction();

            if (blnLogin == true)
            {
                ret = sapAces.SendSLData(auth);
            }
            return ret;
        }

        [HttpPost]
        [Route("sap/post/documents")]
        public string PostDocument(int mapid)
        {
            string ret = "";
            var model = new MapCreateViewModel();
            ////GET MAPPING SETUP
            var _apimap = globalServices.GetAPIPostSAP(mapid);

            return ret;
        }
        [HttpPost]
        [Route("sap/post/items")]
        public string PostItemMaster(int mapid)
        {
            string ret = "";
            var model = new MapCreateViewModel();
            ////GET MAPPING SETUP
            var _apimap = globalServices.GetAPIPostSAP(mapid);

            return ret;
        }
    }
}
