namespace LinkBoxUI.Migrations
{
    using DataCipher;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LinkBoxUI.Context.LinkboxDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(LinkBoxUI.Context.LinkboxDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            if (!context.Users.Any(u => u.UserName == "Admin1"))
            {
                string _usr = "admin", _pword="1234";
                string passw = Cryption.Encrypt(_usr + _pword);
                context.Users.AddOrUpdate(
                    usr => usr.UserName,
                    new DomainLayer.User { UserName = _usr
                                        , Password = passw
                                        , CreateDate = DateTime.Now
                                        , LastName = "Admin"
                                        , MiddleName = "Top"
                                        , FirstName = "Role"                                           
                                        , IsActive = true
                                        , CreateUserID = 1
                                        , AuthorizationID = 1 }
                    );

                context.APISetups.AddOrUpdate(
                    t => t.APICode,
                    new DomainLayer.APISetup { APICode = "EBlast", APIMethod = "POST"
                                        , APIURL = "http://localhost:40710/Linkbox/Post"
                                        , IsActive = false, CreateUserID = 1, CreateDate = DateTime.Now  
                    });
                context.APISetups.AddOrUpdate(
                    t => t.APICode,
                    new DomainLayer.APISetup { APICode = "SAPDocPost", APIMethod = "POST"
                                        , APIURL = "http://localhost:40710/Linkbox/sap/post/documents"
                                        //, APIURL = "http://localhost:PORTNUM/Linkbox/sap/post/documents"
                                        , IsActive = true, CreateUserID = 1, CreateDate = DateTime.Now
                    });

                context.SaveChanges();
            }
        }
    }
}
