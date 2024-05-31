using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task10.Models
{


    [MetadataType(typeof(MetadataProduct))]
    public partial class Product
    {

        internal class MetadataProduct
        {
            [Required(ErrorMessage = "Select Weight Type")]
            public string WeightType { get; set; }

            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Select Product Type")]
            public string Type { get; set; }

            [Required(ErrorMessage = "Price is required")]
            public Nullable<decimal> Price { get; set; }
            [Required(ErrorMessage = "Visible is required")]
            public Nullable<bool> Visible { get; set; }


            public Nullable<decimal> Threshold { get; set; }
            public Nullable<System.DateTime> CreatedOn { get; set; }
            public Nullable<System.DateTime> UpdatedOn { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<int> Isdeleted { get; set; }

        }
    }
}