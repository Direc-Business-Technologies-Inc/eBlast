
using DomainLayer;
using DomainLayer.ViewModels;
using LinkBoxUI.SessionChecker;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using Newtonsoft.Json;
using DomainLayer.Models;
using System.Web;
using System.IO;

namespace LinkBoxUI.Controllers
{
    [SessionCheck]
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationRepository _configrepo;
        private readonly IMappingRepository _maprepo;
        private readonly IGlobalRepository _global;
        public ConfigurationController(IConfigurationRepository configrepo, IGlobalRepository global, IMappingRepository maprepo)
        {
            _configrepo = configrepo;
            _global = global;
            _maprepo = maprepo;
        }

        LinkboxDb db = new LinkboxDb();
        [SetupCheck]
        public ActionResult Setup()
        {
            Session["ModuleAccess"] = 3;
            ViewBag.Title = "Setup";
            return View(_configrepo.View_Setup());
        }
        [FieldMappingCheck]
        public ActionResult MappingConfig()
        {
            Session["ModuleAccess"] = 4;
            return View(_maprepo.View_Mapping());
        }

        public ActionResult QueryManager()
        {
            Session["ModuleAccess"] = 10;
            return View(_maprepo.View_Query());
        }

        [SetupCheck]
        public JsonResult CreateAddon(SetupCreateViewModel.AddonViewModel model)
        {
            AddonSetup addon = AutoMapper.Mapper.Map<SetupCreateViewModel.AddonViewModel, AddonSetup>(model);
            _configrepo.Create_AddonSetup(addon, Convert.ToInt32(Session["Id"]));
            _configrepo.SaveChanges();
            return Json(true);
        }

        public JsonResult FindAddon(int Id)
        {
            return Json(_configrepo.Find_Addon(Id));
        }

        public JsonResult FindConnectionString(string Type)
        {
            return Json(_configrepo.Find_ConnectionString(Type));
        }

        public JsonResult ExecuteQuery(string Type, string ConString, string Query)
        {
            var model = _configrepo.GetConnection(Type, ConString);

            string message = "Please contact the administrator";
            DataTable dt = _configrepo.Fill_DataTable(model, Query);

            if (dt == null)
            {
                return Json(new { message, success = false, JsonRequestBehavior.AllowGet });
            }
            else
            {
                var JSONresult = JsonConvert.SerializeObject(dt);
                return Json(new { JSONresult, success = true, JsonRequestBehavior.AllowGet });
            }

            //return Json(null);
        }

        public JsonResult CreateQuery(QueryManagerViewModel.QueryManager model)
        {
            QueryManager query = AutoMapper.Mapper.Map<QueryManagerViewModel.QueryManager, QueryManager>(model);
            _configrepo.Create_Query(query, Convert.ToInt32(Session["Id"]));
            _configrepo.SaveChanges();
            return Json(true);
        }

        public JsonResult FindQuery(int Id)
        {
            return Json(_configrepo.Find_Query(Id));
        }

        public JsonResult UpdateQuery(QueryManagerViewModel.QueryManager model)
        {
            QueryManager query = AutoMapper.Mapper.Map<QueryManagerViewModel.QueryManager, QueryManager>(model);
            return Json(_configrepo.Update_Query(query, Convert.ToInt32(Session["Id"])), JsonRequestBehavior.AllowGet);
        }

        [SetupCheck]
        public JsonResult ValidateCode(string Code)
        {
            return Json(_configrepo.ValidateCode(Code));
        }
        [SetupCheck]
        public JsonResult UpdateAddon(SetupCreateViewModel.AddonViewModel model)
        {
            AddonSetup addon = AutoMapper.Mapper.Map<SetupCreateViewModel.AddonViewModel, AddonSetup>(model);
            return Json(_configrepo.Update_AddonSetup(addon, Convert.ToInt32(Session["Id"])), JsonRequestBehavior.AllowGet);
        }
        [SetupCheck]
        public JsonResult CreateSAP(SetupCreateViewModel.SAPViewModel model)
        {
            SAPSetup sap = AutoMapper.Mapper.Map<SetupCreateViewModel.SAPViewModel, SAPSetup>(model);
            _configrepo.Create_SapSetup(sap, Convert.ToInt32(Session["Id"]));
            _configrepo.SaveChanges();
            return Json(true);
        }
        public JsonResult FindSAP(int Id)
        {
            return Json(_configrepo.Find_SAP(Id));
        }
        [SetupCheck]
        public JsonResult UpdateSAP(SetupCreateViewModel.SAPViewModel model)
        {
            SAPSetup sap = AutoMapper.Mapper.Map<SetupCreateViewModel.SAPViewModel, SAPSetup>(model);
            return Json(_configrepo.Update_SapSetup(sap, Convert.ToInt32(Session["Id"])), JsonRequestBehavior.AllowGet);
        }

