using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class VMStudentWiseBatch
    {
        public int Id { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string BatchName { get; set; }
        public bool IsActive { get; set; }
    }
}