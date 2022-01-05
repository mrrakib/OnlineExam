using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using OnlineExam.ViewModels;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class BatchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        // GET: Subjects
        private readonly IBatchService _batchService;
        private readonly ISubjectsService _subjectsService;
        private readonly IBatchWiseSubjectService _batchWiseSubjectService;
        public BatchController()
        {
            _batchService = new BatchService(db);
            _subjectsService = new SubjectsService(db);
            _batchWiseSubjectService = new BatchWiseSubjectService(db);
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
            return View(_batchService.GetPageList(page.Value, NoOfRows.Value, searchString));
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch model = _batchService.GetDetails(id ?? 0);
            
            if (model == null)
            {
                return HttpNotFound();
            }
            List<int> subIds = _batchWiseSubjectService.GetAllByBatchId(model.Id).Select(s => s.SubjectId).ToList();
            List<TOSubject> subjects = _subjectsService.GetAllListTo().Where(s => subIds.Any(b => b == s.Id)).ToList();
            model.Subjects = subjects;
            return View(model);
        }

        // GET: Batch/Create
        public ActionResult Create()
        {
            Batch batch = new Batch();
            List<TOSubject> subjects = _subjectsService.GetAllListTo();
            batch.Subjects = subjects;
            return View(batch);
        }

        // POST: Batch/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Batch model)
        {
            if (ModelState.IsValid)
            {
                if (_batchService.SaveBulk(model))
                {
                    _message.save(this);
                    return RedirectToAction("Index");
                }
            }
            _message.custom(this,"No data saved.");
            return View(model);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch model = _batchService.GetDetails(id ?? 0);
            
            if (model == null)
            {
                return HttpNotFound();
            }
            List<BatchWiseSubject> selectedSubjects = _batchWiseSubjectService.GetAllByBatchId(model.Id);
            List<TOSubject> subjects = _subjectsService.GetAllListTo();
            if (selectedSubjects.Count > 0 && subjects.Count > 0)
            {
                foreach (var sub in selectedSubjects)
                {
                    subjects.Where(s => s.Id == sub.SubjectId).First().IsSelected = true;
                }
            }
            model.Subjects = subjects;
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Batch model)
        {
            if (ModelState.IsValid)
            {
                if (_batchService.UpdateBulk(model))
                {
                    _message.update(this);
                    return RedirectToAction("Index");
                }
                
            }
            _message.custom(this, "No data updated.");
            return View(model);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(string currentFilter, int? id, int? page = 1,  int? NoOfRows = 10)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(_batchService.DeleteBulk(id ?? 0))
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
