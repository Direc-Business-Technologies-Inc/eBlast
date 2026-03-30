using DomainLayer;
using DomainLayer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;
using System.Web;


namespace LinkBoxUI.Context
{
    public class LinkboxDb : IdentityDbContext<ApplicationUser>
    {
        public LinkboxDb() : base("SAOLinkBox")
        {
            //Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<UserAuthorization> UserAuthorizations { get; set; }
        public DbSet<UserModule> UserModules { get; set; }
        public DbSet<AuthorizationModule> AuthorizationModules { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<EmailLogs> Emails { get; set; }
        public DbSet<EmailSetup> EmailSetup { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<UploadSchedule> Schedules { get; set; }
        public DbSet<SAPSetup> SAPSetup { get; set; }
        public DbSet<AddonSetup> AddonSetup { get; set; }
        public DbSet<PathSetup> PathSetup { get; set; }
        public DbSet<FieldMapping> FieldMappings { get; set; }
        public DbSet<ProcessSetup> Process { get; set; }
        public DbSet<Header> Headers { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Sync> Syncs { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<QueryManager> QueryManager { get; set; }
        public DbSet<SyncQuery> SyncQueries { get; set; }
        public DbSet<APISetup> APISetups { get; set; }
        public DbSet<Paramenter> Paramenters { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<CompanyDetail> CompanyDetails { get; set; }
    }

}