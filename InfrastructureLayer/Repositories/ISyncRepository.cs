using DomainLayer;
using DomainLayer.ViewModels;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositories
{
    public interface ISyncRepository
    {
        SyncViewModel View_Sync();

        int SaveChanges();
        bool Create_SyncSetup(Sync sync, string check, int id);

        SyncViewModel Find_SyncSteup(int id);
        bool Update_SyncSetup(Sync sync, string check, int id);
        bool Create_Query(Query query, int id);
        SyncViewModel Find_Query(int id);
        bool Update_Query(Query query, int id);
        bool Create_SyncQuery(SyncQuery syncquery,int syncid,int queryid, int id);
        SyncViewModel Find_SyncQuery(int id);

        bool Update_SyncQuery(SyncQuery syncquery, int id);

        SyncViewModel Validate_Sync(string code);

        SyncViewModel Get_SyncQuery(int id);
        DataTable Fill_DataTable(SyncViewModel syncquery);
        void Export(DataTable dt, SyncViewModel syncquery);
    }
}
