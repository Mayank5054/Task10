using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task10.Models
{
    [MetadataType(typeof(MetadataDish))]
    public partial class Dish
    {
        internal class MetadataDish
        {
            [Required]
            public Nullable<int> ItemID { get; set; }
            [Required]
            public string Type { get; set; }
            [Required]
            public Nullable<System.DateTime> Date { get; set; }
            [Required]
            public Nullable<int> DishCount { get; set; }

            public Nullable<System.DateTime> CreatedOn { get; set; }
            public Nullable<System.DateTime> UpdatedOn { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<int> Isdeleted { get; set; }
        }
    }
}