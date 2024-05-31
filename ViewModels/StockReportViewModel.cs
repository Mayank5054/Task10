using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task10.ViewModels
{
    public class StockReportViewModel
    {
        public string ProductName { get; set; }
        public int TotalStock { get; set; } // Changed to decimal
        public int RemStock { get; set; }   // Changed to decimal
        public string IsLowKey { get; set; }
    }
}
