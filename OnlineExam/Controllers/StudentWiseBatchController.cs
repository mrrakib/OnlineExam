using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using System.Net;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class StudentWiseBatchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        // GET: Subjects
        private readonly IBatchService _batchService;
        private readonly IStudentWiseBatchService _studentWiseBatchService;
        public StudentWiseBatchController()
        {
            _studentWiseBatchService = new StudentWiseBatchService(db);
            _batchService = new BatchService(db);
        }
        public ActionResult Index(string currentFilter, string searchString, int? page = 1, int? NoOfRows = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.page = page;
            ViewBag.CurrentFilter = searchString;
            ViewBag.NoOfRows = NoOfRows;
            return View(_studentWiseBatchService.GetPageList(User.GETSTUDENTID(),page.Value, NoOfRows.Value));
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentWiseBatch topic = _studentWiseBatchService.GetDetails(id ?? 0);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name");
            return View();
        }

        // POST: Subjects/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentWiseBatch model)
        {
            if (ModelState.IsValid)
            {
                model.StudentId = User.GETSTUDENTID();
                if (_studentWiseBatchService.IsExists(model.BatchId, model.StudentId))
                {
                    _message.custom(this, "You already have enrolled in selected batch.");
                    ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name", model.BatchId);
                    return View(model);
                }
                if (_studentWiseBatchService.Add(model))
                {
                    _message.save(this);
                    return RedirectToAction("Index");
                }
            }
            _message.custom(this,"No data saved.");
            ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name", model.BatchId);
            return View(model);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentWiseBatch model = _studentWiseBatchService.GetDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name", model.BatchId);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentWiseBatch model)
        {
            if (ModelState.IsValid)
            {
                if (_studentWiseBatchService.IsExistsUpdate(model.BatchId, model.StudentId, model.Id))
                {
                    _message.custom(this, "You already have enrolled in selected batch.");
                    ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name", model.BatchId);
                    return View(model);
                }
                if (_studentWiseBatchService.Update(model))
                {
                    _message.update(this);
                    return RedirectToAction("Index");
                }
                
            }
            _message.custom(this, "No data updated.");
            ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name", model.BatchId);
            return View(model);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(string currentFilter, int? id, int? page = 1,  int? NoOfRows = 10)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(_studentWiseBatchService.Delete(id ?? 0))
            {
                _message.delete(this);
            }
            else
            {
                _message.custom(this, "No data deleted.");
            }
            return RedirectToAction("Index",new { searchString= currentFilter, currentFilter = currentFilter, page = page , NoOfRows = NoOfRows });
        }


    }
}
