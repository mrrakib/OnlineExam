using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class HomeController : Controller
    {
        private readonly IDashboardInfoService _dashboardInfoService;

        public HomeController()
        {
            _dashboardInfoService = new DashboardInfoService(new ApplicationDbContext());
        }

        public ActionResult Index()
        {
            if (User.GETROLENAME() == "Student")
            {
                return RedirectToAction("StudentIndex", "Home");
            }
            else
            {
                var model = _dashboardInfoService.GetDetails();
                return View(model);
            }       
        }

        public ActionResult StudentIndex()
        {
            var model = _dashboardInfoService.GetDetails(User.GETSTUDENTID());
            return View(model);
        }
    
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}