using OnlineExam.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Data.Services
{
    public interface IDashboardInfoService
    {
        DashboardInfoViewModel GetDetails(string studentId="");
    }

    public class DashboardInfoService : IDashboardInfoService
    {

        private readonly ApplicationDbContext _context;

        public DashboardInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public DashboardInfoViewModel GetDetails(string studentId = "")
        {
            var sp = $"EXEC GetDashboardInfo @StudentId='{studentId}'";
            var resultStatus = _context.Database.SqlQuery<DashboardInfoViewModel>(sp).FirstOrDefault();
            return resultStatus;
        }
    }
}