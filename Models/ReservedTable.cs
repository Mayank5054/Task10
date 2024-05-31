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
    
    public partial class ReservedTable
    {
        public int ReservedTableId { get; set; }
        public Nullable<int> TableId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> NoOFPeople { get; set; }
        public Nullable<System.DateTime> FromDateTime { get; set; }
        public Nullable<System.DateTime> ToDateTime { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> IsDeleted { get; set; }
    
        public virtual DineInTable DineInTable { get; set; }
        public virtual User User { get; set; }
    }
}