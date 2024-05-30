using Antlr.Runtime.Misc;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Task10.Models;

namespace Task10.Models
{
    [MetadataType(typeof(UserExtended))]
    public partial class User
    {
        public class CheckAge : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
               if(value != null)
                {
                    DateTime date = (DateTime)value;
                 
                    return date.Date.AddYears(18) <= DateTime.Today;
                }
                else
                {
                    return false;
                }
               
            }

        }
       public class CheckPassword :ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if(value != null)
                {
                    string password = (string)value;
                    string rexexp1 = @"[A-Z]+";
                    string rexexp2 = @"[a-z]+";
                    string rexexp3 = @"[0-9]+";
                    string rexexp4 = @"[_@%!#.]+";
                    Regex re1 = new Regex(rexexp1);
                    Regex re2 = new Regex(rexexp2);
                    Regex re3 = new Regex(rexexp3);
                    Regex re4 = new Regex(rexexp4);
                    if (re1.IsMatch(password) && re2.IsMatch(password) && re3.IsMatch(password) && re4.IsMatch(password))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
              

            }

        }

        internal class UserExtended
        {
            [Required(ErrorMessage ="* Email Is required")]
            [RegularExpression(@"^[\w\.-]+@gmail.com$", ErrorMessage = "*Invalid email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "*BirthDate Is required")]
            [CheckAge(ErrorMessage = "*Age Must Be Greater Than 18")]
            public Nullable<System.DateTime> BirthDate { get; set; }

            [RegularExpression(@"^[0-9]+$",ErrorMessage ="*Contact Number Must Contain  digits")]
            [StringLength(10, MinimumLength = 10, ErrorMessage = "Password Should be 10 Characters")]
            public string ContactNo { get; set; }
            [Required(ErrorMessage = "*Full Name Is required")]
            public string Fullname { get; set; }
            //[RegularExpression(@"[a-z]{1,}[A-Z]{1,}[0-9]{1,}", ErrorMessage = "*Password is not valid")]

            //[RegularExpression(@"", ErrorMessage="Password Is Invalid")]
            [CheckPassword(ErrorMessage = "Password Is Invalid")]
            public string Password { get; set; }
            [Required(ErrorMessage = "*Type Is required")]
            public string Type { get; set; }
         
        }
        
    }
}