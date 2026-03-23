using InfrastructureLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainLayer;
using LinkBoxUI.Context;
using DomainLayer.ViewModels;
using SAPbobsCOM;
using System.Data;
using DataAccessLayer.Class;

namespace LinkBoxUI.Services
{
    public class SyncServices : ISyncRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();
        public SyncViewModel View_Sync()
        {
            SyncViewModel sync = new SyncViewModel();
            List<string> SAPVersionList = new List<string>();
            sync.SyncView = _context.Syncs.Select(x => new SyncViewModel.Sync
            {
                Code = x.Code,
                Id = x.Id,
                DbName = x.DbName,
                DbUser = x.DbUser,
                Path = x.Path,
                FtpPath = x.FtpPath,
                FileType = x.FileType,
                IsActive = x.IsActive,
                DbVersion = x.DbVersion,
                IpAddress = x.IpAddress,
            }).ToList();

            sync.QueryView = _context.Queries.Select(x => new SyncViewModel.Query
            {
                Code = x.Code,
                QueryString = x.QueryString,
                Id = x.Id,
                IsActive = x.IsActive

            }).ToList();

            sync.SyncQueryView = _context.SyncQueries.Select(x => new SyncViewModel.SyncQuery
            {
                Id = x.Id,
                SyncQueryCode = x.SyncQueryCode,
                SyncCode = x.SyncCode,
                QueryCode = x.QueryCode,
                IsActive = x.IsActive,

            }).ToList();

            foreach (var item in Enum.GetValues(typeof(BoDataServerTypes)))
            {
                SAPVersionList.Add(item.ToString());
            }
            sync.SAPList = SAPVersionList.Select(x => new SyncViewModel.SAPVersion
            {
                SAPDBVersion = x.ToString(),

            }).ToList();

            return sync;
        }

