using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer
{
    public class Row
    {
        [Key]
        public int RowId { get; set; }

        public int MapId { get; set; }

        public string TableName { get; set; }

        public string SAPRowField { get; set; }

        public string AddonRowField { get; set; }

        public string DataType { get; set; }

        public string Length { get; set; }

        public bool IsRequired { get; set; }

        public DateTime? CreateDate { get; set; }

        public int CreateUserID { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateUserID { get; set; }
    }
}