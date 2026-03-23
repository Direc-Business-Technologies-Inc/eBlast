using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DomainLayer.ViewModels
{
    public class UploadViewModel
    {

        public List<Upload> UploadList { get; set; }
        public class Upload
        {
            public int TransactionId { get; set; }
            public int TerminalNo { get; set; }

            public string BranchCode { get; set; }

            public string BranchName { get; set; }

            public string TranDate { get; set; }

            public string CashierName { get; set; }

            public string Status { get; set; }

        }

        //public List<Upload> UploadList { get; set; }
        //public class Upload
        //{
        //    public int TransactionId { get; set; }
        //    public int TerminalNo { get; set; }

        //    public string BranchCode { get; set; }

        //    public string BranchName { get; set; }

        //    public string TranDate { get; set; }

        //    public string CashierCode { get; set; }

        //    public string CashierName { get; set; }

        //    public string Status { get; set; }

        //}

        public List<Header> Headers { get; set; }
        public class Header
        {
            public int HeaderId { get; set; }
            public int MapId { get; set; }
            public string TableName { get; set; }
            public string SAPHeaderField { get; set; }
            public string AddonHeaderField { get; set; }
            public string DataType { get; set; }
            public string Length { get; set; }
            public bool IsRequired { get; set; }
        }

        public List<Row> Rows { get; set; }
        public class Row
        {
            public int RowId { get; set; }
            public int MapId { get; set; }
            public string TableName { get; set; }
            public string SAPRowField { get; set; }
            public string AddonRowField { get; set; }
            public string DataType { get; set; }
            public string Length { get; set; }
            public bool IsRequired { get; set; }
        }
        public List<SAPConfig> SAPList { get; set; }
        public class SAPConfig
        {
            public string Code { get; set; }
        }
        public List<Map> MapList { get; set; }
        public class Map
        {
            public int Id { get; set; }

            public string Code { get; set; }

            public string Name { get; set; }
        }
    }
}