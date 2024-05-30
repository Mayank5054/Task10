using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task10.Models
{
    [MetadataType(typeof(LoginExtended))]
    public partial class LoginModel
    {
        internal class LoginExtended
        {
            [Required(ErrorMessage ="*Email Is Required")]
            [RegularExpression(@"^[\w\.-]+@gmail.com$", ErrorMessage = "*Invalid email address")]
            public string Email { get; set; }
            [Required(ErrorMessage = "*password Is Required")]
            public string password { get; set; }
        }
    }
}