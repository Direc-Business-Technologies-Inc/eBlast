using DomainLayer;
using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkBoxUI.Services
{
    public class ProcessServices : IProcessRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();

        public ProcessCreateViewModel View_Process()
        {
            ProcessCreateViewModel model = new ProcessCreateViewModel();
            model.ProcessView = _context.Process.Join(_context.FieldMappings, a => a.MapId, b => b.MapId, (a, b) => new ProcessCreateViewModel.process
            {
                ProcessId = a.ProcessId,
                ProcessCode = a.ProcessCode,
                FieldMappingCode = b.MapCode,
                FieldMappingName = b.MapName,
                ModName = b.ModuleName,
                ProcessName = a.ProcessName,
                PostSAP = a.PostSAP,
                IsActive = a.IsActive

            }).ToList();

            model.MapList = _context.FieldMappings.Select(x => new ProcessCreateViewModel.mapping
            {
                MapCode = x.MapCode,
                MapName = x.MapName

            }).ToList();

            model.EmailList = _context.Emails.Select(x => new ProcessCreateViewModel.Email
            {
                EmailId = x.EmailId,
                EmailCode = x.EmailCode

            }).ToList();

            model.APIList = _context.APISetups.Select(x => new ProcessCreateViewModel.API
            {
                APIId = x.APIId,
                APICode = x.APICode

            }).ToList();

            return model;
        }

        public bool Create_Process(ProcessSetup process, string map, int id)
        {
            var mapid = _context.FieldMappings.Where(x => x.MapCode == map).FirstOrDefault().MapId;
            process.MapId = mapid;
            process.IsActive = true;
            process.CreateDate = DateTime.Now;
            process.CreateUserID = id;
            _context.Process.Add(process);
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public ProcessCreateViewModel Find_Process(int id)
        {
            var model = new ProcessCreateViewModel();
            model.ProcessView = _context.Process.Where(x => x.ProcessId == id)
                                        .Join(_context.FieldMappings, a => a.MapId, b => b.MapId, (a, b) => 
                                        new ProcessCreateViewModel.process
                                        {
                                            ProcessId = a.ProcessId,
                                            ProcessCode = a.ProcessCode,
                                            FieldMappingCode = b.MapCode,
                                            FieldMappingName = b.MapName,
                                            ModName = b.ModuleName,
                                            ProcessName = a.ProcessName,
                                            IsActive = a.IsActive,
                                            PostSAP=a.PostSAP,
                                        }).ToList();
            return model;
        }

        public bool Update_Process(ProcessSetup process,string map,int id)
        {
            var mapid = _context.FieldMappings.Where(x => x.MapCode == map).FirstOrDefault().MapId;
            process.MapId = mapid;
            process.UpdateDate = DateTime.Now;
            process.UpdateUserID = id;
            _context.Entry(process).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            SaveChanges();
            return true;
        }

        public ProcessCreateViewModel Validate_Process(string code)
        {
            var model = new ProcessCreateViewModel();

            model.ProcessView = _context.Process.Where(x => x.ProcessCode == code).Select((x) => new ProcessCreateViewModel.process
            {
                ProcessCode = x.ProcessCode,
            }).ToList();
            return model;
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}