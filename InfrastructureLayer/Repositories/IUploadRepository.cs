using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.ViewModels;
using DomainLayer;
using System.Data;

namespace InfrastructureLayer.Repositories
{
    public interface IUploadRepository
    {
        UploadViewModel View_Upload();
        UploadViewModel Get_Details(string map);
        bool Upload(string map);
        DataTable GetData(string Code);
    }
}
