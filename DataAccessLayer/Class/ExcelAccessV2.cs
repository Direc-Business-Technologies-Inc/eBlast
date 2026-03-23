using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace DataAccessLayer.Class
{
    public class ExcelAccessV2
    {
        public static Tuple<DataTable, DataTable> Getdata(string path, string BackupPath, string constr, string headtbname, string dbname, List<DataRow> HeaderField,
                                                          List<DataRow> RowField, string HeaderWorksheet, string RowWOrksheet, string FileType, string FileName)
        {

            string[] extensions = FileType.Split(',');

            int MaxTransId = QueryAccess.getTransId(constr, headtbname, dbname);
            DataTable tableheader = new DataTable();
            DataTable tablerow = new DataTable();

            //create directory if not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                if (!Directory.Exists(BackupPath))
                {
                    Directory.CreateDirectory(BackupPath);
                }
            }

            var Concat = FileName.Split(new string[] { ",", ".", "!", "?", ";", ":", " ", "-", "+", "&", "*", "#", "$", "%" }, StringSplitOptions.RemoveEmptyEntries);
            var c = Concat.Count();
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] fi = FileName == "" || FileName == null ? directory.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray()
                                                              : directory.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())
                                                              && f.Name.ToLower().Contains(Concat[0].ToLower())
                                                              && f.Name.ToLower().Contains(Concat.Count() >= 1 ? Concat[1].ToLower() : Concat[0].ToLower())).ToArray();

            if (fi.Any() && fi.Length != 0)
            {
                foreach (var filename in fi)
                {
                    string FilePath = $@"{path}\{filename.ToString()}";
                    FileStream stream = File.Open($@"{path}\{filename.ToString()}", FileMode.Open, FileAccess.Read);

                    IExcelDataReader excelReader;
                    //1. Reading Excel file
                    if (Path.GetExtension($@"{path}\{filename.ToString()}").ToUpper() == ".XLS")
                    {
                        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (Path.GetExtension($@"{path}\{filename.ToString()}").ToUpper() == ".XLSX")
                    {
                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        excelReader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                        {
                            FallbackEncoding = Encoding.GetEncoding(1252),
                            AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },
                        });

                    }
                    DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    #region Adding Columns

                    if (!tableheader.Columns.Contains("TransactionId"))
                    {
                        tableheader.Columns.Add("TransactionId", typeof(System.String));
                        foreach (DataColumn col in result.Tables[0].Columns)
                        {
                            foreach (var item in HeaderField)
                            {
                                if (item.ItemArray[0].ToString() == col.ToString())
                                {
                                    tableheader.Columns.Add(col.ToString());
                                    break;
                                }
                            }
                        }
                        //extra columns
                        tableheader.Columns.Add("UploadDate", typeof(System.DateTime));
                        tableheader.Columns.Add("Status", typeof(System.Char));
                        tableheader.Columns.Add("Message", typeof(System.String));
                    }

                    if (!tablerow.Columns.Contains("TransactionId"))
                    {
                        tablerow.Columns.Add("TransactionId", typeof(System.Int32));

                        foreach (DataColumn col in result.Tables[0].Columns)
                        {
                            foreach (var item in RowField)
                            {
                                if (item.ItemArray[0].ToString() == col.ToString())
                                {
                                    tablerow.Columns.Add(col.ToString());
                                    break;
                                }
                            }
                        }
                        tablerow.Columns.Add("Status", typeof(System.Char));
                        tablerow.Columns.Add("Message", typeof(System.String));


                    }

                    #endregion

                    #region Populating Datatables

                    foreach (DataRow data in result.Tables[0].Rows)
                    {

                        object[] items = new object[tableheader.Columns.Count - 3];
                        var row = tableheader.NewRow();
                        if (!(tableheader.AsEnumerable().Where(x => (x.Field<string>(result.Tables[0].Columns.Cast<DataColumn>().Where(y => y.ColumnName.ToLower().Contains("terminal")).FirstOrDefault().ColumnName)
                            == data[result.Tables[0].Columns.Cast<DataColumn>().Where(y => y.ColumnName.ToLower().Contains("terminal")).FirstOrDefault().ColumnName].ToString())
                            
                            && (x.Field<string>(result.Tables[0].Columns.Cast<DataColumn>().Where(y => y.ColumnName.ToLower().Contains("date")).FirstOrDefault().ColumnName)
                            == data[result.Tables[0].Columns.Cast<DataColumn>().Where(y => y.ColumnName.ToLower().Contains("date")).FirstOrDefault().ColumnName].ToString())).Any()))


                        {


                            for (int i = 0; i <= data.ItemArray.Length - 1; i++)
                            {
                                if (tableheader.Columns.Contains(data.Table.Columns[i].ToString()))
                                {
                                    for (int count = 1; count < items.Length; count++)
                                    {
                                        if (items[count] is null)
                                        {
                                            items[count] = data[i];
                                            break;
                                        }
                                    }
                                    //items[i] = data[i].ToString();
                                }
                            }

                            row.ItemArray = (Object[])items;
                            row["TransactionId"] = MaxTransId;

                            row["Status"] = "O";
                            tableheader.Rows.Add(row);

                            foreach (DataRow datarow in result.Tables[0].Rows)
                            {
                                object[] rowitems = new object[tablerow.Columns.Count - 2];
                                var rows = tablerow.NewRow();
                                if ((data[result.Tables[0].Columns.Cast<DataColumn>().Where(x => x.ColumnName.ToLower().Contains("terminal")).FirstOrDefault().ColumnName].ToString()
                                    == datarow[result.Tables[0].Columns.Cast<DataColumn>().Where(x => x.ColumnName.ToLower().Contains("terminal")).FirstOrDefault().ColumnName].ToString())
                                    &&
                                    (data[result.Tables[0].Columns.Cast<DataColumn>().Where(x => x.ColumnName.ToLower().Contains("date")).FirstOrDefault().ColumnName].ToString()
                                    == datarow[result.Tables[0].Columns.Cast<DataColumn>().Where(x => x.ColumnName.ToLower().Contains("date")).FirstOrDefault().ColumnName].ToString()))
                                {

                                    for (int i = 0; i < datarow.ItemArray.Length - 1; i++)
                                    {
                                        var filed = datarow.Table.Columns[i].ToString();
                                        if (tablerow.Columns.Contains(datarow.Table.Columns[i].ToString()))
                                        {
                                            for (int count = 1; count < rowitems.Length; count++)
                                            {
                                                if (rowitems[count] is null)
                                                {
                                                    if (datarow.Table.Columns[i].ToString().ToLower().Contains("barcode"))
                                                    {
                                                        datarow[i] = Decimal.Parse(datarow[i].ToString(), System.Globalization.NumberStyles.Float);
                                                    }
                                                    rowitems[count] = datarow[i];
                                                    break;
                                                }
                                            }
                                            //rowitems[i] = datarow[i].ToString();

                                        }

                                    }

                                    rows.ItemArray = (Object[])rowitems;
                                    rows["TransactionId"] = MaxTransId;
                                    rows["Status"] = "O";
                                    tablerow.Rows.Add(rows);
                                }

                            }
                            MaxTransId++;
                        }
                    }

                    #endregion

                    stream.Close();
                    File.Move($@"{path}\{filename.ToString()}", $@"{BackupPath}\{filename.ToString()}");
                }
            }

            return Tuple.Create(tableheader, tablerow);
        }

        public static DataSet GetFileData(Stream Data, string FileName)
        {
            IExcelDataReader excelReader;
            if (FileName.ToUpper().Contains(".XLS"))
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(Data);
            }
            else if (FileName.ToUpper().Contains(".XLSX"))
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(Data);
            }
            else
            {
                excelReader = ExcelReaderFactory.CreateCsvReader(Data, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.GetEncoding(1252),
                    AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },
                });

            }
            DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            return result;
        }
    }
}