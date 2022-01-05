using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class DashboardInfoViewModel
    {
        public int TotalAdmin { get; set; }
        public int TotalStudent { get; set; }
        public int TotalOnlineTestExam { get; set; }
        public int TotalDailyOnlineTestExam { get; set; }
        public int TotalMonthlyOnlineTestExam { get; set; }
    }
}