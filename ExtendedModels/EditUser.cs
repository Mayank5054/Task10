using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task10.ExtendedModels
{
    public class EditUser
    {

        public int UserId { get; set; }
        public string ContactNo { get; set; }
        public string Fullname { get; set; }
        public string Type { get; set; }

    }
}