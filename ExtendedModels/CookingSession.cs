using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Task10.Models
{
    [MetadataType(typeof(MetadataCookingSession))]
    public partial class CookingSession
    {
        internal class MetadataCookingSession
        {
            [Required(ErrorMessage = "Please Select Any Trainer")]
            public Nullable<int> TranierId { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessage = "Date is required")]
            public Nullable<System.DateTime> SessionDate { get; set; }

            //[Required(ErrorMessage = "Please Select Attendee")]
            //public List<string> AttendeeId { get; set; }
        }
        [NotMapped]

        public List<SelectListItem> TrainerList { get; set; }

        [NotMapped]

        public List<SelectListItem> AttendeeList { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please Select Attendee")]

        public List<string> AttendeeId { get; set; }
    }
}