using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.ViewModels
{
    public class PostingViewModel
    {
        public string HanaConnection { get; set; }
        public string AddonConnection { get; set; }
        public Credential CredentialDetails { get; set; }
        public class Credential
        {
            public int MapId { get; set; }
            public string AddonCode { get; set; }
            public string AddonDBName { get; set; }
            public string AddonIPAddress { get; set; }
            public int? AddonPort { get; set; }
            public string AddonServerName { get; set; }
            public string AddonDBuser { get; set; }
            public string AddonDBPassword { get; set; }
            public string LocalPath { get; set; }
            public string BackupPath { get; set; }
            public string HeaderName { get; set; }
            public string RowName { get; set; }
            public string HeaderWorksheet { get; set; }
            public string RowWorksheet { get; set; }
            public string FileType { get; set; }
            public string FileName { get; set; }
            public string SAPServerName { get; set; }
            public string SAPIPAddress { get; set; }
            public string SAPDBName { get; set; }
            public int SAPLicensePort { get; set; }
            public int SAPDBPort { get; set; }
            public string SAPDBuser { get; set; }
            public string SAPDBPassword { get; set; }
            public string SAPUser { get; set; }
            public string SAPPassword { get; set; }
            public bool PostSAP { get; set; }
            public string FTPPath { get; set; }
            public string FTPUser { get; set; }
            public string FTPPass { get; set; }
            public string Module { get; set; }
        }

        public SyncCreds SyncCredsDetails { get; set; }
        public class SyncCreds
        {
            public string Path { get; set; }
            public string FileType { get; set; }
            public string IpAddress { get; set; }
            public string DbName { get; set; }
            public string DbVersion { get; set; }
            public string DbUser { get; set; }
            public string DbPass { get; set; }
            public string QueryString { get; set; }
            public string Code { get; set; }
            public string FtpPath { get; set; }
            public string FtpUser { get; set; }
            public string FtpPass { get; set; }
        }

        public List<Fields> HeaderFields { get; set; }
        public List<Fields> RowFields { get; set; }
        public class Fields
        {
            public string SAPFieldId { get; set; }
            public string AddonField { get; set; }
        }

        public List<APIViewModel> APIView { get; set; }

        public class APIViewModel
        {
            public int APIId { get; set; }
            public string APICode { get; set; }

            public string APIMethod { get; set; }
            public string APIURL { get; set; }
            public bool IsActive { get; set; }
            public string APIKey { get; set; }
            public string APISecretKey { get; set; }
            public string APIToken { get; set; }
            public string APILoginUrl { get; set; }
            public string APILoginBody { get; set; }
        }
        public List<APIData> APIdatas { get; set; }
        public class APIData
        {
            public long id { get; set; }
            public long checkout_id { get; set; }
            public string created_at { get; set; }
            public string currency { get; set; }
            public string current_subtotal_price { get; set; }
            public string current_total_discounts { get; set; }
            public string current_total_price { get; set; }
            public string current_total_tax { get; set; }
            public string gateway { get; set; }
            public int number { get; set; }
            public int order_number { get; set; }
            public string processed_at { get; set; }
            public string processing_method { get; set; }
            public string subtotal_price { get; set; }
            public class customer
            {
                public long id { get; set; }
                public string first_name { get; set; }
                public string last_name { get; set; }
            }
            public List<linesitem> line_items { get; set; }
        }        
        public class linesitem
        {
            public string price { get; set; }
            public string product_id { get; set; }
            public int quantity { get; set; }
            public string sku { get; set; }
            public string title { get; set; }
            public string total_discount { get; set; }
            public long variant_id { get; set; }
            public string variant_title { get; set; }

        }
    }
}
