using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer.Class
{

    public class QueryAccess
    {
        public static string con(string AddonDBName, string IPAddress, int? Port, string Server, string User, string Password)
        {
            //create database if not exist
            SqlConnection cnn;
            cnn = new SqlConnection($@"Data Source={IPAddress};Initial Catalog=master;User ID={User};Password={Password}");
            string sqlCreateDBQuery = $"SELECT database_id FROM sys.databases WHERE Name = '{AddonDBName}'";
            using (SqlCommand cmd = new SqlCommand(sqlCreateDBQuery, cnn))
            {
                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        string Createscript = $"CREATE DATABASE {AddonDBName}";
                        using (SqlCommand cmdcreate = new SqlCommand(Createscript, cnn))
                        {
                            reader.Close();
                            cmdcreate.ExecuteNonQuery();
                        }
                    }
                }
                cnn.Close();
            }
            return $@"Data Source={IPAddress};Initial Catalog={AddonDBName};User ID={User};Password={Password}";
        }

        public static string con2(string AddonDBName, string IPAddress, int? Port, string Server, string User, string Password)
        { return $@"Data Source={IPAddress};Initial Catalog={AddonDBName};User ID={User};Password={Password}"; }

        public static int getTransId(string constr, string headtbname, string dbname)
        {
            SqlConnection cnn;
            cnn = new SqlConnection(constr);
            int TransId = 0;
            string GetId = $"SELECT ISNULL(MAX(TransactionId), 0) FROM[{dbname}].[dbo].[{headtbname}]";
            using (SqlCommand cmd = new SqlCommand(GetId, cnn))
            {

                cnn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            TransId = reader.GetInt32(0);

                        }
                    }
                }
                cnn.Close();
            }
            return TransId + 1;
        }

        public static void GenerateTable(string AddonDB, string constr, List<string[]> HeaderVal, List<string[]> RowVal, string RowTable, string HeaderTable, int HeadCount, int RowCount)
        {
            var NewHead = HeaderVal;
            var NewRow = RowVal;

            for (int i = 0; i < HeadCount; i++)
            {
                NewHead.RemoveAt(0);
            }

            for (int i = 0; i < RowCount; i++)
            {
                NewRow.RemoveAt(0);
            }
            string query = "", rowquery = "", headerquery = "";
            SqlConnection cnn;
            cnn = new SqlConnection(constr);
            if (NewHead != null || NewRow != null)
            {
                if (NewHead != null && NewHead.Count != 0 && HeadCount != 0)
                {
                    foreach (var item in NewHead)
                    {
                        string length = item[2].Contains("Int") || item[2].Contains("Date") ? "" : $"({item[3]})";
                        headerquery += $" [{item[1]}] [{item[2]}]{length}";
                    }
                    string addheader = $"alter table [{HeaderTable}] add {headerquery} ";

                    using (SqlCommand cmd = new SqlCommand(addheader, cnn))
                    {
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }

                if (NewRow != null && NewRow.Count != 0 && RowCount != 0)
                {
                    foreach (var item in NewRow)
                    {
                        string length = item[2].Contains("Int") || item[2].Contains("Date") ? "" : $"({item[3]})";
                        rowquery += $" [{item[1]}] [{item[2]}]{length}";
                    }
                    string addRow = $"alter table [{RowTable}] add {rowquery} ";

                    using (SqlCommand cmd = new SqlCommand(addRow, cnn))
                    {
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
            }

            if (HeaderTable != null && HeaderTable != "")
            {

                string checktable = $"SELECT * FROM {AddonDB}.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{HeaderTable}'";
                using (SqlCommand cmd = new SqlCommand(checktable, cnn))
                {
                    cnn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            foreach (var item in HeaderVal)
                            {
                                string length = item[2].Contains("INT") || item[2].Contains("DATE") ? "" : $"({item[3]})";
                                query += $" [{item[1]}] [{item[2]}]{length} {item[4]},";
                            }
                            headerquery = $"USE [{AddonDB}] CREATE TABLE[dbo].[{HeaderTable}]([Id] [int] IDENTITY(1,1) NOT NULL,[TransactionId] [int],{query}[UploadDate] [datetime],[Status] [char](1),[Message] [nvarchar](255) CONSTRAINT [PK_dbo.{HeaderTable}] " +
                            $"PRIMARY KEY CLUSTERED([Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";

                            using (SqlCommand cmd1 = new SqlCommand(headerquery, cnn))
                            {
                                reader.Close();
                                cmd1.ExecuteNonQuery();
                            }
                        }
                    }
                    cnn.Close();
                }


            }
            query = ""; 
            if (RowTable != null && RowTable != "")
            {
                

                string checktable = $"SELECT * FROM {AddonDB}.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{RowTable}'";
                using (SqlCommand cmd = new SqlCommand(checktable, cnn))
                {
                    cnn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            foreach (var item in RowVal)
                            {
                                string length = item[2].Contains("INT") || item[2].Contains("DATE") ? "" : $"({item[3]})";
                                query += $" [{item[1]}] [{item[2]}]{length} {item[4]},";
                            }
                            rowquery = $"USE [{AddonDB}] CREATE TABLE[dbo].[{RowTable}]([Id] [int] IDENTITY(1,1) NOT NULL,[TransactionId] [int],{query}[Status] [char](1),[Message] [nvarchar](255) CONSTRAINT [PK_dbo.{RowTable}] " +
                            $"PRIMARY KEY CLUSTERED([Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";

                            using (SqlCommand cmd1 = new SqlCommand(rowquery, cnn))
                            {
                                reader.Close();
                                cmd1.ExecuteNonQuery();
                            }
                        }
                    }
                    cnn.Close();
                }
            }


        }

        public static string HANA_conString(string Server, string User, string Password, string Database)
        { return "DRIVER={HDBODBC32};SERVERNODE=" + Server + ":30015;UID=" + User + ";PWD=" + Password + ";CS=" + Database; }


        public static string MSSQL_conString(string Server, string User, string Password, string Database)
        { return "Server=" + Server + ";Database=" + Database + ";User Id=" + User + ";Password=" + Password; }


        public static Tuple<List<string>, List<string>> connHana(string constr, List<string> tables, string dbname, string dbtype)
        {
            string rowquery = "", headerquery = "";
            List<string> Headerfields = new List<string>();
            List<string> Rowfields = new List<string>();
            if (dbtype.Contains("HANA"))
            {
                HanaConnection cnn;
                cnn = new HanaConnection(constr);
                headerquery = $@"SELECT COLUMN_NAME FROM SYS.COLUMNS WHERE SCHEMA_NAME = '{dbname}' AND TABLE_NAME = '{tables[0]}' 
                                 UNION ALL 
                                 SELECT COLUMN_NAME FROM SYS.COLUMNS WHERE SCHEMA_NAME = '{dbname}' AND TABLE_NAME = '{tables[1]}'";

                //rowquery = $"SELECT COLUMN_NAME FROM SYS.COLUMNS WHERE SCHEMA_NAME = '{dbname}' AND TABLE_NAME = '{tables[1]}' ORDER BY COLUMN_NAME";
                using (HanaCommand cmd = new HanaCommand(headerquery, cnn))
                {
                    cmd.Connection.Open();
                    HanaDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        Headerfields.Add(read.GetString(0));
                    }
                    read.Close();
                    cmd.Connection.Close();
                }
                using (HanaCommand cmd = new HanaCommand(headerquery, cnn))
                {
                    cmd.Connection.Open();
                    HanaDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        Rowfields.Add(read.GetString(0));
                    }
                    read.Close();
                    cmd.Connection.Close();
                }
            }
            else
            {
                SqlConnection cnn;
                cnn = new SqlConnection(constr);
                headerquery = $@"SELECT COLUMN_NAME FROM {dbname}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{dbname}' AND TABLE_NAME = '{tables[0]}' 
                                UNION ALL 
                                SELECT COLUMN_NAME FROM {dbname}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{dbname}' AND TABLE_NAME = '{tables[1]}' ";

                //rowquery = $@"SELECT COLUMN_NAME FROM {dbname}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{dbname}' AND TABLE_NAME = '{tables[1]}' ORDER BY COLUMN_NAME";
                using (SqlCommand cmd = new SqlCommand(headerquery, cnn))
                {
                    cmd.Connection.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        Headerfields.Add(read.GetString(0));
                    }
                    read.Close();
                    cmd.Connection.Close();
                }
                using (SqlCommand cmd = new SqlCommand(headerquery, cnn))
                {
                    cmd.Connection.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        Rowfields.Add(read.GetString(0));
                    }
                    read.Close();
                    cmd.Connection.Close();
                }
            }

            return Tuple.Create(Headerfields, Rowfields);
        }


        public static void UploadtoAddon(string con, Tuple<DataTable, DataTable> Data, string HeaderName, string RowName)
        {
            SqlConnection cnn;
            cnn = new SqlConnection(con);
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(cnn))
            {
                //Set the database table name
                sqlBulkCopy.DestinationTableName = $"dbo.{HeaderName}";
                foreach (var col in Data.Item1.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(col.ToString(), col.ToString());
                }
                cnn.Open();
                sqlBulkCopy.WriteToServer(Data.Item1);
                cnn.Close();
            }

            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(cnn))
            {
                //Set the database table name
                sqlBulkCopy.DestinationTableName = $"dbo.{RowName}";
                foreach (var col in Data.Item2.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(col.ToString(), col.ToString());
                }
                cnn.Open();
                sqlBulkCopy.WriteToServer(Data.Item2);
                cnn.Close();
            }
        }
        public static bool CheckCon(string con, string hanaorsql)
        {
            if (hanaorsql.Contains("HANA"))
            {
                HanaConnection cnn;
                cnn = new HanaConnection(con);

                try
                {
                    cnn.Open();
                    return true;
                }
                catch
                {
                    cnn.Close();
                    return false;
                }
            }
            else
            {
                SqlConnection cnn;
                cnn = new SqlConnection(con);
                try
                {
                    cnn.Open();
                    return true;
                }
                catch
                {
                    cnn.Close();
                    return false;
                }
            }
        }

        public static Tuple<DataTable, DataTable> getDataType(string constr, List<string> tables, string dbname, string dbtype, string Field)
        {
            string rowquery = "", headerquery = "";
            //List<string> Headerfields = new List<string>();
            //List<string> Rowfields = new List<string>();
            DataTable HeaderDataType = new DataTable();
            DataTable RowDataType = new DataTable();
            if (dbtype.Contains("HANA"))
            {
                HanaConnection cnn;
                cnn = new HanaConnection(constr);
                headerquery = $"SELECT DATA_TYPE_NAME,LENGTH,SCALE FROM SYS.COLUMNS WHERE SCHEMA_NAME = '{dbname}' AND TABLE_NAME = '{tables[0]}' AND COLUMN_NAME = '{Field}' " +
                              $"UNION ALL " +
                              $"SELECT DATA_TYPE_NAME,LENGTH,SCALE FROM SYS.COLUMNS WHERE SCHEMA_NAME = '{dbname}' AND TABLE_NAME = '{tables[1]}' AND COLUMN_NAME = '{Field}' ";
                                
                //rowquery = $"SELECT DATA_TYPE_NAME,LENGTH,SCALE FROM SYS.COLUMNS WHERE SCHEMA_NAME = '{dbname}' AND TABLE_NAME = '{tables[1]}' AND COLUMN_NAME = '{Field}'";

                using (HanaCommand cmd = new HanaCommand(headerquery, cnn))
                {
                    HanaDataAdapter da = new HanaDataAdapter(cmd);
                    cmd.Connection.Open();
                    da.Fill(HeaderDataType);
                    cmd.Connection.Close();
                }
                using (HanaCommand cmd = new HanaCommand(/*rowquery*/headerquery, cnn))
                {
                    HanaDataAdapter da = new HanaDataAdapter(cmd);
                    cmd.Connection.Open();
                    da.Fill(RowDataType);
                    cmd.Connection.Close();
                }
            }
            else
            {
                SqlConnection cnn;
                cnn = new SqlConnection(constr);
                headerquery = $"SELECT DATA_TYPE_NAME,LENGTH FROM {dbname}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{dbname}' AND TABLE_NAME = '{tables[0]}' " +
                              $"UNION ALL" +
                              $"SELECT DATA_TYPE_NAME,LENGTH FROM {dbname}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{dbname}' AND TABLE_NAME = '{tables[1]}' ";

                //rowquery = $"SELECT DATA_TYPE_NAME,LENGTH FROM {dbname}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{dbname}' AND TABLE_NAME = '{tables[1]}' ";
                using (SqlCommand cmd = new SqlCommand(headerquery, cnn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    cmd.Connection.Open();
                    da.Fill(HeaderDataType);
                    cmd.Connection.Close();
                }
                using (SqlCommand cmd = new SqlCommand(/*rowquery*/headerquery, cnn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    cmd.Connection.Open();
                    da.Fill(RowDataType);
                    cmd.Connection.Close();
                }
            }

            return Tuple.Create(HeaderDataType, RowDataType);
        }

        public static string GetItemStockMSSQL()
        {
            return $@"select T1.ItemCode,T1.ItemName, sum(T0.InQty-T0.OutQty) AS Stock,T1.SalUnitMsr , X.DocDate
                        from OINM T0 
	                        inner join OITM T1 on T0.ItemCode=T1.ItemCode
	                        , (select top 1 DocDate from OINV 
				                        order by DocEntry desc) as X 
                        where T1.SellItem='Y'
                        Group by T1.ItemCode,T1.ItemName,T1.SalUnitMsr, X.DocDate";
        }

        public static string GetItemStockHANA()
        {
            return $@"select T1.""ItemCode"",T1.""ItemName"", sum(T0.""InQty""-T0.""OutQty"") AS ""Stock"" ,T1.""SalUnitMsr"",T2.""Price"" 
                        from OINM T0 
	                        inner join OITM T1 on T0.""ItemCode""=T1.""ItemCode"" inner join ITM1 T2 ON T1.""ItemCode"" = T2.""ItemCode"" 
                        where T1.""SellItem""='Y' AND T2.""PriceList"" = 2 
                        Group by T1.""ItemCode"",T1.""ItemName"",T1.""SalUnitMsr"",T2.""Price""";
        }

        public static string UpdateTransaction(string DbName,string Table,string Status,string Message,string Id)
        {
            return $@"UPDATE {DbName}.dbo.{Table} SET UploadDate = GetDate() , Status = '{Status}' ,Message = '{Message}' WHERE TransactionId = '{Id}'";
        }

        public static string CheckItem(string BarCode)
        {
            return $@"SELECT ""ItemCode"" FROM OITM WHERE ""ItemCode"" = '{BarCode}'";
        }

        public static string GetTopOrder()
        {
            return $@"SELECT TOP 1 ""DocEntry"" FROM ORDR ORDER BY ""DocEntry"" DESC; ";
        }

    }
}