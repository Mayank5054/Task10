using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Models;

namespace Task10.Models
{
    public class stockViewModel
    {
        public Stock stock { get; set; }

        public IList<SelectListItem> ItemNames { get; set; }
    }
}