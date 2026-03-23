using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.ViewModels
{
    public class DashboardViewModel
    {
        public List<Item> ItemList { get; set; }
        public List<Item> ItemComparisonList { get; set; }
        public class Item
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string Uom { get; set; }
            public string Price { get; set; }
            public string Stock { get; set; }
            public string LastUpdate { get; set; }
            public string SalesQty { get; set; }
            public string CurrentStock { get; set; }
            public string SalesPrice { get; set; }
            
        }

        public List<SAPConfig> SAPList { get; set; }
        public class SAPConfig
        { 
        public string Code { get; set; }
        }

    }
}
