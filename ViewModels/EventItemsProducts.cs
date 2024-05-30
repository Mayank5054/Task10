using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Task10.Models;


namespace Task10.ViewModels
{
    public class EventItemsProducts
    {
        public List<EventItem> eventItems { get; set; }
        public List<Product> products { get; set; }
        public int EventId { get; set; }
    }
}