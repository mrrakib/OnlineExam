using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class TempUser
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string Password { get; set; }
        public string Otp { get; set; }
        public string OtpKey { get; set; }
    }
}