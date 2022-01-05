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
    public class ModelTestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        // GET: Subjects
        private readonly IBatchService _batchService;
        private readonly IMCQQuestionsService _mCQQuestionsService;
        private readonly IExamService _examService;
        private readonly IExamWiseQuestionService _examWiseQuestionService;
        private readonly IResultSummaryService _resultSummaryService;
        public ModelTestController()
        {
            _batchService = new BatchService(db);
            _mCQQuestionsService = new MCQQuestionsService(db);
            _examService = new ExamService(db);
            _examWiseQuestionService = new ExamWiseQuestionService(db);
            _resultSummaryService = new ResultSummaryService(db);
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
            return View(_examService.GetPageList(page.Value, NoOfRows.Value, searchString));
        }

        // GET: ModelTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam model = _examService.GetDetails(id ?? 0);

            if (model == null)
            {
                return HttpNotFound();
            }
            List<int> quesIds = _examWiseQuestionService.GetAllByExamId(model.Id).Select(s => s.QuestionId).ToList();
            List<VMQuestionForModelTest> questions = _mCQQuestionsService.GetAllListTo().Where(s => quesIds.Any(b => b == s.QuestionId)).ToList();
            model.QuestionForModelTests = questions;
            return View(model);
        }

        // GET: ModelTest/Create
        public ActionResult Create()
        {
            Exam exam = new Exam();
            List<VMQuestionForModelTest> questions = _mCQQuestionsService.GetAllListTo();
            exam.QuestionForModelTests = questions.OrderBy(q => q.QuestionTypeName).ToList();
            ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name");
            return View(exam);
        }

        // POST: ModelTest/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exam model)
        {
            if (ModelState.IsValid)
            {
                if (_examService.SaveBulk(model))
                {
                    _message.save(this);
                    return RedirectToAction("Index");
                }
            }
            _message.custom(this,"No data saved.");
            ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name", model.BatchId);
            return View(model);
        }

        // GET: ModelTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam model = _examService.GetDetails(id ?? 0);

            if (model == null)
            {
                return HttpNotFound();
            }
            List<ExamWiseQuestion> selectedQuestions = _examWiseQuestionService.GetAllByExamId(model.Id);
            List<VMQuestionForModelTest> questions = _mCQQuestionsService.GetAllListTo();
            if (selectedQuestions.Count > 0 && questions.Count > 0)
            {
                foreach (var question in selectedQuestions)
                {
                    questions.Where(s => s.QuestionId == question.QuestionId).First().IsIncluded = true;
                }
            }
            model.QuestionForModelTests = questions;
            ViewBag.BatchId = new SelectList(_batchService.GetAllBatchDDL(), "Id", "Name", model.BatchId);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Exam model)
        {
            if (ModelState.IsValid)
            {
                if (_examService.UpdateBulk(model))
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
            if (_resultSummaryService.IsExamHeld(id ?? 0))
            {
                _message.custom(this, "One or more student has taken exam with this model test, model test can't be deleted!");
                return RedirectToAction("Index", new { searchString = currentFilter, currentFilter = currentFilter, page = page, NoOfRows = NoOfRows });
            }
            if(_examService.DeleteBulk(id ?? 0))
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
