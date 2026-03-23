using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainLayer.ViewModels;
using DataAccessLayer.Class;
using System.Data;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ExcelDataReader;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LinkBoxUI.Helpers;

namespace LinkBoxUI.Services
{
    public class GlobalServices : IGlobalRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();
        public DataAccess da = new DataAccess();
        public SAPAccess sapAces = new SAPAccess();
        public List<int> GetModules(int id)
        {
            var mod = from um in _context.UserModules.Where(a => a.UserId == id && a.IsActive == true)
                      select um.ModId;
            mod.ToList();
            List<int> mods = new List<int>();
            for (int x = 1; x <= 11; x++)
            {
                mods.Add(x);
            }
            foreach (var x in mod)
            {
                mods.Remove(x);
            }
            return mods;
        }

        public DashboardViewModel GetItemStock()
        {
            var model = new DashboardViewModel();
            model.SAPList = _context.FieldMappings.Select(x => new DashboardViewModel.SAPConfig
            { Code = x.MapCode }).ToList();

            return model;
        }

        public DashboardViewModel GetItemStock(string Code)
        {
            var model = new DashboardViewModel();
            var config = new MapCreateViewModel();
            var Setup = new SetupCreateViewModel();
            DataTable table = new DataTable();

            config.FieldMapDetails = _context.FieldMappings.AsEnumerable()?.Where(x => x.MapCode == Code).Select(x => new MapCreateViewModel.FieldMapping
            {
                MapId = x.MapId,
                MapCode = x.MapCode,
                AddonCode = x.AddonCode,
                SAPCode = x.SAPCode,
                FileName = x.FileName,
                FileType = x.FileType,
                HeaderWorksheet = x.HeaderWorksheet,
                MapName = x.MapName,
                ModuleName = x.ModuleName,
                PathCode = x.PathCode,
                RowWorksheet = x.RowWorksheet,

            }).FirstOrDefault();

            if (config.FieldMapDetails != null)
            {
                var PathSettings = _context.PathSetup.Where(x => x.PathCode == config.FieldMapDetails.PathCode).FirstOrDefault();
                if (PathSettings != null)
                {
                    Setup.SAPDbDetails = _context.SAPSetup.AsEnumerable()?.Where(x => x.SAPCode == Code).Select(x => new SetupCreateViewModel.SAPViewModel
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
                    }).FirstOrDefault();

                    foreach (var item in DataAccess.GetFileListFTP(PathSettings.RemotePath, PathSettings.RemoteUserName, PathSettings.RemotePassword))
                    {
                        if ((item.ToLower().Contains("d") && !item.ToLower().Contains("z")) && item.ToLower().Contains(".csv"))
                        {
                            Stream stream = DataAccess.ReadFromFTP(PathSettings.RemotePath + item, PathSettings.RemoteUserName, PathSettings.RemotePassword);
                            table.Merge(ExcelAccessV2.GetFileData(stream, item).Tables[0]);
                        }
                    }

                    model.ItemList = Execute.ExecuteQuery(Setup)?.AsEnumerable()?.Select(x => new DashboardViewModel.Item
                    {
                        ItemCode = x["ItemCode"].ToString(),
                        ItemName = x["ItemName"].ToString(),
                        Uom = x["SalUnitMsr"].ToString(),
                        Price = ConcatDecimal(x["Price"].ToString()),
                        LastUpdate = DateTime.Now.ToString(),
                        Stock = da.ConcatDecimal(x["Stock"].ToString()),
                        SalesPrice = da.ConcatDecimal(table.AsEnumerable()?.Where(y => y["ProductID"].ToString() == x["ItemCode"].ToString()).Select(y => y["UnitPrice"].ToString()).FirstOrDefault()),
                        SalesQty = table.AsEnumerable()?.Where(y => y["ProductID"].ToString() == x["ItemCode"].ToString()).Sum(y => Convert.ToDouble(y["QtySold"].ToString())).ToString(),
                        CurrentStock = da.ConcatDecimal((Convert.ToDouble(x["Stock"].ToString()) - table.AsEnumerable().Where(y => y["ProductID"].ToString() == x["ItemCode"].ToString()).Sum(y => Convert.ToDouble(y["QtySold"].ToString()))).ToString())

                    }).ToList() ?? new List<DashboardViewModel.Item>();

                    model.ItemComparisonList = model.ItemList.AsEnumerable().Where(x => x.Price != x.SalesPrice && Convert.ToDouble(x.SalesPrice) != 0).ToList();
                }
            }


