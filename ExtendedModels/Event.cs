using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task10.Models
{
    [MetadataType(typeof(CustomEvent))]
    partial class Event
    {
        internal class CustomEvent
        {
            [Display(Name = "Quantity of dishes")]
            public Nullable<int> QtyOfDishes { get; set; }

            [Display(Name = "Event Date")]
            [Required(ErrorMessage = "Event Date is required.")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> EventDate { get; set; }

            [Display(Name = "Duration")]
            [Required(ErrorMessage = "Duration is required.")]
            public string Duration { get; set; }
        }
    }
}