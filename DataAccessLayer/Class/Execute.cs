using DomainLayer.ViewModels;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Class
{
    public class Execute
    {
        public static DataTable ExecuteQuery(SetupCreateViewModel model)
        {
            try
            {
                string query = "";
                if (model.SAPDbDetails.SAPDBVersion.ToLower().Contains("hana"))
                {
                    query = QueryAccess.GetItemStockHANA();
                    string conn = "DRIVER={HDBODBC32};SERVERNODE=" + model.SAPDbDetails.SAPIPAddress + ":30015;UID=" + model.SAPDbDetails.SAPDBuser + ";PWD=" + model.SAPDbDetails.SAPDBPassword + ";CS=" + model.SAPDbDetails.SAPDBName;
                    using (DataTable dt = new DataTable())
                    {
                        using (HanaConnection con = new HanaConnection(conn))
                        {
                            using (HanaCommand cmd = new HanaCommand(query, con))
                            {
                                HanaDataAdapter da = new HanaDataAdapter(cmd);
                                con.Open();
                                da.Fill(dt);
                                con.Close();
                                return dt;
                            }
                        }
                    }
                }
                else
                {
                    query = QueryAccess.GetItemStockMSSQL();
                    using (SqlConnection conn = new SqlConnection(@"Data Source=" + model.SAPDbDetails.SAPIPAddress + ";Initial Catalog=" + model.SAPDbDetails.SAPDBName + ";Persist Security Info=True;User ID="
                    + model.SAPDbDetails.SAPDBuser + ";Password=" + model.SAPDbDetails.SAPDBPassword))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
