using InfrastructureLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainLayer.ViewModels;
using SAPbobsCOM;
using LinkBoxUI.Context;
using DomainLayer;
using DataCipher;
using System.Data;
using DataAccessLayer.Class;
using DomainLayer.Models;

namespace LinkBoxUI.Services
{
    public class ConfigurationServices : IConfigurationRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();
        public SetupCreateViewModel View_Setup()
        {
            SetupCreateViewModel model = new SetupCreateViewModel();

            List<string> SAPVersionList = new List<string>();

            model.AddonView = _context.AddonSetup.Select(x => new SetupCreateViewModel.AddonViewModel
            {
                AddonId = x.AddonId,
                AddonCode = x.AddonCode,
                AddonDBVersion = x.AddonDBVersion,
                AddonServerName = x.AddonServerName,
                AddonIPAddress = x.AddonIPAddress,
                AddonDBName = x.AddonDBName,
                AddonPort = x.AddonPort,
                AddonDBuser = x.AddonDBuser,
                AddonDBPassword = x.AddonDBPassword,
                IsActive = x.IsActive
            }).ToList();

            model.SAPView = _context.SAPSetup.Select(x => new SetupCreateViewModel.SAPViewModel
            {
                SAPId = x.SAPId,
                SAPCode = x.SAPCode,
                SAPDBVersion = x.SAPDBVersion,
                SAPServerName = x.SAPServerName,
                SAPIPAddress = x.SAPIPAddress,
                SAPDBName = x.SAPDBName,
                SAPVersion = x.SAPVersion,
                SAPLicensePort = x.SAPLicensePort,
                SAPDBPort = x.SAPDBPort,
                SAPDBuser = x.SAPDBuser,
                SAPDBPassword = x.SAPDBPassword,
                SAPUser = x.SAPUser,
                SAPPassword = x.SAPPassword,
                IsActive = x.IsActive

            }).ToList();

            model.PathView = _context.PathSetup.Select(x => new SetupCreateViewModel.PathViewModel
            {
                PathId = x.PathId,
                PathCode = x.PathCode,
                LocalPath = x.LocalPath,
                BackupPath = x.BackupPath,
                RemotePath = x.RemotePath,
                RemoteServerName = x.RemoteServerName,
                RemoteIPAddress = x.RemoteIPAddress,
                RemotePort = x.RemotePort,
                RemoteUserName = x.RemoteUserName,
                RemotePassword = x.RemotePassword,
                //FileType = x.FileType,
                IsActive = x.IsActive
            }).ToList();

            model.APIView = _context.APISetups.Select(x => new SetupCreateViewModel.APIViewModel
            {
                APIId = x.APIId,
                APICode = x.APICode,
                APIURL = x.APIURL,
                APIMethod = x.APIMethod,
                IsActive = x.IsActive,
                //APIKey = x.APIKey,
                //APISecretKey = x.APISecretKey,
                //APIToken = x.APIToken,
            }).ToList();

            model.ParameterList = _context.Paramenters.Select(x => new SetupCreateViewModel.Parameter
            {
                Code = x.Code,
                Id = x.Id,
                ParameterType = x.ParameterType,
                Value = x.Value,
                IsActive = x.IsActive
            }).ToList();

            model.DocumentList = _context.Documents.Select(x => new SetupCreateViewModel.Document
            {
                Code = x.Code,
                Id = x.Id,
                FileName = x.FileName,
                IsActive = x.IsActive
            }).ToList();


            model.CompanyList = _context.CompanyDetails.Select(x => new SetupCreateViewModel.Company
            {

                Id = x.Id,
                FileName = x.FileName,
                IsActive = x.IsActive,
                Address = x.Address,
                MobileNo = x.MobileNo,
                TelNo = x.TelNo,
                CompanyName = x.CompanyName,
                FilePath = x.FilePath

            }).ToList();

            model.EmailView = _context.EmailSetup.Select(x => new SetupCreateViewModel.EmailViewModel
            {
                EmailId = x.EmailId,
                EmailCode = x.EmailCode,
                EmailDesc = x.EmailDesc,
                Email = x.Email,
                DisplayName = x.DisplayName,
                Password = x.Password,
                Port = x.Port,
                SMTPClient = x.SMTPClient,
                IsActive = x.IsActive
            }).ToList();

            foreach (var item in Enum.GetValues(typeof(BoDataServerTypes)))
            {
                SAPVersionList.Add(item.ToString());
            }
            model.SAPList = SAPVersionList.Select(x => new SetupCreateViewModel.SAPVersion
            {
                SAPDBVersion = x.ToString(),

            }).ToList();
            return model;
        }
        public void Create_AddonSetup(AddonSetup addon, int id)
        {
            addon.CreateDate = DateTime.Now;
            addon.IsActive = true;
            addon.CreateUserID = id;
            _context.AddonSetup.Add(addon);
            _context.SaveChanges();
        }
        public bool Update_AddonSetup(AddonSetup addon, int id)
        {
            var addonsetup = addon;
            addonsetup.UpdateUserID = id;
            addonsetup.UpdateDate = DateTime.Now;
            _context.Entry(addonsetup).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }
        public void Create_SapSetup(SAPSetup sap, int id)
        {
            sap.CreateDate = DateTime.Now;
            sap.IsActive = true;
            sap.CreateUserID = id;
            _context.SAPSetup.Add(sap);
            _context.SaveChanges();
        }
        public bool Update_SapSetup(SAPSetup sap, int id)
        {
            var sapsetup = sap;
            sapsetup.UpdateUserID = id;
            sapsetup.UpdateDate = DateTime.Now;
            _context.Entry(sapsetup).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public void Create_APISetup(APISetup api, int id)
        {
            api.CreateDate = DateTime.Now;
            api.IsActive = true;
            api.CreateUserID = id;
            _context.APISetups.Add(api);
            _context.SaveChanges();
        }

        public void Create_EmailSetup(EmailSetup email, int id)
        {
            email.CreateDate = DateTime.Now;
            email.IsActive = true;
            email.CreateUserID = id;
            email.Password = Cryption.Encrypt($"{email.Password}");
            _context.EmailSetup.Add(email);
            _context.SaveChanges();
        }
        public bool Update_APISetup(APISetup api, int id)
        {
            var apisetup = api;
            apisetup.UpdateUserID = id;
            apisetup.CreateUserID = id;
            apisetup.UpdateDate = DateTime.Now;
            apisetup.CreateDate = DateTime.Now;
            _context.Entry(apisetup).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public bool Update_Query(QueryManager api, int id)
        {
            var queryManager = api;
            queryManager.UpdateUserId = id;
            queryManager.CreateUserId = id;
            queryManager.CreateDate = DateTime.Now;
            queryManager.UpdateDate = DateTime.Now;
            _context.Entry(queryManager).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public bool Update_EmailSetup(EmailSetup emailSetup, int id)
        {
            var email = emailSetup;
            email.UpdateUserID = id;
            email.IsActive = emailSetup.IsActive;
            email.CreateDate = DateTime.Now;
            email.UpdateDate = DateTime.Now;
            email.Password = Cryption.Encrypt($"{email.Password}");
            _context.Entry(email).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }
        public void Create_PathSetup(PathSetup path, int id)
        {
            path.CreateDate = DateTime.Now;
            path.IsActive = true;
            path.CreateUserID = id;
            _context.PathSetup.Add(path);
            _context.SaveChanges();
        }
        public SetupCreateViewModel Find_Addon(int id)
        {
            var model = new SetupCreateViewModel();
            model.AddonView = _context.AddonSetup
                                .Where(x => x.AddonId == id)
                                .Select((x) => new SetupCreateViewModel.AddonViewModel
                                {
                                    AddonCode = x.AddonCode,
                                    AddonDBVersion = x.AddonDBVersion,
                                    AddonServerName = x.AddonServerName,
                                    AddonIPAddress = x.AddonIPAddress,
                                    AddonDBName = x.AddonDBName,
                                    AddonPort = x.AddonPort,
                                    AddonDBuser = x.AddonDBuser,
                                    AddonDBPassword = x.AddonDBPassword,
                                    IsActive = x.IsActive,
                                }).ToList();
            return model;
        }

        public SetupCreateViewModel Find_ConnectionString(string Type)
        {
            var model = new SetupCreateViewModel();

            switch (Type)
            {
                case "SAP":
                    model.DatabaseConnectionView = _context.SAPSetup
                                .Where(x => x.IsActive == true)
                                .Select((x) => new SetupCreateViewModel.DbConnection
                                {
                                    Id = x.SAPId,
                                    QueryName = x.SAPCode
                                }).ToList();
                    break;
                default:
                    model.DatabaseConnectionView = _context.AddonSetup
                                .Where(x => x.IsActive == true)
                                .Select((x) => new SetupCreateViewModel.DbConnection
                                {
                                    Id = x.AddonId,
                                    QueryName = x.AddonCode
                                }).ToList();
                    break;
            }

            return model;
        }

        public SetupCreateViewModel GetConnection(string Type, string ConString)
        {
            var model = new SetupCreateViewModel();

            switch (Type)
            {
                case "SAP":
                    model.DatabaseConnectionView = _context.SAPSetup
                                .AsEnumerable()
                                .Where(x => x.IsActive == true && x.SAPId == Convert.ToInt32(ConString))
                                .Select((x) => new SetupCreateViewModel.DbConnection
                                {
                                    Id = x.SAPId,
                                    QueryName = x.SAPCode,
                                    ConnectionType = x.SAPDBVersion,
                                    ConnectionString = (x.SAPDBVersion.Contains("HANA") ? "DRIVER={HDBODBC32};" + $"SERVERNODE={x.SAPServerName}{(x.SAPDBPort > 0 ? $":{x.SAPDBPort}" : "")};UID={x.SAPDBuser};PWD={x.SAPDBPassword};CS={x.SAPDBName}" :
                                                                                           $"Data Source={x.SAPServerName}{(x.SAPDBPort > 0 ? $":{x.SAPDBPort}" : "")};Initial Catalog={x.SAPDBName};Persist Security Info=True;User ID={x.SAPDBuser};Password={x.SAPDBPassword}")
                                }).ToList();
                    break;
                default:
                    model.DatabaseConnectionView = _context.AddonSetup
                                .AsEnumerable()
                                .Where(x => x.IsActive == true && x.AddonId == Convert.ToInt32(ConString))
                                .Select((x) => new SetupCreateViewModel.DbConnection
                                {
                                    Id = x.AddonId,
                                    QueryName = x.AddonCode,
                                    ConnectionType = x.AddonDBVersion,
                                    ConnectionString = (x.AddonDBVersion.Contains("HANA") ? "DRIVER={HDBODBC32};" + $"SERVERNODE={x.AddonServerName}{(x.AddonPort > 0 ? $":{x.AddonPort}" : "")};UID={x.AddonDBuser};PWD={x.AddonDBPassword};CS={x.AddonDBName}" :
                                                                                           $"Data Source={x.AddonServerName}{(x.AddonPort > 0 ? $":{x.AddonPort}" : "")};Initial Catalog={x.AddonDBName};Persist Security Info=True;User ID={x.AddonDBuser};Password={x.AddonDBPassword}")
                                }).ToList();
                    break;
            }

            return model;
        }

        public DataTable Fill_DataTable(SetupCreateViewModel model, string Query)
        {
            DataTable dt = new DataTable();
            foreach (var item in model.DatabaseConnectionView)
            {
                if (item.ConnectionType.Contains("HANA"))
                {
                    dt = DataAccess.SelectHana(item.ConnectionString, Query);
                }
                else
                {
                    dt = DataAccess.Select(item.ConnectionString, Query);
                }
            }
            return dt;
        }

        public void Create_Query(QueryManager query, int id)
        {
            query.CreateDate = DateTime.Now;
            query.UpdateDate = DateTime.Now;
            query.IsActive = true;
            query.CreateUserId = id;
            _context.QueryManager.Add(query);
            _context.SaveChanges();
        }

        public SetupCreateViewModel Find_SAP(int id)
        {
            var model = new SetupCreateViewModel();
            model.SAPView = _context.SAPSetup.Where(x => x.SAPId == id).Select((x) => new SetupCreateViewModel.SAPViewModel
            {
                SAPCode = x.SAPCode,
                SAPDBVersion = x.SAPDBVersion,
                SAPLicensePort = x.SAPLicensePort,
                SAPServerName = x.SAPServerName,
                SAPIPAddress = x.SAPIPAddress,
                SAPDBPort = x.SAPDBPort,
                SAPDBName = x.SAPDBName,
                SAPVersion = x.SAPVersion,
                SAPDBuser = x.SAPDBuser,
                SAPDBPassword = x.SAPDBPassword,
                SAPUser = x.SAPUser,
                SAPPassword = x.SAPPassword,
                IsActive = x.IsActive,

            }).ToList();

            return model;
        }

        public SetupCreateViewModel Find_Query(int id)
        {
            var model = new SetupCreateViewModel();
            model.DatabaseConnectionView = _context.QueryManager.Where(x => x.Id == id).Select((x) => new SetupCreateViewModel.DbConnection
            {
                QueryCode = x.QueryCode,
                QueryName = x.QueryName,
                ConnectionString = x.ConnectionString,
                ConnectionType = x.ConnectionType,
                QueryString = x.QueryString,
                IsActive = x.IsActive
            }).ToList();

            return model;
        }

        public SetupCreateViewModel Find_API(int id)
        {
            var model = new SetupCreateViewModel();
            model.APIView = _context.APISetups.Where(x => x.APIId == id).Select((x) => new SetupCreateViewModel.APIViewModel
            {
                APIId = x.APIId,
                APICode = x.APICode,
                APIMethod = x.APIMethod,
                //APIModule = x.APIModule,
                APIURL = x.APIURL,
                IsActive = x.IsActive,
                //APIKey =x.APIKey,
                //APISecretKey = x.APISecretKey,
                //APIToken = x.APIToken,
                //APILoginUrl = x.APILoginUrl,
                //APILoginBody = x.APILoginBody,
            }).ToList();

            return model;
        }

        public SetupCreateViewModel Find_Email(int id)
        {
            var model = new SetupCreateViewModel();
            model.EmailView = _context.EmailSetup.AsEnumerable().Where(x => x.EmailId == id).Select((x) => new SetupCreateViewModel.EmailViewModel
            {
                EmailId = x.EmailId,
                EmailCode = x.EmailCode,
                CreateDate = x.CreateDate,
                CreateUserID = x.CreateUserID,
                EmailDesc = x.EmailDesc,
                Email = x.Email,
                DisplayName = x.DisplayName,
                Password = Cryption.Decrypt(x.Password).Replace(x.Email, ""),
                SMTPClient = x.SMTPClient,
                Port = x.Port,
                IsActive = x.IsActive,
            }).ToList();

            return model;
        }

        public SetupCreateViewModel Find_Path(int id)
        {
            var model = new SetupCreateViewModel();

            model.PathView = _context.PathSetup
                               .Where(x => x.PathId == id)
                               .Select((x) => new SetupCreateViewModel.PathViewModel
                               {
                                   PathCode = x.PathCode,
                                   LocalPath = x.LocalPath,
                                   BackupPath = x.BackupPath,
                                   RemotePath = x.RemotePath,
                                   RemoteServerName = x.RemoteServerName,
                                   RemoteIPAddress = x.RemoteIPAddress,
                                   RemotePort = x.RemotePort,
                                   RemoteUserName = x.RemoteUserName,
                                   RemotePassword = x.RemotePassword,
                                   //FileType = x.FileType,
                                   IsActive = x.IsActive,

                               }).ToList();

            return model;
        }

        public SetupCreateViewModel Find_File(int id)
        {
            var model = new SetupCreateViewModel();

            model.DocumentList = _context.Documents
                               .Where(x => x.Id == id)
                               .Select((x) => new SetupCreateViewModel.Document
                               {
                                   Code = x.Code,
                                   Id = x.Id,
                                   FileName = x.FileName,
                                   SavePath = x.SavePath,
                                   IsActive = x.IsActive,
                                   Credential = x.Credential
                               }).ToList();

            return model;
        }

        public SetupCreateViewModel FindCompany(int id)
        {
            var model = new SetupCreateViewModel();

            model.CompanyDetails = _context.CompanyDetails
                               .Where(x => x.Id == id)
                               .Select((x) => new SetupCreateViewModel.Company
                               {
                                   Id = x.Id,
                                   FileName = x.FileName,
                                   IsActive = x.IsActive,
                                   Address = x.Address,
                                   MobileNo = x.MobileNo,
                                   TelNo = x.TelNo,
                                   CompanyName = x.CompanyName,
                                   FilePath = x.FilePath
                               }).FirstOrDefault();

            return model;
        }
        public SetupCreateViewModel Find_Addon(string code)
        {
            var model = new SetupCreateViewModel();
            model.AddonView = _context.AddonSetup
                                .Where(x => x.AddonCode == code)
                                .Select((x) => new SetupCreateViewModel.AddonViewModel
                                {
                                    AddonCode = x.AddonCode,
                                    AddonDBVersion = x.AddonDBVersion,
                                    AddonServerName = x.AddonServerName,
                                    AddonIPAddress = x.AddonIPAddress,
                                    AddonDBName = x.AddonDBName,
                                    AddonPort = x.AddonPort,
                                    AddonDBuser = x.AddonDBuser,
                                    AddonDBPassword = x.AddonDBPassword,
                                    IsActive = x.IsActive,
                                }).ToList();
            return model;
        }
        public SetupCreateViewModel Find_SAP(string code)
        {
            var model = new SetupCreateViewModel();
            model.SAPView = _context.SAPSetup.Where(x => x.SAPCode == code).Select((x) => new SetupCreateViewModel.SAPViewModel
            {
                SAPCode = x.SAPCode,
                SAPDBVersion = x.SAPDBVersion,
                SAPLicensePort = x.SAPLicensePort,
                SAPServerName = x.SAPServerName,
                SAPIPAddress = x.SAPIPAddress,
                SAPDBPort = x.SAPDBPort,
                SAPDBName = x.SAPDBName,
                SAPVersion = x.SAPVersion,
                SAPDBuser = x.SAPDBuser,
                SAPDBPassword = x.SAPDBPassword,
                SAPUser = x.SAPUser,
                SAPPassword = x.SAPPassword,
                IsActive = x.IsActive,

            }).ToList();

            return model;
        }
        public SetupCreateViewModel Find_Path(string code)
        {
            var model = new SetupCreateViewModel();

            model.PathView = _context.PathSetup
                               .Where(x => x.PathCode == code)
                               .Select((x) => new SetupCreateViewModel.PathViewModel
                               {
                                   PathCode = x.PathCode,
                                   LocalPath = x.LocalPath,
                                   BackupPath = x.BackupPath,
                                   RemotePath = x.RemotePath,
                                   RemoteServerName = x.RemoteServerName,
                                   RemoteIPAddress = x.RemoteIPAddress,
                                   RemotePort = x.RemotePort,
                                   RemoteUserName = x.RemoteUserName,
                                   RemotePassword = x.RemotePassword,
                                   //FileType = x.FileType,
                                   IsActive = x.IsActive,

                               }).ToList();

            return model;
        }
        public bool Update_PathSetup(PathSetup path, int id)
        {
            var pathsetup = path;
            pathsetup.UpdateUserID = id;
            pathsetup.UpdateDate = DateTime.Now;
            _context.Entry(pathsetup).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }
        public SetupCreateViewModel ValidateCode(string code)
        {
            var model = new SetupCreateViewModel();

            model.AddonView = _context.AddonSetup
                                .Where(x => x.AddonCode == code)
                                .Select((x) => new SetupCreateViewModel.AddonViewModel
                                {
                                    AddonCode = x.AddonCode,

                                }).ToList();
            model.PathView = _context.PathSetup
                               .Where(x => x.PathCode == code)
                               .Select((x) => new SetupCreateViewModel.PathViewModel
                               {
                                   PathCode = x.PathCode,

                               }).ToList();
            model.SAPView = _context.SAPSetup
                              .Where(x => x.SAPCode == code)
                              .Select((x) => new SetupCreateViewModel.SAPViewModel
                              {
                                  SAPCode = x.SAPCode,

                              }).ToList();
            model.APIView = _context.APISetups
                           .Where(x => x.APICode == code)
                           .Select((x) => new SetupCreateViewModel.APIViewModel
                           {
                               APICode = x.APICode,

                           }).ToList();

            return model;
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}