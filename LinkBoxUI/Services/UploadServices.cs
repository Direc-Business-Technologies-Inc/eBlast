using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainLayer.ViewModels;
using DataAccessLayer.Class;
using LinkBoxUI.Helpers;

namespace LinkBoxUI.Services
{
    public class UploadServices : IUploadRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();

        //public UploadViewModel Get_Authorization(int id)
        //{
        //    var model = new UploadViewModel();
        //    model.AuthList = _context.Users.Join(_context.Authorizations, a => a.AuthorizationID, b => b.AuthId, (a, b) => new { a, b }).Where(a => a.a.UserId == id)
        //                                   .Join(_context.AuthorizationModules, ab => ab.b.AuthId, c => c.AuthId, (ab, c) => new { ab, c }).Where(c => c.c.IsActive == true && c.c.ModId == 5)
        //                                   .Join(_context.Modules, abc => abc.c.ModId, d => d.ModId, (abc, d) => new UploadViewModel.Authorization
        //                                   {
        //                                       authorization = d.ModName
        //                                   }).ToList();
        //    return model;
        //}

        //public UploadViewModel Get_Deposit()
        //{
        //    UploadViewModel model = new UploadViewModel();
        //    model.DepositList = _context.Deposits.Where(x => x.IsActive == true).Select(x => new UploadViewModel.Deposit
        //    {
        //        BrkId = x.BrkId,
        //        BrkCode = x.BrkCode,
        //        BrkDescription = x.BrkDescription
        //    }).ToList();
        //    return model;
        //}

        public UploadViewModel Get_Details(string map)
        {

            UploadViewModel model = new UploadViewModel();

           // model.Headers = _context.
            //var fieldpath = _context.FieldMappings.Join(_context.Headers, a => a.MapId, b => b.MapId, (a, b) => new { a, b }).Where(a => a.a.MapName == map)
            //                                      .Join(_context.Rows, ab => ab.a.MapId, c => c.MapId, (ab, c) => new { ab, c })
            //                                      .Join(_context.PathSetup, abc => abc.ab.a.PathCode, d => d.PathCode, (abc, d) => new { abc, d })
            //                                      .Join(_context.AddonSetup, abcd => abcd.abc.ab.a.AddonCode, e => e.AddonCode, (abcd, e) => new { abcd, e })
            //                                      .Join(_context.SAPSetup, abcde => abcde.abcd.abc.ab.a.SAPCode, f => f.SAPCode, (abcde, f) => new
            //                                      {
            //                                          MapId = abcde.abcd.abc.ab.a.MapId,
            //                                          AddonDBName = abcde.e.AddonDBName,
            //                                          HeaderName = abcde.abcd.abc.ab.b.TableName,
            //                                          RowName = abcde.abcd.abc.c.TableName

            //                                      }).FirstOrDefault();

            //DataTable AddonData = DataAccess.Select(Properties.Settings.Default.Constr, $"sp_GetFieldsAndValue {fieldpath.AddonDBName}," +
            //                                       $"{fieldpath.HeaderName},{fieldpath.RowName},{fieldpath.MapId},3");


            //model.UploadList = AddonData.AsEnumerable().Select(x => new UploadViewModel.Upload
            //{
            //    TransactionId = Convert.ToInt32(x[0].ToString()),
            //    BranchCode = x[1].ToString(),
            //    TranDate = Convert.ToDateTime(x[2].ToString()).ToString("MM/dd/yyyy"),
            //    CashierName = x[4].ToString(),
            //    Status = x[5].ToString(),
            //    Message = x[6].ToString()

            //}).ToList();

            //return model;
            return model;
        }

        public UploadViewModel View_Upload()
        {
            var model = new UploadViewModel();
            model.SAPList = _context.FieldMappings.Select(x => new UploadViewModel.SAPConfig
            { Code = x.MapCode }).ToList();

            return model;
        }


        public DataTable GetData(string Code)
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
                var AddonSettings = _context.AddonSetup.Where(x => x.AddonCode == config.FieldMapDetails.AddonCode).FirstOrDefault();

                var header = _context.Headers.Where(x => x.MapId == config.FieldMapDetails.MapId).FirstOrDefault();
                var rows = _context.Rows.Where(x => x.MapId == config.FieldMapDetails.MapId).FirstOrDefault();
                var con = QueryAccess.con2(AddonSettings.AddonDBName, AddonSettings.AddonIPAddress, 40000, AddonSettings.AddonServerName, AddonSettings.AddonDBuser, AddonSettings.AddonDBPassword);

                table = DataAccess.Select(con, $@"SELECT * FROM {header.TableName} WHERE Status IN ('O','E')");
            }

            return table;
        }


        public bool Upload(string code)
        {
            bool ret;
            if (code != "" || code != string.Empty)
            {
                TaskSchedulerHelpers.RunTask(code);

                ret = true;
            }
            else
            {
                ret = false;
            }

            return ret ;
        }
    }
}