        public bool Create_SyncSetup(Sync sync, string check, int id)
        {
            sync.Path = check == "FTP" ? "" : sync.Path;
            sync.FtpPath = check == "FTP" ? check : "";
            sync.FtpUser = check == "FTP" ? check : "";
            sync.FtpPass = check == "FTP" ? check : "";
            sync.IsActive = true;
            sync.CreateDate = DateTime.Now;
            sync.CreateUserID = id;
            _context.Syncs.Add(sync);
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public SyncViewModel Find_SyncSteup(int id)
        {

            var model = new SyncViewModel();

            model.SyncView = _context.Syncs.Where(x => x.Id == id).Select(x => new SyncViewModel.Sync

            {
                Code = x.Code,
                DbName = x.DbName,
                DbUser = x.DbUser,
                DbPass = x.DbPass,
                Path = x.Path,
                FtpPath = x.FtpPath,
                FtpUser = x.FtpUser,
                FtpPass = x.FtpPass,
                IsActive = x.IsActive,
                FileType = x.FileType,
                DbVersion = x.DbVersion,
                IpAddress = x.IpAddress,

            }).ToList();

            return model;
        }
        public bool Update_SyncSetup(Sync sync, string check, int id)
        {
            var Sync = sync;
            Sync.Path = check == "FTP" ? "" : sync.Path;
            Sync.FtpPath = check == "FTP" ? sync.FtpPath : "";
            Sync.FtpUser = check == "FTP" ? sync.FtpUser : "";
            Sync.FtpPass = check == "FTP" ? sync.FtpPass : "";
            Sync.UpdateDate = DateTime.Now;
            Sync.CreateDate = DateTime.Now;
            Sync.UpdateUserID = id;
            _context.Entry(Sync).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public bool Create_Query(Query query, int id)
        {
            query.IsActive = true;
            query.CreateDate = DateTime.Now;
            query.CreateUserID = id;
            _context.Queries.Add(query);
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public SyncViewModel Find_Query(int id)
        {
            var model = new SyncViewModel();

            model.QueryView = _context.Queries.Where(x => x.Id == id).Select(x => new SyncViewModel.Query
            {
                Code = x.Code,
                QueryString = x.QueryString,
                IsActive = x.IsActive,

            }).ToList();
            return model;
        }

        public bool Update_Query(Query query, int id)
        {

            var Query = query;
            Query.UpdateDate = DateTime.Now;
            Query.CreateDate = DateTime.Now;
            Query.UpdateUserID = id;
            _context.Entry(Query).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }


        public bool Create_SyncQuery(SyncQuery syncquery, int syncid, int queryid, int id)
        {
            string selectSyncCode = _context.Syncs.Where(x => x.Id == syncid).Select(x => x.Code).FirstOrDefault();
            string selectQueryCode = _context.Queries.Where(x => x.Id == queryid).Select(x => x.Code).FirstOrDefault();
            syncquery.SyncId = syncid;
            syncquery.SyncCode = selectSyncCode;
            syncquery.QueryId = queryid;
            syncquery.QueryCode = selectQueryCode;
            syncquery.IsActive = true;
            syncquery.CreateDate = DateTime.Now;
            syncquery.CreateUserID = id;
            _context.SyncQueries.Add(syncquery);
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public SyncViewModel Find_SyncQuery(int id)
        {
            var model = new SyncViewModel();
            model.SyncQueryView = _context.SyncQueries.Where(x => x.Id == id).Select(x => new SyncViewModel.SyncQuery
            {
                SyncQueryCode = x.SyncQueryCode,
                QueryCode = x.QueryCode,
                SyncCode = x.SyncCode,
                QueryId = x.QueryId,
                SyncId = x.SyncId,
                IsActive = x.IsActive,
            }).ToList();

            return model;
        }

        public bool Update_SyncQuery(SyncQuery syncquery, int id)
        {
            var syncQuery = _context.SyncQueries.Find(syncquery.Id);
            syncQuery.UpdateDate = DateTime.Now;
            syncQuery.UpdateUserID = id;
            syncQuery.IsActive = syncquery.IsActive;
            _context.Entry(syncQuery).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public SyncViewModel Validate_Sync(string code)
        {
            var model = new SyncViewModel();

            model.SyncView = _context.Syncs.Where(x => x.Code == code).Select((x) => new SyncViewModel.Sync
            {
                Code = x.Code,
            }).ToList();

            model.QueryView = _context.Queries.Where(x => x.Code == code).Select((x) => new SyncViewModel.Query
            {
                Code = x.Code,
            }).ToList();

            model.SyncQueryView = _context.SyncQueries.Where(x => x.SyncQueryCode == code).Select((x) => new SyncViewModel.SyncQuery
            {
                SyncQueryCode = x.SyncQueryCode,
            }).ToList();

            return model;

        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public SyncViewModel Get_SyncQuery(int id)
        {
            var model = new SyncViewModel();
            model.SyncQueryView = _context.SyncQueries.Where(a => a.Id == id).Join(_context.Syncs, a => a.SyncId, b => b.Id, (a, b) =>
           new
           {
               a,
               b
           }).Join(_context.Queries, ab => ab.a.QueryId, c => c.Id, (ab, c) => new SyncViewModel.SyncQuery
           {
               Id = ab.a.Id,
               SyncQueryCode = ab.a.SyncQueryCode,
               IsActive = ab.a.IsActive,
               QueryId = c.Id,
               QueryCode = c.Code,
               QueryString = c.QueryString,
               SyncId = ab.b.Id,
               SyncCode = ab.b.Code,
               Path = ab.b.Path,
               RemotePath = ab.b.FtpPath,
               RemoteUser = ab.b.FtpUser,
               RemotePassword = ab.b.FtpPass,
               DbName = ab.b.DbName,
               DbUser = ab.b.DbUser,
               DbPass = ab.b.DbPass,
               FileType = ab.b.FileType,
               IpAddress = ab.b.IpAddress,
               DbVersion = ab.b.DbVersion,

           }).ToList();
            return model;
        }

        public DataTable Fill_DataTable(SyncViewModel syncquery)
        {
            string message = "";
            DataTable dt = new DataTable();
            foreach (var item in syncquery.SyncQueryView)
            {
                if (item.DbVersion == "dst_HANADB")
                {
                    dt = SyncAccess.Execute(syncquery, (m) => message = m);
                }
                else if (item.DbVersion.Contains("MSSQL"))
                {
                    dt = SyncAccess.Execute2(syncquery, (m) => message = m);
                }
            }
            return dt;
        }

        public void Export(DataTable dt, SyncViewModel syncquery)
        {
            SyncAccess.ExportData(dt, syncquery);
        }
    }
}