        [SetupCheck]
        public JsonResult CreateAPI(SetupCreateViewModel.APIViewModel model)
        {
            APISetup api = AutoMapper.Mapper.Map<SetupCreateViewModel.APIViewModel, APISetup>(model);
            _configrepo.Create_APISetup(api, Convert.ToInt32(Session["Id"]));
            _configrepo.SaveChanges();
            return Json(true);
        }

        [SetupCheck]
        public JsonResult CreateEmail(SetupCreateViewModel.EmailViewModel model)
        {
            EmailSetup email = AutoMapper.Mapper.Map<SetupCreateViewModel.EmailViewModel, EmailSetup>(model);
            _configrepo.Create_EmailSetup(email, Convert.ToInt32(Session["Id"]));
            _configrepo.SaveChanges();
            return Json(true);
        }

        public JsonResult FindAPI(int Id)
        {
            return Json(_configrepo.Find_API(Id));
        }

        public JsonResult FindEmail(int Id)
        {
            return Json(_configrepo.Find_Email(Id));
        }

        [SetupCheck]
        public JsonResult UpdateAPI(SetupCreateViewModel.APIViewModel model)
        {
            APISetup api = AutoMapper.Mapper.Map<SetupCreateViewModel.APIViewModel, APISetup>(model);
            return Json(_configrepo.Update_APISetup(api, Convert.ToInt32(Session["Id"])), JsonRequestBehavior.AllowGet);
        }

        [SetupCheck]
        public JsonResult UpdateEmail(SetupCreateViewModel.EmailViewModel model)
        {
            EmailSetup email = AutoMapper.Mapper.Map<SetupCreateViewModel.EmailViewModel, EmailSetup>(model);
            return Json(_configrepo.Update_EmailSetup(email, Convert.ToInt32(Session["Id"])), JsonRequestBehavior.AllowGet);
        }

        [SetupCheck]
        public JsonResult CreatePath(SetupCreateViewModel.PathViewModel model)
        {
            PathSetup path = AutoMapper.Mapper.Map<SetupCreateViewModel.PathViewModel, PathSetup>(model);
            _configrepo.Create_PathSetup(path, Convert.ToInt32(Session["Id"]));
            _configrepo.SaveChanges();
            return Json(true);
        }