            return model;
        }


        public PostingViewModel GetCredentials(string Task)
        {
            PostingViewModel model = new PostingViewModel();
            model.CredentialDetails = _context.FieldMappings.Join(_context.Headers, fm => fm.MapId, h => h.MapId, (fm, h) => new { fm, h })
                          .Join(_context.Rows, fmh => fmh.fm.MapId, r => r.MapId, (fmh, r) => new { fmh, r })
                          .Join(_context.PathSetup, fmhr => fmhr.fmh.fm.PathCode, p => p.PathCode, (fmhr, p) => new { fmhr, p })
                          .Join(_context.AddonSetup, fmhrp => fmhrp.fmhr.fmh.fm.AddonCode, a => a.AddonCode, (fmhrp, a) => new { fmhrp, a })
                          .Join(_context.SAPSetup, fmhrpa => fmhrpa.fmhrp.fmhr.fmh.fm.SAPCode, s => s.SAPCode, (fmhrpa, s) => new { fmhrpa, s })
                          .Join(_context.Process, fmhrpas => fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.MapId, pr => pr.MapId, (fmhrpas, pr) => new { fmhrpas, pr })
                          .Join(_context.Schedules.Where(x => x.SchedCode == Task), fmhrpaspr => fmhrpaspr.pr.ProcessCode, sh => sh.Process, (fmhrpaspr, sh) => new PostingViewModel.Credential
                          {
                              MapId = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.MapId,
                              AddonCode = fmhrpaspr.fmhrpas.fmhrpa.a.AddonCode,
                              AddonDBName = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBName,
                              AddonIPAddress = fmhrpaspr.fmhrpas.fmhrpa.a.AddonIPAddress,
                              AddonPort = fmhrpaspr.fmhrpas.fmhrpa.a.AddonPort,
                              AddonServerName = fmhrpaspr.fmhrpas.fmhrpa.a.AddonServerName,
                              AddonDBuser = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBuser,
                              AddonDBPassword = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBPassword,
                              LocalPath = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.LocalPath,
                              FTPPath = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.RemotePath,
                              FTPUser = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.RemoteUserName,
                              FTPPass = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.RemotePassword,
                              BackupPath = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.BackupPath,
                              HeaderName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.h.TableName,
                              RowName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.r.TableName,
                              HeaderWorksheet = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.HeaderWorksheet,
                              RowWorksheet = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.RowWorksheet,
                              FileType = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.FileType,
                              FileName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.FileName,
                              SAPServerName = fmhrpaspr.fmhrpas.s.SAPServerName,
                              SAPIPAddress = fmhrpaspr.fmhrpas.s.SAPIPAddress,
                              SAPDBName = fmhrpaspr.fmhrpas.s.SAPDBName,
                              SAPLicensePort = fmhrpaspr.fmhrpas.s.SAPLicensePort,
                              SAPDBPort = fmhrpaspr.fmhrpas.s.SAPDBPort,
                              SAPDBuser = fmhrpaspr.fmhrpas.s.SAPDBuser,
                              SAPDBPassword = fmhrpaspr.fmhrpas.s.SAPDBPassword,
                              SAPUser = fmhrpaspr.fmhrpas.s.SAPUser,
                              SAPPassword = fmhrpaspr.fmhrpas.s.SAPPassword,
                              PostSAP = fmhrpaspr.pr.PostSAP,
                              Module = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.ModuleName
                          }).FirstOrDefault();
            return model;
        }

        public PostingViewModel GetAPICredentials(string Task)
        {
            PostingViewModel model = new PostingViewModel();
            model.CredentialDetails = _context.FieldMappings.Join(_context.Headers, fm => fm.MapId, h => h.MapId, (fm, h) => new { fm, h })
                          .Join(_context.Rows, fmh => fmh.fm.MapId, r => r.MapId, (fmh, r) => new { fmh, r })
                          .Join(_context.AddonSetup, fmhrp => fmhrp.fmh.fm.AddonCode, a => a.AddonCode, (fmhrp, a) => new { fmhrp, a })
                          .Join(_context.SAPSetup, fmhrpa => fmhrpa.fmhrp.fmh.fm.SAPCode, s => s.SAPCode, (fmhrpa, s) => new { fmhrpa, s })
                          .Join(_context.Process, fmhrpas => fmhrpas.fmhrpa.fmhrp.fmh.fm.MapId, pr => pr.MapId, (fmhrpas, pr) => new { fmhrpas, pr })
                          .Join(_context.Schedules.Where(x => x.SchedCode == Task), fmhrpaspr => fmhrpaspr.pr.ProcessCode, sh => sh.Process, (fmhrpaspr, sh) => new PostingViewModel.Credential
                          {
                              MapId = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmh.fm.MapId,
                              AddonCode = fmhrpaspr.fmhrpas.fmhrpa.a.AddonCode,
                              AddonDBName = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBName,
                              AddonIPAddress = fmhrpaspr.fmhrpas.fmhrpa.a.AddonIPAddress,
                              AddonPort = fmhrpaspr.fmhrpas.fmhrpa.a.AddonPort,
                              AddonServerName = fmhrpaspr.fmhrpas.fmhrpa.a.AddonServerName,
                              AddonDBuser = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBuser,
                              AddonDBPassword = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBPassword,
                              LocalPath = "",
                              FTPPath = "",
                              FTPUser = "",
                              FTPPass = "",
                              BackupPath = "",
                              HeaderName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmh.h.TableName,
                              RowName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.r.TableName,
                              HeaderWorksheet = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmh.fm.HeaderWorksheet,
                              RowWorksheet = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmh.fm.RowWorksheet,
                              FileType = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmh.fm.FileType,
                              FileName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmh.fm.FileName,
                              SAPServerName = fmhrpaspr.fmhrpas.s.SAPServerName,
                              SAPIPAddress = fmhrpaspr.fmhrpas.s.SAPIPAddress,
                              SAPDBName = fmhrpaspr.fmhrpas.s.SAPDBName,
                              SAPLicensePort = fmhrpaspr.fmhrpas.s.SAPLicensePort,
                              SAPDBPort = fmhrpaspr.fmhrpas.s.SAPDBPort,
                              SAPDBuser = fmhrpaspr.fmhrpas.s.SAPDBuser,
                              SAPDBPassword = fmhrpaspr.fmhrpas.s.SAPDBPassword,
                              SAPUser = fmhrpaspr.fmhrpas.s.SAPUser,
                              SAPPassword = fmhrpaspr.fmhrpas.s.SAPPassword,
                              PostSAP = fmhrpaspr.pr.PostSAP,
                              Module = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmh.fm.ModuleName
                          }).FirstOrDefault();
            return model;
        }

        public PostingViewModel GetPaymentCredentials()
        {
            PostingViewModel model = new PostingViewModel();
            model.CredentialDetails = _context.FieldMappings.Where(x => x.ModuleName.ToLower().Contains("payment")).Join(_context.Headers, fm => fm.MapId, h => h.MapId, (fm, h) => new { fm, h })
                          .Join(_context.Rows, fmh => fmh.fm.MapId, r => r.MapId, (fmh, r) => new { fmh, r })
                          .Join(_context.PathSetup, fmhr => fmhr.fmh.fm.PathCode, p => p.PathCode, (fmhr, p) => new { fmhr, p })
                          .Join(_context.AddonSetup, fmhrp => fmhrp.fmhr.fmh.fm.AddonCode, a => a.AddonCode, (fmhrp, a) => new { fmhrp, a })
                          .Join(_context.SAPSetup, fmhrpa => fmhrpa.fmhrp.fmhr.fmh.fm.SAPCode, s => s.SAPCode, (fmhrpa, s) => new { fmhrpa, s })
                          .Join(_context.Process, fmhrpas => fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.MapId, pr => pr.MapId, (fmhrpas, pr) => new { fmhrpas, pr })
                          .Join(_context.Schedules, fmhrpaspr => fmhrpaspr.pr.ProcessCode, sh => sh.Process, (fmhrpaspr, sh) => new PostingViewModel.Credential
                          {
                              MapId = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.MapId,
                              AddonCode = fmhrpaspr.fmhrpas.fmhrpa.a.AddonCode,
                              AddonDBName = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBName,
                              AddonIPAddress = fmhrpaspr.fmhrpas.fmhrpa.a.AddonIPAddress,
                              AddonPort = fmhrpaspr.fmhrpas.fmhrpa.a.AddonPort,
                              AddonServerName = fmhrpaspr.fmhrpas.fmhrpa.a.AddonServerName,
                              AddonDBuser = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBuser,
                              AddonDBPassword = fmhrpaspr.fmhrpas.fmhrpa.a.AddonDBPassword,
                              LocalPath = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.LocalPath,
                              FTPPath = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.RemotePath,
                              FTPUser = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.RemoteUserName,
                              FTPPass = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.RemotePassword,
                              BackupPath = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.p.BackupPath,
                              HeaderName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.h.TableName,
                              RowName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.r.TableName,
                              HeaderWorksheet = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.HeaderWorksheet,
                              RowWorksheet = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.RowWorksheet,
                              FileType = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.FileType,
                              FileName = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.FileName,
                              SAPServerName = fmhrpaspr.fmhrpas.s.SAPServerName,
                              SAPIPAddress = fmhrpaspr.fmhrpas.s.SAPIPAddress,
                              SAPDBName = fmhrpaspr.fmhrpas.s.SAPDBName,
                              SAPLicensePort = fmhrpaspr.fmhrpas.s.SAPLicensePort,
                              SAPDBPort = fmhrpaspr.fmhrpas.s.SAPDBPort,
                              SAPDBuser = fmhrpaspr.fmhrpas.s.SAPDBuser,
                              SAPDBPassword = fmhrpaspr.fmhrpas.s.SAPDBPassword,
                              SAPUser = fmhrpaspr.fmhrpas.s.SAPUser,
                              SAPPassword = fmhrpaspr.fmhrpas.s.SAPPassword,
                              PostSAP = fmhrpaspr.pr.PostSAP,
                              Module = fmhrpaspr.fmhrpas.fmhrpa.fmhrp.fmhr.fmh.fm.ModuleName
                          }).FirstOrDefault();
            return model;
        }

        public PostingViewModel GetSyncCredentials(string Task)
        {
            PostingViewModel model = new PostingViewModel();
            model.SyncCredsDetails = _context.SyncQueries.Join(_context.Queries, sq => sq.QueryId, q => q.Id, (sq, q) => new { sq, q })
                          .Join(_context.Syncs, sqq => sqq.sq.SyncId, s => s.Id, (sqq, s) => new { sqq, s })
                          .Join(_context.Schedules.Where(x => x.SchedCode == Task), sqqs => sqqs.sqq.sq.SyncQueryCode, sc => sc.Process, (sqqs, sc) => new PostingViewModel.SyncCreds
                          {
                              Path = sqqs.s.Path,
                              FileType = sqqs.s.FileType,
                              IpAddress = sqqs.s.IpAddress,
                              DbName = sqqs.s.DbName,
                              DbVersion = sqqs.s.DbVersion,
                              DbUser = sqqs.s.DbUser,
                              DbPass = sqqs.s.DbPass,
                              QueryString = sqqs.sqq.q.QueryString,
                              Code = sqqs.sqq.q.Code,
                              FtpPath = sqqs.s.FtpPath,
                              FtpUser = sqqs.s.FtpUser,
                              FtpPass = sqqs.s.FtpPass,

                          }).FirstOrDefault();
            return model;
        }

        public EmailViewModel GetEmailCredentials(string Task)
        {

            EmailViewModel model = new EmailViewModel();
            model.EmailCreds = _context.Schedules.AsEnumerable().Where(x => x.SchedCode == Task).Select(x => new EmailViewModel.EmailCredentials
            {
                SchedCode = x.Process,
                EmailCredCode = x.Credential,

            }).FirstOrDefault();
            if (model.EmailCreds != null)
            {
                var emailtemplate = _context.EmailTemplate.Where(x => x.Code == model.EmailCreds.SchedCode).FirstOrDefault();
                var emailsetup = _context.EmailSetup.Where(x => x.EmailCode == model.EmailCreds.EmailCredCode).FirstOrDefault();
                var filesetup = _context.Documents.AsEnumerable().Where(x => x.Id == Convert.ToDouble(emailtemplate.FileCode)).FirstOrDefault();
                var pathsetup = filesetup != null ? _context.PathSetup.Where(x => x.PathId.ToString() == filesetup.SavePath).FirstOrDefault() : null;
                model.EmailCreds.EmailFrom = emailsetup.Email;
                model.EmailCreds.EmailDesc = emailsetup.DisplayName;
                model.EmailCreds.EmailCc = string.Join(",", emailtemplate.CC);
                model.EmailCreds.EmailHost = emailsetup.SMTPClient;
                model.EmailCreds.EmailPassword = emailsetup.Password;
                model.EmailCreds.EmailPort = emailsetup.Port;
                model.EmailCreds.EmailSubject = emailtemplate.Subject;
                model.EmailCreds.EmailTo = string.Join(",", emailtemplate.To);
                //model.EmailCreds.EmailTo = "direc.raha@gmail.com";
                model.EmailCreds.Body = emailtemplate.Body;
                model.EmailCreds.ToQuery = emailtemplate.QueryTo;
                model.EmailCreds.CcQuery = emailtemplate.QueryCC;
                model.EmailCreds.QueryData = emailtemplate.QueryCode;
                model.EmailCreds.FilePath = filesetup != null ? filesetup.FilePath : "";
                model.EmailCreds.FileName = filesetup != null ? filesetup.FileName : "";
                model.EmailCreds.SavePath = pathsetup != null ? pathsetup.LocalPath : "";
                model.EmailCreds.FileCred = filesetup != null ? filesetup.Credential : "0";
                model.EmailCreds.Company = emailtemplate.Company;
                //model.EmailCreds = _context.Schedules.Where(x => x.SchedCode == Task).AsEnumerable().Join(_context.EmailTemplate, em => em.Process, a => a.Code, (em, a) => new { em, a }).AsEnumerable()
                //          .Join(_context.EmailSetup, ema => ema.em.Credential, sc => sc.EmailCode, (ema, sc) => new { ema, sc }).Join(_context.Documents,
                //          emasc => emasc.ema.a.FileCode, sched => sched.Code, (emasc, sched) => new { emasc, sched }).Join(_context.PathSetup, emascsched => emascsched.sched.SavePath, path => path.PathId.ToString(), (emascsched, p) => new EmailViewModel.EmailCredentials
                //          {
                //              EmailFrom = emascsched.emasc.sc.Email,
                //              EmailCc = string.Join(",", emascsched.emasc.ema.a.CC),
                //              EmailHost = emascsched.emasc.sc.SMTPClient,
                //              EmailPassword = emascsched.emasc.sc.Password,
                //              EmailPort = emascsched.emasc.sc.Port,
                //              EmailSubject = emascsched.emasc.ema.a.Subject,
                //              EmailTo = string.Join(",", emascsched.emasc.ema.a.To),
                //              Body = emascsched.emasc.ema.a.Body,
                //              ToQuery = emascsched.emasc.ema.a.QueryTo,
                //              CcQuery = emascsched.emasc.ema.a.QueryCC,
                //              QueryData = emascsched.emasc.ema.a.QueryCode,
                //              FilePath = emascsched.sched.FilePath,
                //              FileName = emascsched.sched.FileName,
                //              SavePath = p.LocalPath,
                //              FileCred = emascsched.sched.Credential,
                //          }).FirstOrDefault();
                if (model.EmailCreds != null)
                {

                    model.CompanyDetails = _context.CompanyDetails.AsEnumerable().Where(x => x.Id == Convert.ToDouble(model.EmailCreds.Company)).Select(x => new EmailViewModel.Company
                    {
                        CompanyName = x.CompanyName,
                        FilePath = x.FilePath,
                        Address = x.Address,
                        MobileNo = x.MobileNo,
                        TelNo = x.TelNo,
                        FileName = x.FileName
                    }).FirstOrDefault();
                    model.FileCredentials = _context.SAPSetup.AsEnumerable().Where(x => x.SAPId == Convert.ToDecimal(string.IsNullOrEmpty(model.EmailCreds.FileCred) ? "0" : model.EmailCreds.FileCred)).Select(x => new EmailViewModel.FileCredential
                    {
                        DbName = x.SAPDBName,
                        IpAddress = x.SAPIPAddress,
                        SapUser = x.SAPDBuser,
                        SapPassword = x.SAPDBPassword,
                        ServerName = x.SAPServerName
                    }).FirstOrDefault();

                    model.QueryDetails = _context.QueryManager.AsEnumerable().Where(x => x.Id == Convert.ToInt32(model.EmailCreds.ToQuery)).Select(x => new EmailViewModel.QuerySetup
                    {
                        ConnectionString = x.ConnectionString,
                        Id = x.Id,
                        ConnectionType = x.ConnectionType,
                        QueryCode = x.QueryCode,
                        QueryString = x.QueryString
                    }).FirstOrDefault();
                    model.ToTable = GetQueryData(model);

                    model.QueryDetails = _context.QueryManager.AsEnumerable().Where(x => x.Id == Convert.ToInt32(model.EmailCreds.CcQuery)).Select(x => new EmailViewModel.QuerySetup
                    {
                        ConnectionString = x.ConnectionString,
                        Id = x.Id,
                        ConnectionType = x.ConnectionType,
                        QueryCode = x.QueryCode,
                        QueryString = x.QueryString
                    }).FirstOrDefault();

                    model.CcTable = GetQueryData(model);

                    model.QueryDetails = _context.QueryManager.AsEnumerable().Where(x => x.Id == Convert.ToInt32(model.EmailCreds.QueryData)).Select(x => new EmailViewModel.QuerySetup
                    {
                        ConnectionString = x.ConnectionString,
                        Id = x.Id,
                        ConnectionType = x.ConnectionType,
                        QueryCode = x.QueryCode,
                        QueryString = x.QueryString
                    }).FirstOrDefault();

                    model.QueryTable = GetQueryData(model);

                }
            }
            return model;
        }

        public DataTable GetQueryData(EmailViewModel model)
        {
            try
            {
                if (model.QueryDetails != null)
                {
                    switch (model.QueryDetails.ConnectionType)
                    {
                        case "SAP":
                            model.DatabaseConnectionView = _context.SAPSetup
                                        .AsEnumerable()
                                        .Where(x => x.IsActive == true && x.SAPId == Convert.ToInt32(model.QueryDetails.ConnectionString))
                                        .Select((x) => new EmailViewModel.DbConnection
                                        {
                                            Id = x.SAPId,
                                            QueryName = x.SAPCode,
                                            ConnectionType = x.SAPDBVersion,
                                            ConnectionString = (x.SAPDBVersion.Contains("HANA") ? "DRIVER={HDBODBC32};" + $"SERVERNODE={x.SAPServerName}{(x.SAPDBPort > 0 ? $":{x.SAPDBPort}" : "")};UID={x.SAPDBuser};PWD={x.SAPDBPassword};CS={x.SAPDBName}" :
                                                                                                   $"Data Source={x.SAPServerName}{(x.SAPDBPort > 0 ? $":{x.SAPDBPort}" : "")};Initial Catalog={x.SAPDBName};Persist Security Info=True;User ID={x.SAPDBuser};Password={x.SAPDBPassword}")
                                        }).FirstOrDefault();
                            break;
                        default:
                            model.DatabaseConnectionView = _context.AddonSetup
                                        .AsEnumerable()
                                        .Where(x => x.IsActive == true && x.AddonId == Convert.ToInt32(model.QueryDetails.ConnectionString))
                                        .Select((x) => new EmailViewModel.DbConnection
                                        {
                                            Id = x.AddonId,
                                            QueryName = x.AddonCode,
                                            ConnectionType = x.AddonDBVersion,
                                            ConnectionString = (x.AddonDBVersion.Contains("HANA") ? "DRIVER={HDBODBC32};" + $"SERVERNODE={x.AddonServerName}{(x.AddonPort > 0 ? $":{x.AddonPort}" : "")};UID={x.AddonDBuser};PWD={x.AddonDBPassword};CS={x.AddonDBName}" :
                                                                                                   $"Data Source={x.AddonServerName}{(x.AddonPort > 0 ? $":{x.AddonPort}" : "")};Initial Catalog={x.AddonDBName};Persist Security Info=True;User ID={x.AddonDBuser};Password={x.AddonDBPassword}")
                                        }).FirstOrDefault();
                            break;
                    }
                    return Fill_DataTable(model);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public DataTable Fill_DataTable(EmailViewModel model)
        {
            try
            {
                DataTable dt = new DataTable();

                if (model.DatabaseConnectionView.ConnectionType.Contains("HANA"))
                {
                    dt = DataAccess.SelectHana(model.DatabaseConnectionView.ConnectionString, model.QueryDetails.QueryString);
                }
                else
                {
                    dt = DataAccess.Select(model.DatabaseConnectionView.ConnectionString, model.QueryDetails.QueryString);
                }

                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public PostingViewModel GetFields(int MapId)
        {
            PostingViewModel model = new PostingViewModel();
            model.HeaderFields = _context.Headers.Where(x => x.MapId == MapId).Select(x => new PostingViewModel.Fields
            {
                SAPFieldId = x.SAPHeaderFieldId,
                AddonField = x.AddonHeaderField
            }).ToList() ?? new List<PostingViewModel.Fields>();

            model.RowFields = _context.Rows.Where(x => x.MapId == MapId).Select(x => new PostingViewModel.Fields
            {
                SAPFieldId = x.SAPRowFieldId,
                AddonField = x.AddonRowField
            }).ToList() ?? new List<PostingViewModel.Fields>();

            return model;
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

        public PostingViewModel GetAPIPostSAP(int mapid)
        {
            var model = new PostingViewModel();

            foreach (var task in TaskScheduler.getRunningTask())
            {
                var Creds = GetAPICredentials(task);
                if (Creds.CredentialDetails != null)
                {
                    if (Creds.CredentialDetails.PostSAP && PostingHelpers.LoginAction(Creds))
                    {
                        if (Creds.CredentialDetails.Module.ToLower().Contains("sales order"))
                        {
                            model.APIView = _context.FieldMappings.Where(x => x.MapId == mapid)
                                       .Join(_context.APISetups, f => f.APICode, a => a.APICode, (f, a) => new { f, a })
                                       .Select(x => new PostingViewModel.APIViewModel
                                       {
                                           APIId = x.a.APIId,
                                           APICode = x.a.APICode,
                                           APIMethod = x.a.APIMethod,
                                           APIURL = x.a.APIURL,
                                           APIKey = x.a.APIKey,
                                           APISecretKey = x.a.APISecretKey,
                                           APIToken = x.a.APIToken,
                                           APILoginUrl = x.a.APILoginUrl,
                                           APILoginBody = x.a.APILoginBody,
                                       }).ToList();
                            var apimethod = model.APIView.Select(x => x.APIMethod).FirstOrDefault();
                            var apiurl = model.APIView.Select(x => x.APIURL).FirstOrDefault();
                            var apiuser = model.APIView.Select(x => x.APIKey).FirstOrDefault();
                            var apipwd = model.APIView.Select(x => x.APISecretKey).FirstOrDefault();
                            string ret = sapAces.APIResponse(apimethod, apiurl, "", "", "", apiuser, apipwd, 0); //Zero non limit
                            JObject json = JObject.Parse(ret);
                            var jheader = json["orders"];
                            model.APIdatas = JsonConvert.DeserializeObject<List<PostingViewModel.APIData>>(jheader.ToString());

                            model.HeaderFields = _context.Headers.Where(x => x.MapId == mapid).Select(x => new PostingViewModel.Fields
                            {
                                SAPFieldId = x.SAPHeaderFieldId,
                                AddonField = x.AddonHeaderField
                            }).ToList() ?? new List<PostingViewModel.Fields>();

                            model.RowFields = _context.Rows.Where(x => x.MapId == mapid).Select(x => new PostingViewModel.Fields
                            {
                                SAPFieldId = x.SAPRowFieldId,
                                AddonField = x.AddonRowField
                            }).ToList() ?? new List<PostingViewModel.Fields>();

                            foreach (var order in model.APIdatas)
                            {
                                var headqry = from i in model.APIdatas where i.id == order.id select i;
                                var rowsqry = from i in model.APIdatas where i.id == order.id select i.line_items;
                                string hjson = Newtonsoft.Json.JsonConvert.SerializeObject(headqry);
                                string rjson = Newtonsoft.Json.JsonConvert.SerializeObject(rowsqry);
                                DataTable dataheader = JsonConvert.DeserializeObject<DataTable>(hjson);
                                DataTable datarows = JsonConvert.DeserializeObject<DataTable>(rjson.Substring(1, rjson.Length - 2));
                                var posjson = PostingStringBuilderHelpers.BuildJsonAPIPost(dataheader, datarows, model.HeaderFields, model.RowFields);
                                var result = PostingHelpers.SBOResponse("POST", $@"Orders", posjson, "DocEntry", Creds);
                                if (!result.ToLower().Contains("error"))
                                {

                                }
                                else
                                {
                                    result = PostingHelpers.SBOResponse("POST", $@"Drafts", posjson, "DocEntry", Creds);
                                }
                            }

                        }
                    }
                }
            }

            return model;
        }
    }
}