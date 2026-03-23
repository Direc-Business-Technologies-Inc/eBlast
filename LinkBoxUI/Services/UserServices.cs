using DomainLayer;
using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataCipher;
namespace LinkBoxUI.Services
{
    public class UserServices : IUserRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();
        public void Create_New_User(User user, int userid)
        {
            user.Password = Cryption.Encrypt(user.UserName + user.Password);
            user.IsActive = true;
            user.CreateUserID = userid;
            user.CreateDate = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();
            SaveUserModules(user.AuthorizationID, userid);
        }
        public void Update_User(int Id, string UserName, 
                                string FirstName, string LastName, 
                                string MiddleName, bool IsActive, 
                                string Password, string LastPassword,int userid)
        {
            var ousr = _context.Users.Find(Id);
            ousr.UserName = UserName;
            ousr.FirstName = FirstName;
            ousr.LastName = LastName;
            ousr.MiddleName = MiddleName;
            ousr.IsActive = IsActive;
            ousr.Password = Cryption.Encrypt(Password);
            if ((LastPassword != ""))
            {
                ousr.LastPassword = Cryption.Encrypt(LastPassword);
            }
            ousr.UpdateDate = DateTime.Now;
            ousr.UpdateUserID = userid;
            _context.Entry(ousr).State = System.Data.Entity.EntityState.Modified;
        }
        public List<UserDetailsViewModel.UserViewModel> Find_User(int id)
        {
            var model = new UserDetailsViewModel();
            model.CreateUser = _context.Users.Where(x => x.UserId == id).Select(x => new UserDetailsViewModel.UserViewModel

            {
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                Password = x.Password,
                ConfirmPassword = x.Password,
                LastPassword = x.LastPassword,
                IsActive = x.IsActive,
            }).ToList();
            var result = model.CreateUser.ToList();
            foreach (var x in result)
            {

                x.Password = Cryption.Decrypt(x.Password);
                x.ConfirmPassword = Cryption.Decrypt(x.ConfirmPassword);
                if (x.LastPassword != null)
                {
                    x.LastPassword = Cryption.Decrypt(x.LastPassword);
                }
            }
            return result;
        }
        public UserDetailsViewModel View_Users()
        {
            UserDetailsViewModel user = new UserDetailsViewModel();
            user.CreateUser = _context.Users.Where(x => x.UserId != 1).Select(x =>
                    new UserDetailsViewModel.UserViewModel
                    {
                        UserId = x.UserId,
                        UserName = x.UserName,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        MiddleName = x.MiddleName,
                        IsActive = x.IsActive
                    }).ToList();
            user.ModulesList = _context.Modules.Where(x => x.IsActive == true).Select(sel =>
                    new UserDetailsViewModel.Modules
                    {
                        ModName = sel.ModName,
                        ModId = sel.ModId,
                        ModCode = sel.ModuleCode
                    }).ToList();

            return user;
        }
        public IEnumerable<Authorization> AuthorizationList()
        {
            return _context.Authorizations.Where(x => x.IsActive == true);

        }
        public bool IsUserExist(string username)
        {
            return _context.Users.Where(e => e.UserName == username).Any();
        }
        public void SaveUserModules(int id, int userid)
        {
            var modules = _context.Users.Join(_context.Authorizations.Where(b => b.AuthId == id), a => a.AuthorizationID, b => b.AuthId, (a, b) => new { a, b })
                                        .Join(_context.AuthorizationModules.Where(c => c.IsActive == true), ab => ab.b.AuthId, c => c.AuthId, (ab, c) => new { ab, c })
                                        .Join(_context.Modules, abc => abc.c.ModId, d => d.ModId, (abc, d) => new { abc.ab.a.UserId, d.ModId }).ToList();
            foreach (var x in modules)
            {
                _context.UserModules.Add(new UserModule()
                {
                    UserId = x.UserId,
                    ModId = x.ModId,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    CreateUserID = userid
                });
                _context.SaveChanges();
            }
        }
        public UserDetailsViewModel GetUserModules(int id)
        {
            var model = new UserDetailsViewModel();

            model.UserModuleList = _context.UserModules.Where(a => a.UserId == id).Join(_context.Users, a => a.UserId, b => b.UserId, (a, b) => new { a, b })
                                           .Join(_context.Modules.Where(c => c.IsActive == true), ab => ab.a.ModId, c => c.ModId, (ab, c) => new UserDetailsViewModel.UserModules
                                           {
                                               UMId = ab.a.Id,
                                               UserName = ab.b.UserName,
                                               UserId = ab.b.UserId,
                                               ModId = c.ModId,
                                               ModuleName = c.ModName,
                                               ModuleCode = c.ModuleCode,
                                               IsActive = ab.a.IsActive
                                           }).ToList();

            model.ModulesList = _context.Modules.Where(x => x.IsActive == true).Select(sel => new UserDetailsViewModel.Modules
            {
                ModName = sel.ModName,
                ModId = sel.ModId,
                ModCode = sel.ModuleCode
            }).ToList();
            return model;
        }
        public bool isActiveModule(int id)
        {
            var modules = _context.UserModules.Find(id);
            return modules.IsActive;
        }
        public void UpdateUserModules(int userid, int moduleid, bool isactive)
        {
            var modules = _context.UserModules.Find(moduleid);
            modules.IsActive = isactive;
            modules.UpdateDate = DateTime.Now;
            modules.UpdateUserID = userid;
            _context.Entry(modules).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }
        public int GetModuleId(int id)
        {
            var modules = _context.UserModules.Find(id);
            return modules.ModId;
        }
        public List<int> NewModuleList(List<int> olddata,List<int> newdata)
        {
            foreach (var x in olddata)
            {
                newdata.Remove(x);
            }
            return newdata;
        }
        public void AddUserModules(int createid,int userid,int moduleid)
        {
            var module = new UserModule();
            module.UserId = userid;
            module.ModId = moduleid;
            module.IsActive = true;
            module.CreateDate = DateTime.Now;
            module.CreateUserID = createid;
            _context.UserModules.Add(module);
            _context.SaveChanges();
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }


    }
}