using DomainLayer;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositories
{
    public interface IMappingRepository
    {
        MapCreateViewModel View_Mapping();
        MapCreateViewModel View_Query();
        MapCreateViewModel Find_Map(int id);
        MapCreateViewModel ValidateMap(string code);
        SetupCreateViewModel.AddonViewModel Select_AddonSetup(string code);
        void Create_FieldMapping(FieldMapping field, int id);
        void Update_FieldMapping(FieldMapping field, int id);
        void Create_Headers(List<string[]> headers, int id, string table, int check, int newid, int mapid);
        void Create_Rows(List<string[]> rows, int id, string table, int check, int newid, int mapid);
        List<string[]> NewFieldValue(List<string[]> list, int count);

        string Get_Constring(SetupCreateViewModel.AddonViewModel AddonPath);
        void GenerateTable(string database, string constring, List<string[]> headerfield, List<string[]> rowfield,
                                  string rowrname, string headername, int headercount, int rowcount);

        MapCreateViewModel Populate(string table, string code,string header,string row);
        MapCreateViewModel PopulateAPI(string table, string code, string header, string row, string APICode);
        MapCreateViewModel Get_DataType(string table, string code, string field);
        SetupCreateViewModel.SAPViewModel Select_SAPSetup(string code);
        int SaveChanges();
    }
}
