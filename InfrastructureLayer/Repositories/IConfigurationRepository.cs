using DomainLayer;
using DomainLayer.Models;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositories
{
    public interface IConfigurationRepository
    {
        SetupCreateViewModel View_Setup();
        void Create_AddonSetup(AddonSetup addon, int id);
        SetupCreateViewModel Find_Addon(int id);
        SetupCreateViewModel Find_ConnectionString(string Type);
        SetupCreateViewModel GetConnection(string Type, string ConString);
        DataTable Fill_DataTable(SetupCreateViewModel model, string Query);
        SetupCreateViewModel ValidateCode(string code);
        bool Update_AddonSetup(AddonSetup addon, int id);
        void Create_SapSetup(SAPSetup addon, int id);
        void Create_Query(QueryManager addon, int id);
        SetupCreateViewModel Find_SAP(int id);
        bool Update_SapSetup(SAPSetup addon, int id);
        void Create_PathSetup(PathSetup path, int id);
        SetupCreateViewModel Find_Path(int id);
        SetupCreateViewModel Find_File(int id);
        SetupCreateViewModel FindCompany(int id);
        bool Update_PathSetup(PathSetup path, int id);
        SetupCreateViewModel Find_Addon(string code);
        SetupCreateViewModel Find_SAP(string code);
        SetupCreateViewModel Find_Path(string code);
        void Create_APISetup(APISetup addon, int id);
        void Create_EmailSetup(EmailSetup addon, int id);
        SetupCreateViewModel Find_API(int id);
        SetupCreateViewModel Find_Email(int id);
        SetupCreateViewModel Find_Query(int id);
        bool Update_EmailSetup(EmailSetup addon, int id);

        bool Update_APISetup(APISetup addon, int id);
        bool Update_Query(QueryManager addon, int id);
        int SaveChanges();
    }
}
