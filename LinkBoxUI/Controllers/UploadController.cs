using LinkBoxUI.SessionChecker;
using DomainLayer.ViewModels;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using Newtonsoft.Json;

namespace LinkBoxUI.Controllers
{
    [SessionCheck]
    public class UploadController : Controller
    {
        private readonly IUploadRepository _uploadrepo;
        private readonly IGlobalRepository _globalrepo;

        public UploadController(IUploadRepository upload, IGlobalRepository global)
        {
            _uploadrepo = upload;
            _globalrepo = global;
        }

        LinkboxDb db = new LinkboxDb();
        // GET: Upload
        public ActionResult Uploader()
        {
            Session["ModuleAccess"] = 10;
            return View(_uploadrepo.View_Upload());
        }
        public JsonResult GetModule(int Id)
        {
            return Json(_globalrepo.GetModules(Id));
        }

        public JsonResult GetDetails(string MapCode)
        {       
            return Json(_uploadrepo.Get_Details(MapCode));
        }

        public JsonResult TryExecute(string MapCode)
        {

            string message = "";
            DataTable dt = _uploadrepo.GetData(MapCode);
            if (dt == null)
            {
                return Json(new { message, success = false, JsonRequestBehavior.AllowGet });
            }
            else
            {
                var JSONresult = JsonConvert.SerializeObject(dt);
                return Json(new { JSONresult, success = true, JsonRequestBehavior.AllowGet });
            }
        }

        public JsonResult Upload(string MapCode)
        {
            return Json(_uploadrepo.Upload(MapCode));
        }
        //public JsonResult GetDeposit()
        //{

        //    return Json(_uploadrepo.Get_Deposit());
        //}
    }
}