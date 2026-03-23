using DataAccessLayer.Class;
using DomainLayer.ViewModels;
using LinkBoxUI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkBoxUI.Controllers.api
{
    [RoutePrefix("Linkbox")]
    public class GetController : ApiController
    {
        public SqlHelpers sql = new SqlHelpers();
        public SAPAccess sapAces = new SAPAccess();

        [HttpGet]
        [Route("sap/get")]
        public HttpResponseMessage GetData(AuthenticationCredViewModel auth)
        {
            bool blnLogin = false;
            var response = new HttpResponseMessage();
            sapAces.SaveCredentials(auth);
            blnLogin = SAPAccess.LoginAction();
            if (blnLogin == true)
            {
                string ret = sapAces.SendSLData(auth);
                JObject json = JObject.Parse(ret);
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(json.ToString(), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("Unauthorized Login Credential");
            }

            return response;
        }
    }
}
