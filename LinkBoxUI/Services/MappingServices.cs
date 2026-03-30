using DomainLayer;
using LinkBoxUI.Context;
using InfrastructureLayer.Repositories;
using DomainLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DataAccessLayer.Class;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace LinkBoxUI.Services
{
    public class MappingServices : IMappingRepository
    {
        private readonly LinkboxDb _context = new LinkboxDb();
        private SAPAccess sapAces = new SAPAccess();
        private GlobalServices globalServices = new GlobalServices();
        public MapCreateViewModel View_Mapping()
        {
            MapCreateViewModel model = new MapCreateViewModel();
            model.FieldView = _context.FieldMappings.Select(x => new MapCreateViewModel.FieldMapping
            {
                MapId = x.MapId,
                MapCode = x.MapCode,
                MapName = x.MapName,
                AddonCode = x.AddonCode,
                SAPCode = x.SAPCode,
                PathCode = x.PathCode,
                HeaderWorksheet = x.HeaderWorksheet,
                RowWorksheet = x.RowWorksheet,
                FileName = x.FileName,
                FileType = x.FileType,
                ModuleName = x.ModuleName,
                DataType = x.DataType
            }).ToList();

            model.SAPCodeList = _context.SAPSetup.Where(x => x.IsActive == true).Select(x => new MapCreateViewModel.SAPCodes
            {
                SAPId = x.SAPId,
                SAPCode = x.SAPCode,
                SAPDBVersion = x.SAPDBVersion,
                SAPServerName = x.SAPServerName,
                SAPIPAddress = x.SAPIPAddress,
                SAPDBName = x.SAPDBName,
                SAPVersion = x.SAPVersion,
                SAPDBPort = x.SAPDBPort,
                SAPDBuser = x.SAPDBuser,
                SAPLicensePort = x.SAPLicensePort,
                SAPDBPassword = x.SAPDBPassword,
                SAPUser = x.SAPUser,
                SAPPassword = x.SAPPassword,


            }).ToList();

            model.AddonCodeList = _context.AddonSetup.Where(x => x.IsActive == true).Select(x => new MapCreateViewModel.AddonCodes
            {
                AddonId = x.AddonId,
                AddonCode = x.AddonCode,
                AddonDBVersion = x.AddonDBVersion,
                AddonServerName = x.AddonServerName,
                AddonIPAddress = x.AddonIPAddress,
                AddonDBName = x.AddonDBName,
                AddonPort = x.AddonPort,
                AddonDBuser = x.AddonDBuser,
                AddonDBPassword = x.AddonDBPassword,

            }).ToList();

            model.PathCodeList = _context.PathSetup.Where(x => x.IsActive == true).Select(x => new MapCreateViewModel.PathCodes
            {
                PathId = x.PathId,
                PathCode = x.PathCode,
                LocalPath = x.LocalPath,
                BackupPath = x.BackupPath,
                RemotePath = x.RemotePath,
                RemoteIPAddress = x.RemoteIPAddress,
                RemotePort = x.RemotePort,
                RemoteServerName = x.RemoteServerName,
                RemoteUser = x.RemoteUserName,
                RemotePassword = x.RemotePassword

            }).ToList();

            model.APIView = _context.APISetups.Where(x => x.IsActive == true).Select(x => new MapCreateViewModel.APIViewModel
            {
                APIId = x.APIId,
                APICode = x.APICode,
                APIMethod = x.APIMethod,
                APIURL = x.APIURL,
                //APIKey = x.APIKey,
                //APISecretKey = x.APISecretKey,
                //APIToken = x.APIToken,
            }).ToList();
            return model;
        }

        public MapCreateViewModel View_Query()
        {
            MapCreateViewModel model = new MapCreateViewModel();
            model.QueryManagerView = _context.QueryManager.Select(x => new MapCreateViewModel.QueryManager
            {
                Id = x.Id,
                QueryCode = x.QueryCode,
                QueryName = x.QueryName,
                QueryString = x.QueryString,
                IsActive = x.IsActive,
                CreateDate = x.CreateDate,
                CreateUserId = x.CreateUserId
            }).ToList();

            return model;
        }
        public MapCreateViewModel Find_Map(int id)
        {
            var model = new MapCreateViewModel();
            model.FieldView = _context.FieldMappings
                                .Where(x => x.MapId == id)
                                .Select((x) => new MapCreateViewModel.FieldMapping
                                {
                                    MapId = x.MapId,
                                    MapCode = x.MapCode,
                                    MapName = x.MapName,
                                    AddonCode = x.AddonCode,
                                    SAPCode = x.SAPCode,
                                    ModuleName = x.ModuleName,
                                    PathCode = x.PathCode,
                                    HeaderWorksheet = x.HeaderWorksheet,
                                    RowWorksheet = x.RowWorksheet,
                                    FileName = x.FileName,
                                    FileType = x.FileType,
                                    //APICode = x.APICode,
                                }).ToList();

            model.Headers = _context.Headers
                              .Where(x => x.MapId == id)
                              .Select((x) => new MapCreateViewModel.Header
                              {

                                  TableName = x.TableName,
                                  //SAPHeaderFieldId = x.SAPHeaderFieldId,
                                  AddonHeaderField = x.AddonHeaderField,
                                  DataType = x.DataType,
                                  Length = x.Length,
                                  IsRequired = x.IsRequired,
                                  //DefaultValue = x.DefaultValue,
                              }).ToList();

            model.Rows = _context.Rows
                           .Where(x => x.MapId == id)
                           .Select((x) => new MapCreateViewModel.Row
                           {
                               TableName = x.TableName,
                               //SAPRowFieldId = x.SAPRowFieldId,
                               AddonRowField = x.AddonRowField,
                               DataType = x.DataType,
                               Length = x.Length,
                               IsRequired = x.IsRequired,
                               //DefaultValue = x.DefaultValue,
                           }).ToList();



            return model;
        }
        public MapCreateViewModel ValidateMap(string code)
        {

            var model = new MapCreateViewModel();

            model.FieldView = _context.FieldMappings
                                .Where(x => x.MapCode == code)
                                .Select((x) => new MapCreateViewModel.FieldMapping
                                {
                                    MapCode = x.MapCode,
                                }).ToList();
            return model;
        }

        public SetupCreateViewModel.AddonViewModel Select_AddonSetup(string code)
        {
            return _context.AddonSetup.Where(x => x.AddonCode == code).Select((x) => new SetupCreateViewModel.AddonViewModel
            {
                AddonDBName = x.AddonDBName,
                AddonIPAddress = x.AddonIPAddress,
                AddonPort = x.AddonPort,
                AddonServerName = x.AddonServerName,
                AddonDBuser = x.AddonDBuser,
                AddonDBPassword = x.AddonDBPassword
            }).FirstOrDefault();
        }

        public SetupCreateViewModel.SAPViewModel Select_SAPSetup(string code)
        {
            return _context.SAPSetup.Where(x => x.SAPCode == code).Select((x) => new SetupCreateViewModel.SAPViewModel
            {
                SAPCode = x.SAPCode,
                SAPServerName = x.SAPServerName,
                SAPIPAddress = x.SAPIPAddress,
                SAPDBPort = x.SAPDBPort,
                SAPDBuser = x.SAPDBuser,
                SAPDBPassword = x.SAPDBPassword,
                SAPDBName = x.SAPDBName,
                SAPDBVersion = x.SAPDBVersion,
                SAPUser = x.SAPUser,
                SAPPassword = x.SAPPassword,
                SAPLicensePort = x.SAPLicensePort,                
            }).FirstOrDefault();
        }

        public void Create_FieldMapping(FieldMapping field, int id)
        {
            field.MapId = _context.FieldMappings.Count();
            field.CreateDate = DateTime.Now;
            field.CreateUserID = id;
            _context.FieldMappings.Add(field);
            _context.SaveChanges();
        }

        public void Update_FieldMapping(FieldMapping field, int id)
        {
            var map = field;
            map.UpdateUserID = id;
            map.UpdateDate = DateTime.Now;
            _context.Entry(map).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        public void Create_Headers(List<string[]> headers, int id, string table, int check, int newid, int mapid)
        {
            foreach (var item in headers)
            {
                var _id = (check == 1 ? mapid : newid);
                var fieldid = item[0].ToString();
                var dtexist = _context.Headers.Where(x => x.MapId == _id).Any();
                if (dtexist)
                {
                    var headerdata = _context.Headers.Where(x => x.MapId == _id).FirstOrDefault();
                    headerdata.MapId = check == 1 ? mapid : newid;
                    //headerdata.SAPHeaderFieldId = item[0].ToString();
                    headerdata.TableName = table;
                    headerdata.AddonHeaderField = item[1].ToString();
                    headerdata.DataType = item[2].ToString();
                    headerdata.Length = item[3].ToString();
                    headerdata.IsRequired = item[4].ToString() == "NULL" ? false : true;
                    headerdata.CreateDate = DateTime.Now;
                    headerdata.CreateUserID = id;
                    //headerdata.DefaultValue = item[5].ToString() == "NULL" ? "" : item[5].ToString();
                    _context.SaveChanges();
                    SaveChanges();
                }
                else
                {
                    Header header = new Header();
                    header.MapId = check == 1 ? mapid : newid;
                    //header.SAPHeaderFieldId = item[0].ToString();
                    header.TableName = table;
                    header.AddonHeaderField = item[1].ToString();
                    header.DataType = item[2].ToString();
                    header.Length = item[3].ToString();
                    header.IsRequired = item[4].ToString() == "NULL" ? false : true;
                    header.CreateDate = DateTime.Now;
                    header.CreateUserID = id;
                    //header.DefaultValue = item[5].ToString() == "NULL" ? "" : item[5].ToString();
                    _context.Headers.Add(header);
                    _context.SaveChanges();
                    SaveChanges();
                }
            }
        }
        public void Create_Rows(List<string[]> rows, int id, string table, int check, int newid, int mapid)
        {
            foreach (var item in rows)
            {
                var _id = (check == 1 ? mapid : newid);
                var fieldid = item[0].ToString();
                var dtexist = _context.Rows.Where(x => x.MapId == _id).Any();
                if (dtexist)
                {
                    var rowdata = _context.Rows.Where(x => x.MapId == _id ).FirstOrDefault();
                    rowdata.MapId = check == 1 ? mapid : newid;
                    rowdata.TableName = table;
                    rowdata.AddonRowField = item[1].ToString();
                    rowdata.DataType = item[2].ToString();
                    rowdata.Length = item[3].ToString();
                    rowdata.IsRequired = item[4].ToString() == "NULL" ? false : true;
                    rowdata.CreateDate = DateTime.Now;
                    rowdata.CreateUserID = id;
                    //rowdata.DefaultValue = item[5].ToString() == "NULL" ? "" : item[5].ToString();
                    _context.SaveChanges();
                    SaveChanges();
                }
                else
                {
                    Row row = new Row();
                    row.MapId = check == 1 ? mapid : newid;
                    row.TableName = table;
                    row.AddonRowField = item[1].ToString();
                    row.DataType = item[2].ToString();
                    row.Length = item[3].ToString();
                    row.IsRequired = item[4].ToString() == "NULL" ? false : true;
                    row.CreateDate = DateTime.Now;
                    row.CreateUserID = id;
                    //row.DefaultValue = item[5].ToString() == "NULL" ? "" : item[5].ToString();
                    _context.Rows.Add(row);
                    _context.SaveChanges();
                    SaveChanges();
                }
            }
        }

        public List<string[]> NewFieldValue(List<string[]> list, int count)
        {
            List<string[]> _list = new List<string[]>();
            _list = list;
            for (int i = 0; i < count; i++)
            {
                _list.RemoveAt(0);
            }
            return _list;
        }
        public string Get_Constring(SetupCreateViewModel.AddonViewModel AddonPath)
        {
            return QueryAccess.con(AddonPath.AddonDBName,
                                            AddonPath.AddonIPAddress,
                                            AddonPath.AddonPort,
                                            AddonPath.AddonServerName,
                                            AddonPath.AddonDBuser,
                                            AddonPath.AddonDBPassword);
        }

        public void GenerateTable(string database, string constring, List<string[]> headerfield, List<string[]> rowfield,
                                  string rowname, string headername, int headercount, int rowcount)
        {

            QueryAccess.GenerateTable(database, constring, headerfield, rowfield,
                                      rowname, headername, headercount, rowcount);

        }
        public MapCreateViewModel Populate(string table, string code, string header, string row)
        {
            string[] tables = new string[2];
            var model = new MapCreateViewModel();

            tables[0] = header;
            tables[1] = row;
            var SAPSetup = Select_SAPSetup(code);
            string Hanaconn = SAPSetup.SAPDBVersion.Contains("HANA") ? QueryAccess.HANA_conString(SAPSetup.SAPIPAddress,
                                                                                               SAPSetup.SAPDBuser,
                                                                                               SAPSetup.SAPDBPassword,
                                                                                               SAPSetup.SAPDBName)
                                                                                               :
                                                                    QueryAccess.MSSQL_conString(SAPSetup.SAPIPAddress,
                                                                                               SAPSetup.SAPDBuser,
                                                                                               SAPSetup.SAPDBPassword,
                                                                                               SAPSetup.SAPDBName);
            if (QueryAccess.CheckCon(Hanaconn, SAPSetup.SAPDBVersion) is true)
            {
                model.HeaderHanaFields = QueryAccess.connHana(Hanaconn, tables.ToList(), SAPSetup.SAPDBName, SAPSetup.SAPDBVersion)
                                                    .Item1.Select(x => new MapCreateViewModel.HeaderHanaField
                                                    {
                                                        ColumnName = x.ToString(),
                                                    }).ToList();

                model.HeaderHanaFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = "" });
                model.RowHanaFields = QueryAccess.connHana(Hanaconn, tables.ToList(), SAPSetup.SAPDBName, SAPSetup.SAPDBVersion)
                                                 .Item2.Select(x => new MapCreateViewModel.RowHanaField
                                                 {
                                                     ColumnName = x.ToString(),
                                                 }).ToList();
                model.RowHanaFields.Add(new MapCreateViewModel.RowHanaField { ColumnName = "" });
                model.DataTypes = Enum.GetValues(typeof(SqlDbType)).Cast<SqlDbType>().Select(x => new MapCreateViewModel.Data { DataType = x.ToString().ToUpper() }).ToList();

            }
            return model;
        }

        public MapCreateViewModel PopulateAPI(string table, string code, string header, string row, string APICode)
        {
            string[] tables = new string[2];
            var model = new MapCreateViewModel();
            model.HeaderHanaFields = new List<MapCreateViewModel.HeaderHanaField>();
            model.RowHanaFields = new List<MapCreateViewModel.RowHanaField>();
            model.HeaderAPIFields = new List<MapCreateViewModel.HeaderHanaField>();
            model.RowAPIFields = new List<MapCreateViewModel.RowHanaField>();

            #region DefaultValue
            model.HeaderAPIFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = "--Default Value--" }); ////ADD BLANK AS OPTION            
            model.RowAPIFields.Add(new MapCreateViewModel.RowHanaField { ColumnName = "--Default Value--" }); ////ADD BLANK AS OPTION
            #endregion

            tables[0] = header; //unused
            tables[1] = row; //unused
            var SAPSetup = Select_SAPSetup(code);

            string module = "";
            if (table.ToLower().Contains("item"))
            { module = "Item"; }
            else if (table.ToLower().Contains("incoming payments"))
            { module = "IncomingPayments"; }
            else
            { module = "Document"; }

            string Hanaconn = SAPSetup.SAPDBVersion.Contains("HANA") ? QueryAccess.HANA_conString(SAPSetup.SAPIPAddress,
                                                                                               SAPSetup.SAPDBuser,
                                                                                               SAPSetup.SAPDBPassword,
                                                                                               SAPSetup.SAPDBName)
                                                                                               :
                                                                    QueryAccess.MSSQL_conString(SAPSetup.SAPIPAddress,
                                                                                                SAPSetup.SAPDBuser,
                                                                                                SAPSetup.SAPDBPassword,
                                                                                                SAPSetup.SAPDBName);
            #region GETSAP_Fields_Types 
            if (QueryAccess.CheckCon(Hanaconn, SAPSetup.SAPDBVersion) is true)
            {
                string docid = SAPSetup.SAPDBVersion.Contains("HANA") ? DataAccess.SelectHana(QueryAccess.HANA_conString(
                                                                                                    SAPSetup.SAPIPAddress,
                                                                                                    SAPSetup.SAPDBuser,
                                                                                                    SAPSetup.SAPDBPassword,
                                                                                                    SAPSetup.SAPDBName), QueryAccess.GetTopOrder()).AsEnumerable().Select(x => x["DocEntry"].ToString()).FirstOrDefault()
                                                                                                    :
                                                                                                    DataAccess.Select(QueryAccess.MSSQL_conString(
                                                                                                    SAPSetup.SAPIPAddress,
                                                                                                    SAPSetup.SAPDBuser,
                                                                                                    SAPSetup.SAPDBPassword,
                                                                                                    SAPSetup.SAPDBName), QueryAccess.GetTopOrder()).AsEnumerable().Select(x => x["DocEntry"].ToString()).FirstOrDefault();

                //Get the fields from JSON Service Layer
                var auth = new AuthenticationCredViewModel
                {
                    Method = "GET",
                    //Action = //$@"Orders({docid})", //For JSON_DATA VERSION 1
                    Action = "$metadata?retrieveFields=1", //For META_DATA  VERSION 2
                    JsonString = "{}",
                    SAPServer = SAPSetup.SAPIPAddress,
                    Port = SAPSetup.SAPLicensePort.ToString(),
                    SAPDatabase = SAPSetup.SAPDBName,
                    SAPDBUserId = SAPSetup.SAPDBuser,
                    SAPDBPassword = SAPSetup.SAPDBPassword,
                    SAPUserID = SAPSetup.SAPUser,
                    SAPPassword = SAPSetup.SAPPassword
                };
                sapAces.SaveCredentials(auth);
                bool blnLogin = SAPAccess.LoginAction();

                #region SAP_METADATA_XML
                if (blnLogin == true)
                {
                    string sxml = sapAces.SendSLData(auth);
                    XmlDocument xmlDoc = new XmlDocument();
                    if (!string.IsNullOrEmpty(sxml))
                    {
                        xmlDoc.LoadXml(sxml);
                        foreach (XmlElement element in xmlDoc.DocumentElement)
                        {
                            foreach (XmlNode nodeSchem in element.ChildNodes)
                            {
                                foreach (XmlNode nodeEntity in nodeSchem.ChildNodes)
                                {
                                    if (nodeEntity.Name.Equals("EntityType") && nodeEntity.Attributes[0].Value.Equals(module))
                                    {
                                        foreach (XmlNode nodeProp in nodeEntity.ChildNodes)
                                        {
                                            if (nodeProp.Name.Equals("Property") && !nodeProp.Attributes[1].Value.ToLower().Contains("collection"))
                                            {
                                                model.HeaderHanaFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = nodeProp.Attributes[0].Value });
                                            }
                                        }
                                    }
                                    else if (nodeEntity.Name.Equals("ComplexType") && nodeEntity.Attributes[0].Value.Equals($"{module}Line"))
                                    {
                                        foreach (XmlNode nodeProp in nodeEntity.ChildNodes)
                                        {
                                            if (nodeProp.Name.Equals("Property") && !nodeProp.Attributes[1].Value.ToLower().Contains("collection"))
                                            {
                                                model.RowHanaFields.Add(new MapCreateViewModel.RowHanaField { ColumnName = nodeProp.Attributes[0].Value });
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }
                    //model.HeaderHanaFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = "" }); ////ADD BLANK AS OPTION
                    //model.RowHanaFields.Add(new MapCreateViewModel.RowHanaField { ColumnName = "" }); ////ADD BLANK AS OPTION
                }
                #endregion

                #region SAP_JSON_DATA
                //if (blnLogin == true)
                //{
                //    string js = sapAces.SendSLData(auth);
                //    if (!js.Substring(0,5).ToLower().Contains("error"))
                //    {
                //        JObject jobj = JObject.Parse(js);

                //        #region HEADER_SAPFIELD
                //        var jlist = jobj.Properties().AsEnumerable().Where(x=> x.Name != "odata.metadata").Select(x => x).ToList();
                //        jlist.ForEach(x =>
                //        {
                //            JToken propertyValue = x.Value;
                //            if (!(propertyValue.Type == JTokenType.Array || propertyValue.Type == JTokenType.Object))
                //            {
                //                model.HeaderHanaFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = x.Name });
                //            }
                //        });
                //        #endregion

                //        #region ROW_SAPFIELD
                //        ////ADD FIRST THE DocumentLines BEFORE REMOVING THE PROPERTY
                //        var jlines = jobj["DocumentLines"];
                //        var JChild = jlines.Children<JObject>();
                //        var jlistrow = JChild.Properties().AsEnumerable().Select(x => x).ToList();
                //        jlistrow.ForEach(x => 
                //        {
                //            JToken propertyValue = x.Value;
                //            if (!(propertyValue.Type == JTokenType.Array || propertyValue.Type == JTokenType.Object))
                //            {
                //                model.RowHanaFields.Add(new MapCreateViewModel.RowHanaField { ColumnName = x.Name });
                //            }

                //        });
                //        #endregion
                //    }
                //}
                #endregion

                #region SAP_DATATYPE
                model.DataTypes = Enum.GetValues(typeof(SqlDbType)).Cast<SqlDbType>().Select(x => new MapCreateViewModel.Data { DataType = x.ToString().ToUpper() }).ToList();
                #endregion
            }
            #endregion

            #region GETAPI_Field
            //string ret = JsonSboSample();
            var apimethod = _context.APISetups.Where(x => x.APICode == APICode).Select(x => x.APIMethod).FirstOrDefault(); apimethod = (apimethod == null ? "" : apimethod);
            var apiurl = _context.APISetups.Where(x => x.APICode == APICode).Select(x => x.APIURL).FirstOrDefault(); apiurl = (apiurl == null ? "" : apiurl);
            //var apiuser = _context.APISetups.Where(x => x.APICode == APICode).Select(x => x.APIKey).FirstOrDefault(); apiuser = (apiuser == null ? "" : apiuser);
            //var apipwd = _context.APISetups.Where(x => x.APICode == APICode).Select(x => x.APISecretKey).FirstOrDefault(); apipwd = (apipwd == null ? "" : apipwd); 
            //var apiloginurl = _context.APISetups.Where(x => x.APICode == APICode).Select(x => x.APILoginUrl).FirstOrDefault(); apiloginurl = (apiloginurl == null ? "" : apiloginurl);
            //var apiloginbody = _context.APISetups.Where(x => x.APICode == APICode).Select(x => x.APILoginBody).FirstOrDefault(); apiloginbody = (apiloginbody == null ? "" : apiloginbody);
            var apiuser = "";
            var apipwd = "";
            var apiloginurl = "";
            var apiloginbody = "";
            string ret = (!apiloginurl.ToLower().Contains("b1s/v1")) ? sapAces.APIResponse(apimethod, apiurl, "", "", "", apiuser, apipwd, 1)
                                    : (sapAces.XmlPostJson("POST", apiloginurl, $"{apiloginbody}")).ToLower().Contains("error") ? "" : sapAces.XmlPostJson(apimethod, apiurl, "");
            if (!string.IsNullOrEmpty(ret)) 
            { 
                if (ret.ToLower().Substring(0, 5).Contains("xml"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    if (!string.IsNullOrEmpty(ret))
                    {
                        xmlDoc.LoadXml(ret);
                        foreach (XmlElement element in xmlDoc.DocumentElement)
                        {
                            foreach (XmlNode nodeSchem in element.ChildNodes)
                            {
                                foreach (XmlNode nodeEntity in nodeSchem.ChildNodes)
                                {
                                    if (nodeEntity.Name.Equals("EntityType") && nodeEntity.Attributes[0].Value.Equals(module))
                                    {
                                        foreach (XmlNode nodeProp in nodeEntity.ChildNodes)
                                        {
                                            if (nodeProp.Name.Equals("Property") && !nodeProp.Attributes[1].Value.ToLower().Contains("collection"))
                                            {
                                                model.HeaderAPIFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = nodeProp.Attributes[0].Value });
                                            }
                                        }
                                    }
                                    else if (nodeEntity.Name.Equals("ComplexType") && nodeEntity.Attributes[0].Value.Equals($"{module}Line"))
                                    {
                                        foreach (XmlNode nodeProp in nodeEntity.ChildNodes)
                                        {
                                            if (nodeProp.Name.Equals("Property") && !nodeProp.Attributes[1].Value.ToLower().Contains("collection"))
                                            {
                                                model.RowAPIFields.Add(new MapCreateViewModel.RowHanaField { ColumnName = nodeProp.Attributes[0].Value });
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }
                }
                else
                {
                    JObject json = JObject.Parse(ret);
                    if (module.ToLower().Contains("document"))
                    {
                        //ForShopifyApi
                        if (json["orders"] != null)
                        {
                            var jheader = json["orders"];
                            var JChild = jheader.Children<JObject>();
                            var hlist = JChild.Properties().GroupBy(x => x.Name).Select(x => x.First()).ToList();
                            hlist.ForEach(x=> {
                                JToken propertyValue = x.Value;
                                if (!(propertyValue.Type == JTokenType.Array || propertyValue.Type == JTokenType.Object))
                                {
                                    model.HeaderAPIFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = x.Name });
                                }
                            });
                            if (hlist.Where(x=> x.Name.Contains("line_items")).Any())
                            {
                                var jlines = json["orders"][0]["line_items"];
                                if (jlines != null)
                                {
                                    var jlinechild = jlines.Children<JObject>();
                                    var lis = jlinechild.Properties().GroupBy(x => x.Name).Select(x => x.First()).ToList();
                                    lis.ForEach(x =>
                                    {
                                        JToken propertyValue = x.Value;
                                        if (!(propertyValue.Type == JTokenType.Array || propertyValue.Type == JTokenType.Object))
                                        {
                                            model.RowAPIFields.Add(new MapCreateViewModel.RowHanaField { ColumnName = x.Name });
                                        }
                                    });
                                }
                            }   

                        }
                        //Default
                        else
                        {

                        }

                    }
                    else if (module.ToLower().Contains("item"))
                    {
                        //ForShopifyApi
                        var jheader = json["products"];
                        var JChild = jheader.Children<JObject>();
                        var hlist = JChild.Properties().GroupBy(x => x.Name).Select(x => x.First()).ToList();
                        hlist.ForEach(x => {
                            JToken propertyValue = x.Value;
                            if (!(propertyValue.Type == JTokenType.Array || propertyValue.Type == JTokenType.Object))
                            {
                                model.HeaderAPIFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = x.Name });
                            }
                        });

                        var varchild = json["products"][0]["variants"];
                        var lchild = varchild.Children<JObject>();
                        var lis = lchild.Properties().GroupBy(x => x.Name).Select(x => x.First()).ToList();
                        lis.ForEach(x =>
                        {
                            JToken propertyValue = x.Value;
                            if (!(propertyValue.Type == JTokenType.Array || propertyValue.Type == JTokenType.Object))
                            {
                                model.HeaderAPIFields.Add(new MapCreateViewModel.HeaderHanaField { ColumnName = x.Name });
                            }
                        });
                    }

                    #endregion
                }

            }
            return model;
        }

        public MapCreateViewModel Get_DataType(string table, string code, string field)
        {

            string[] tables = new string[2];
            var model = new MapCreateViewModel();
            if (table == "Sales Order") { tables[0] = "ORDR"; tables[1] = "RDR1"; }
            var SAPSetup = Select_SAPSetup(code);

            string Hanaconn = SAPSetup.SAPDBVersion.Contains("HANA") ? QueryAccess.HANA_conString(SAPSetup.SAPIPAddress,
                                                                                               SAPSetup.SAPDBuser,
                                                                                               SAPSetup.SAPDBPassword,
                                                                                               SAPSetup.SAPDBName)
                                                                                               :
                                                                    QueryAccess.MSSQL_conString(SAPSetup.SAPIPAddress,
                                                                                                SAPSetup.SAPDBuser,
                                                                                                SAPSetup.SAPDBPassword,
                                                                                                SAPSetup.SAPDBName);

            string Db = SAPSetup.SAPDBVersion.Contains("HANA") ? "HANA" : "MSSQL";

            model.HeaderDataTypes = QueryAccess.getDataType(Hanaconn, tables.ToList(), SAPSetup.SAPDBName, SAPSetup.SAPDBVersion, field)
                                               .Item1.AsEnumerable().Select(x => new MapCreateViewModel.HeadData
                                               {
                                                   DataType = x[0].ToString(),
                                                   Length = x[1].ToString(),
                                                   Scale = x[2].ToString(),
                                                   DBType = Db,
                                               }).ToList();

            model.RowDataTypes = QueryAccess.getDataType(Hanaconn, tables.ToList(), SAPSetup.SAPDBName, SAPSetup.SAPDBVersion, field)
                                            .Item2.AsEnumerable().Select(x => new MapCreateViewModel.RowData
                                            {
                                                DataType = x[0].ToString(),
                                                Length = x[1].ToString(),
                                                Scale = x[2].ToString(),
                                                DBType = Db,
                                            }).ToList();

            return model;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public string JsonSboSample()
        {
            string ret = "";
            AuthenticationCredViewModel auth = new AuthenticationCredViewModel();
            auth.URL = "http";
            auth.Port = "50001";
            auth.Action = "Orders(631)";
            auth.Method = "GET";
            auth.SAPServer = "192.168.2.15";
            auth.SAPDatabase = "DIRECDEALS_TEST";
            auth.SAPUserID = "Direc2";
            auth.SAPPassword = "1234";
            auth.SAPDBUserId = "SYSTEM";
            auth.SAPDBPassword = "Sb1@dbti";
            auth.JsonData = JObject.Parse(@"{}");
            auth.JsonString = "{}";
            auth.Id = 0;
            sapAces.SaveCredentials(auth);
            if (SAPAccess.LoginAction() == true)
            {
                 ret = sapAces.SendSLData(auth);
            }
            return ret;
        }
    }
}