using DataCipher;
using DomainLayer;
using LinkBoxUI.Context;
using LinkBoxUI.SessionChecker;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InfrastructureLayer.Repositories;

namespace LinkBoxUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginRepository _loginrepo;
        private readonly IGlobalRepository _globalrepo;

        public HomeController(ILoginRepository login,IGlobalRepository global)
        {
            _loginrepo = login;
            _globalrepo = global;
        }

        LinkboxDb db = new LinkboxDb();
        [SessionCheck]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            Session["ModuleAccess"] = 0;
            return View(_globalrepo.GetItemStock());
        }

        public ActionResult Logout()
        {
            _loginrepo.Logout((int)Session["Id"]);
            _loginrepo.SaveChanges();
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            return RedirectToActionPermanent("Login", "Home");
        }
        public ActionResult Login()
        {
            _loginrepo.Login_Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModels.Login login)
        {
            if (ModelState.IsValid)
            {
                var user =  _loginrepo.Get_User(login);
                ViewBag.Attempt = 0;

                if (user == null)
                {
                    ViewBag.WarningMessage = "User does not exist ";
                    ViewBag.Attempt = "";
                    ViewBag.Id = null;
                }
                else if (!user.IsActive)
                {
                    ViewBag.WarningMessage = "Inactive User";
                    ViewBag.Attempt = "";
                    ViewBag.Id = null;
                }
                else
                {
                    var userPass = _loginrepo.Check_User(login,user);
                    if (userPass == null)
                    {
                        _loginrepo.Update_Attempt(user,userPass);
                        _loginrepo.SaveChanges();
                        ViewBag.Attempt = user.Attempt;
                        Session["Attempt"] = user.Attempt;
                        Session["Id"] = user.UserId;
                        ViewBag.WarningMessage = "Incorrect Username or Password";

                    }
                    else if (userPass != null)
                    {
                        _loginrepo.Update_Attempt(user, userPass);
                        _loginrepo.SaveChanges();
                        Session["Id"] = userPass.UserId;
                        Session["UserName"] = userPass.UserName;
                        Session["Name"] = $"{userPass.FirstName} {userPass.MiddleName} {userPass.LastName}";
                        Session["CreateDate"] = userPass.CreateDate;
                        Session["LastLogin"] = userPass.LastLoginDate;                        
                        return RedirectToAction("Index", "Home");
                    }
                }
            }            

            ViewBag.Title = "Login Page";
            return View();
        }
        
        public JsonResult GetModule(int Id)
        { 
            return Json(_globalrepo.GetModules(Id));
        }
        public JsonResult GetItemStock(string Code)
        {
            return Json(_globalrepo.GetItemStock(Code));
        }
        //public JsonResult GetItemStock()
        //{
        //    return Json(_globalrepo.GetItemStock());
        //}

        public JsonResult SetCount()
        {
            if(Session["Id"] != null)
            {
                _loginrepo.Login_Attempt(Convert.ToInt32(Session["Id"]),
                                         Convert.ToInt32(Session["Attempt"]));
            }
            return Json(true);
        }

    }
}
