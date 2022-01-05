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

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class SubjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        // GET: Subjects
        private readonly ISubjectsService _subjectsService;
        public SubjectsController()
        {
            _subjectsService = new SubjectsService(db);
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
            return View(_subjectsService.GetPageList(page.Value, NoOfRows.Value, searchString));
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = _subjectsService.GetDetails(id ?? 0);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Subject model)
        {
            if (ModelState.IsValid)
            {
                if (_subjectsService.Add(model))
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
            Subject model = _subjectsService.GetDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Subject model)
        {
            if (ModelState.IsValid)
            {
                if (_subjectsService.Update(model))
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
            if(_subjectsService.Delete(id ?? 0))
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
