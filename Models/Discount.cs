//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Task10.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Discount
    {
        public int DiscountId { get; set; }
        public Nullable<System.DateTime> From { get; set; }
        public Nullable<System.DateTime> To { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> Value { get; set; }
        public Nullable<int> NumberOfTimes { get; set; }
        public Nullable<decimal> MaxDiscount { get; set; }
        public string CouponName { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> IsDeleted { get; set; }
    }
}
