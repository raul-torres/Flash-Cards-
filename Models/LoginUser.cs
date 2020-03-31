using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FlashCards.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Email is required.")]
        public string LEmail{get;set;}



        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string LPassword{get;set;}
    }

}