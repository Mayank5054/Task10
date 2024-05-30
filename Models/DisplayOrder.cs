using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task10.Models
{
    public class DisplayOrder
    {
        public int OrderId { get; set; }
        public List<string> ProductName { get; set; }
        public int Amount { get; set; }
        public DateTime orderDate { get; set; }

        public string singleProduct { get;set; }
    }
}