        public JsonResult FindPath(int Id)
        {
            return Json(_configrepo.Find_Path(Id));
        }
        [SetupCheck]
        public JsonResult UpdatePath(SetupCreateViewModel.PathViewModel model)
        {
            PathSetup path = AutoMapper.Mapper.Map<SetupCreateViewModel.PathViewModel, PathSetup>(model);
            return Json(_configrepo.Update_PathSetup(path, Convert.ToInt32(Session["Id"])), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindMap(int Id)
        {
            var model = _maprepo.Find_Map(Id);
            Session["UpdateHead"] = model.Headers.Count;
            Session["UpdateRow"] = model.Rows.Count;
            return Json(model);
        }
        [FieldMappingCheck]
        public JsonResult ValidateMap(string Code)
        {
            return Json(_maprepo.ValidateMap(Code));
        }
        //json field mapping
        [FieldMappingCheck]
        public JsonResult CreateField(MapCreateViewModel.FieldMapping model, int Check,
                                      string RowTable, string HeaderTable,
                                      List<string[]> HeaderVal,
                                      List<string[]> RowVal,
                                      List<string[]> StoreHeaderVal,
                                      List<string[]> StoreRowVal)
        {
            //string filetype = "";
            //if (FileType != null)
            //{
            //    foreach (var item in FileType) { filetype += $"{item.ToString()},"; }
            //}
            var AddonPath = _maprepo.Select_AddonSetup(model.AddonCode);

            if (Check == 0)
            {
                Session["UpdateHead"] = 0;
                Session["UpdateRow"] = 0;
                FieldMapping field = AutoMapper.Mapper.Map<MapCreateViewModel.FieldMapping, FieldMapping>(model);
                _maprepo.Create_FieldMapping(field, Convert.ToInt32(Session["Id"]));
                _maprepo.SaveChanges();
            }
            else
            {
                FieldMapping field = AutoMapper.Mapper.Map<MapCreateViewModel.FieldMapping, FieldMapping>(model);
                _maprepo.Update_FieldMapping(field, Convert.ToInt32(Session["Id"]));
                _maprepo.SaveChanges();

            }

            if (HeaderVal != null || RowVal != null)
            {
                var NewHead = _maprepo.NewFieldValue(HeaderVal, Convert.ToInt32(Session["UpdateHead"]));
                var NewRow = _maprepo.NewFieldValue(RowVal, Convert.ToInt32(Session["UpdateRow"]));

                var oMapId = db.FieldMappings.OrderByDescending(x => x.MapId).FirstOrDefault();
                //saving of headers and rows to addon db
                if (HeaderVal != null)
                {
                    if (NewHead.Any() && Convert.ToInt32(Session["UpdateHead"]) != 0)
                    {
                        _maprepo.Create_Headers(NewHead, Check, HeaderTable, Convert.ToInt32(Session["Id"]), model.MapId, oMapId.MapId);
                    }
                    else //else if (HeaderVal.Any() && Convert.ToInt32(Session["UpdateHead"]) == 0)
                    {
                        _maprepo.Create_Headers(StoreHeaderVal, Convert.ToInt32(Session["Id"]), HeaderTable, Check, oMapId.MapId, model.MapId);
                    }
                }

                if (RowVal != null)
                {
                    if (NewRow.Any() && Convert.ToInt32(Session["UpdateRow"]) != 0)
                    {
                        _maprepo.Create_Rows(NewRow, Check, RowTable, Convert.ToInt32(Session["Id"]), model.MapId, oMapId.MapId);
                    }
                    else //else if (RowVal.Any() && Convert.ToInt32(Session["UpdateRow"]) == 0)
                    {
                        _maprepo.Create_Rows(StoreRowVal, Convert.ToInt32(Session["Id"]), RowTable, Check, oMapId.MapId, model.MapId);
                    }
                }
            }

            string constr = _maprepo.Get_Constring(AddonPath);

            _maprepo.GenerateTable(AddonPath.AddonDBName, constr,
                                   StoreHeaderVal, StoreRowVal,
                                   RowTable, HeaderTable,
                                   Convert.ToInt32(Session["UpdateHead"]),
                                   Convert.ToInt32(Session["UpdateRow"]));

            return Json(true);
        }

        [FieldMappingCheck]
        public JsonResult HanaTablePopulate(string HanaTable, string SAPCode, string HeaderTable, string RowTable)
        {
            return Json(_maprepo.Populate(HanaTable, SAPCode, HeaderTable, RowTable));
        }

        [FieldMappingCheck]
        public JsonResult HanaTablePopulateAPI(string HanaTable, string SAPCode, string HeaderTable, string RowTable, string APICode)
        {
            return Json(_maprepo.PopulateAPI(HanaTable, SAPCode, HeaderTable, RowTable, APICode));
        }

        [FieldMappingCheck]
        public JsonResult GetDataype(string HanaTable, string SAPCode, string Field)
        {
            return Json(_maprepo.Get_DataType(HanaTable, SAPCode, Field));
        }

        public JsonResult GetModule(int Id)
        {
            return Json(_global.GetModules(Id));
        }

        [HttpPost]
        public ActionResult SaveFile(HttpPostedFileBase DocFile, string Code, string SavePath, string FileCred)
        {
            Document model = new Document();
            var savePath = Server.MapPath("~/App_Data/").ToString();
            var InputFileName = Path.GetFileName(DocFile.FileName);
            var Extention = Path.GetExtension(DocFile.FileName);
            var ServerSavePath = Path.Combine(Server.MapPath("~/App_Data/") + InputFileName);
            var directory = new DirectoryInfo(Server.MapPath("~/App_Data/"));
            if (directory.Exists == false)
            {
                directory.Create();
            }
            DocFile.SaveAs(ServerSavePath);
            model.FileName = DocFile.FileName;
            model.FilePath = Path.Combine(savePath + InputFileName);
            model.SavePath = SavePath;
            model.Code = Code;
            model.Credential = FileCred;
            model.IsActive = false;
            model.CreateDate = DateTime.Now;
            db.Documents.Add(model);
            db.SaveChanges();

            return RedirectToAction("Setup", "Configuration");
        }

        [HttpPost]
        public ActionResult SaveCompany(HttpPostedFileBase LogoFile, string CompanyName, string Address, string MobileNo, string TelNo)
        {
            CompanyDetail model = new CompanyDetail();
            var savePath = Server.MapPath("~/ImageFile/").ToString();
            var InputFileName = Path.GetFileName(LogoFile.FileName);
            var Extention = Path.GetExtension(LogoFile.FileName);
            var ServerSavePath = Path.Combine(Server.MapPath("~/ImageFile/") + InputFileName);
            var directory = new DirectoryInfo(Server.MapPath("~/ImageFile/"));
            if (directory.Exists == false)
            {
                directory.Create();
            }
            LogoFile.SaveAs(ServerSavePath);
            model.FileName = LogoFile.FileName;
            model.FilePath = Path.Combine(savePath + InputFileName);
            model.CompanyName = CompanyName;
            model.Address = Address;
            model.MobileNo = MobileNo;
            model.TelNo = TelNo;
            model.IsActive = true;
            model.CreateDate = DateTime.Now;
            db.CompanyDetails.Add(model);
            db.SaveChanges();

            return RedirectToAction("Setup", "Configuration");
        }

        public JsonResult FindFile(int Id)
        {
            return Json(_configrepo.Find_File(Id));
        }
        public JsonResult FindCompany(int Id)
        {
            return Json(_configrepo.FindCompany(Id));
        }

        [HttpPost]
        public ActionResult UpdateFile(HttpPostedFileBase DocFile, string Code, string SavePath, int DocId, string FileStatus, string FileCred)
        {
            var model = db.Documents.Find(DocId);
            if (DocFile != null)
            {
                var savePath = Server.MapPath("~/App_Data/").ToString();
                var InputFileName = Path.GetFileName(DocFile.FileName);
                var ServerSavePath = Path.Combine(Server.MapPath("~/App_Data/") + InputFileName);
                var directory = new DirectoryInfo(Server.MapPath("~/App_Data/"));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                DocFile.SaveAs(ServerSavePath);
                var prevFile = model.FileName;
                FileInfo[] fi = directory.GetFiles().Where(f => f.Name == prevFile).ToArray();
                if (fi.Length != 0 && prevFile != DocFile.FileName)
                {
                    fi[0].Delete();
                }
                model.FileName = DocFile.FileName;
                model.FilePath = Path.Combine(savePath + InputFileName);
            }
            model.SavePath = SavePath;
            model.Code = Code;
            model.Credential = FileCred;
            model.IsActive = FileStatus.ToLower().Contains("checked") ? true : false;
            model.UpdateDate = DateTime.Now;
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Setup", "Configuration");
        }

        [HttpPost]
        public ActionResult UpdateCompany(HttpPostedFileBase LogoFile, string CompanyName, string Address, string MobileNo, string TelNo, int CompanyId, string CompanyStatus)
        {
            var model = db.CompanyDetails.Find(CompanyId);
            if (LogoFile != null)
            {
                var savePath = Server.MapPath("~/ImageFile/").ToString();
                var InputFileName = Path.GetFileName(LogoFile.FileName);
                var ServerSavePath = Path.Combine(Server.MapPath("~/ImageFile/") + InputFileName);
                var directory = new DirectoryInfo(Server.MapPath("~/ImageFile/"));
                if (directory.Exists == false)
                {
                    directory.Create();
                }
                LogoFile.SaveAs(ServerSavePath);
                var prevFile = model.FileName;
                FileInfo[] fi = directory.GetFiles().Where(f => f.Name == prevFile).ToArray();
                if (fi.Length != 0 && prevFile != LogoFile.FileName)
                {
                    fi[0].Delete();
                }
                model.FileName = LogoFile.FileName;
                model.FilePath = Path.Combine(savePath + InputFileName);
            }
            model.CompanyName = CompanyName;
            model.Address = Address;
            model.MobileNo = MobileNo;
            model.TelNo = TelNo;
            model.IsActive = string.IsNullOrEmpty(CompanyStatus) ? false : CompanyStatus.ToLower().Contains("on") || CompanyStatus.ToLower().Contains("checked") ? true : false;
            model.UpdateDate = DateTime.Now;
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Setup", "Configuration");
        }
    }
}