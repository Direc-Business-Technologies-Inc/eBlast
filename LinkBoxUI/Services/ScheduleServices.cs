using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainLayer;
using DataAccessLayer.Class;
using LinkBoxUI.Helpers;

namespace LinkBoxUI.Services
{
    public class ScheduleServices : IScheduleRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();

        public SchedCreateViewMdel View_Schedule()
        {
            SchedCreateViewMdel model = new SchedCreateViewMdel();
            model.SchedView = _context.Schedules.Select(x =>
                    new SchedCreateViewMdel.Schedule
                    {
                        SchedId = x.SchedId,
                        SchedCode = x.SchedCode,
                        Frequency = x.Frequency,
                        StartDate = x.StartDate.Value,
                        StartTime = x.StartTime.Value,
                        Process = x.Process,
                        ScheduleType = x.ScheduleType,
                        IsActive = x.IsActive

                    }).ToList();

            model.ProcessList = _context.Process.Select(x =>
                    new SchedCreateViewMdel.process
                    {
                        ProcessCode = x.ProcessCode
                    }).ToList();


            model.EmailList = _context.EmailTemplate.Select(x =>
                    new SchedCreateViewMdel.Email
                    {
                        EmailCode = x.Code
                    }).ToList();

            model.EmailCred = _context.EmailSetup.Select(x =>
                new SchedCreateViewMdel.Email
                {
                    EmailCode = x.EmailCode
                }).ToList();
            model.SyncList = _context.SyncQueries.Select(x =>
                    new SchedCreateViewMdel.Sync
                    {
                        SyncQueryCode = x.SyncQueryCode
                    }).ToList();

            model.APIList = _context.APISetups.Select(x => new SchedCreateViewMdel.API
            {
                APIId = x.APIId,
                APICode = x.APICode

            }).ToList();

            return model;
        }

        public void Create_Schedule(UploadSchedule schedule, int id)
        {
            schedule.CreateDate = DateTime.Now;
            schedule.IsActive = true;
            schedule.CreateUserID = id;
            _context.Schedules.Add(schedule);
            _context.SaveChanges();

            var api = _context.Process.Where(x => x.ProcessCode == schedule.Process).Join(_context.APISetups, p => p.APICode, a => a.APICode, (p, a) =>
                                         new { API = a.APIURL }).FirstOrDefault();
            string url = api == null ? _context.APISetups.Where(x => x.APICode == schedule.Api).Select(x => x.APIURL).FirstOrDefault() : api.API;
            var apimapcode = _context.Process.Where(x => x.ProcessCode == schedule.Process).Join(_context.FieldMappings, p => p.MapId, f => f.MapId, (p, f) =>
                                        new { APICode = "" }).FirstOrDefault();
            ////IF FIELD MAPPING IS API THEN SET THE MAPID            
            if (apimapcode != null)
            {
                var mapid = _context.Process.Where(x => x.ProcessCode == schedule.Process).Select(x => x.MapId).FirstOrDefault();
                url = $@"{url}?mapid={mapid}";
            }

            TaskSchedulerHelpers.AddSchedule(schedule.SchedCode, schedule.Frequency, schedule.StartDate.ToString(),
                                      schedule.StartTime.ToString(), schedule.ScheduleType, url);
        }
        public SchedCreateViewMdel Find_Schedule(int id, string type)
        {
            var model = new SchedCreateViewMdel();

            model.SchedView = _context.Schedules.Where(x => x.SchedId == id && x.ScheduleType == type).Select(a =>
                        new SchedCreateViewMdel.Schedule
                        {
                            SchedId = a.SchedId,
                            SchedCode = a.SchedCode,
                            Frequency = a.Frequency,
                            ScheduleType = a.ScheduleType,
                            StartDate = a.StartDate.Value,
                            StartTime = a.StartTime.Value,
                            Process = a.Process,
                            IsActive = a.IsActive,
                            Api = a.Api,
                            Credential = a.Credential
                        }).ToList();
            return model;
        }

        public bool Update_Schedule(UploadSchedule schedule, int id)
        {
            var sched = schedule;
            sched.UpdateDate = DateTime.Now;
            sched.CreateDate = DateTime.Now;
            sched.UpdateUserID = id;
            sched.CreateUserID = id;
            _context.Entry(sched).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            var api = _context.Process.Where(x => x.ProcessCode == schedule.Process).Join(_context.APISetups, p => p.APICode, a => a.APICode, (p, a) =>
                                       new { API = a.APIURL }).FirstOrDefault();
            string url = api == null ? _context.APISetups.Where(x => x.APICode == sched.Api).Select(x => x.APIURL).FirstOrDefault() : api.API;
            var apimapcode = _context.Process.Where(x => x.ProcessCode == schedule.Process).Join(_context.FieldMappings, p => p.MapId, f => f.MapId, (p, f) =>
                                        new { APICode = "" }).FirstOrDefault();
            ////IF FIELD MAPPING IS API THEN SET THE MAPID
            if (apimapcode != null) 
            {
                var mapid = _context.Process.Where(x => x.ProcessCode == schedule.Process).Select(x => x.MapId).FirstOrDefault();
                url = $@"{url}?mapid={mapid}";
            }

            if (schedule.IsActive)
            {
                TaskSchedulerHelpers.AddSchedule(schedule.SchedCode, schedule.Frequency,
                                          schedule.StartDate.ToString(), schedule.StartTime.ToString(), schedule.ScheduleType, url);
            }
            else
            {
                TaskSchedulerHelpers.UpdateSchedule(schedule.SchedCode);
            }
            return true;
        }

        public bool Run_Schedule(string Code)
        {
            try
            {
                TaskSchedulerHelpers.RunTask(Code);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        
        }

        public SchedCreateViewMdel Validate_Schedule(string code)
        {
            var model = new SchedCreateViewMdel();

            model.SchedView = _context.Schedules.Where(x => x.SchedCode == code).Select((x) => new SchedCreateViewMdel.Schedule
            {
                SchedCode = x.SchedCode,
            }).ToList();
            return model;
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}