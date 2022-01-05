using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using OnlineExam.ViewModels;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class SBAQuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        // GET: Subjects
        private readonly ISubjectsService _subjectsService;
        private readonly IChapterService _chapterService;
        private readonly ITopicService _topicService;
        private readonly IMCQQuestionsService _mCQQuestionsService;
        public SBAQuestionsController()
        {
            _subjectsService = new SubjectsService(db);
            _topicService = new TopicService(db);
            _chapterService = new ChapterService(db);
            _mCQQuestionsService = new MCQQuestionsService(db);
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
            return View(_mCQQuestionsService.GetPageList(page.Value, NoOfRows.Value, searchString, false));
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MCQQuestion model = _mCQQuestionsService.GetDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName");
            ViewBag.ChapterId = new SelectList(_chapterService.GetChapterBySubjectId(0), "Id", "ChapterName");
            ViewBag.TopicId = new SelectList(_topicService.GetTopicByChapterId(0), "Id", "TopicName");
            VMMCQQuestion question = new VMMCQQuestion();
            List<VMMCQQuestionOption> optionList = new List<VMMCQQuestionOption>();
            VMMCQQuestionOption optionOne = new VMMCQQuestionOption();
            VMMCQQuestionOption optionTwo = new VMMCQQuestionOption();
            VMMCQQuestionOption optionThree = new VMMCQQuestionOption();
            VMMCQQuestionOption optionFour = new VMMCQQuestionOption();
            VMMCQQuestionOption optionFive = new VMMCQQuestionOption();
            optionList.Add(optionOne);
            optionList.Add(optionTwo);
            optionList.Add(optionThree);
            optionList.Add(optionFour);
            optionList.Add(optionFive);
            question.IsMCQ = false;
            question.MCQQuestionOptions = optionList;
            return View(question);
        }

        // POST: Subjects/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<int> SubjectId, List<int> ChapterId, List<int> TopicId, VMMCQQuestion model)
        {

            if (ModelState.IsValid)
            {
                int err = 0;
                //foreach (var item in TopicId)
                //{
                //    var topic = _topicService.GetDetails(item);
                //    model.TopicId = topic.Id;
                //    model.ChapterId = topic.ChapterId;
                //    model.SubjectId = topic.Chapter.SubjectId;

                //    var question = _mCQQuestionsService.VMToQuestionModel(model);
                //    int answer = model.AnswerIndex;
                //    question.MCQQuestionOptions.ForEach(m => m.IsAnswer = false);
                //    question.MCQQuestionOptions[answer].IsAnswer = true;
                //    if (!_mCQQuestionsService.Add(question))
                //    {
                //        err++;
                //    }
                //}
                foreach (var item in ChapterId)
                {
                    var topic = _topicService.GetFirstTopicByChapterId(item);
                    model.TopicId = topic.Id;
                    model.ChapterId = topic.ChapterId;
                    model.SubjectId = topic.Chapter.SubjectId;

                    var question = _mCQQuestionsService.VMToQuestionModel(model);
                    int answer = model.AnswerIndex;
                    question.MCQQuestionOptions.ForEach(m => m.IsAnswer = false);
                    question.MCQQuestionOptions[answer].IsAnswer = true;
                    if (!_mCQQuestionsService.Add(question))
                    {
                        err++;
                    }
                }
                if (err == 0)
                {
                    _message.save(this);
                    return RedirectToAction("Index");
                }

            }
            _message.custom(this, "No data saved.");
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName", model.SubjectId);
            ViewBag.ChapterId = new SelectList(_chapterService.GetChapterBySubjectId(model.SubjectId), "Id", "ChapterName", model.ChapterId);
            ViewBag.TopicId = new SelectList(_topicService.GetTopicByChapterId(model.ChapterId), "Id", "TopicName", model.TopicId);
            return View(model);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MCQQuestion model = _mCQQuestionsService.GetDetails(id ?? 0);
            var question = _mCQQuestionsService.ModelToQuestionVM(model);
            int index = question.MCQQuestionOptions.FindIndex(a => a.IsAnswer == true);
            question.AnswerIndex = index;
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName", model.SubjectId);
            ViewBag.ChapterId = new SelectList(_chapterService.GetChapterBySubjectId(model.SubjectId), "Id", "ChapterName", model.ChapterId);
            ViewBag.TopicId = new SelectList(_topicService.GetTopicByChapterId(model.ChapterId), "Id", "TopicName", model.TopicId);
            return View(question);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VMMCQQuestion model)
        {
            if (ModelState.IsValid)
            {
                #region Dummy Topic
                var topic = _topicService.GetFirstTopicByChapterId(model.ChapterId);
                model.TopicId = topic.Id;
                #endregion
                var question = _mCQQuestionsService.VMToQuestionModel(model);
                int answer = model.AnswerIndex;
                question.MCQQuestionOptions.ForEach(m => m.IsAnswer = false);
                question.MCQQuestionOptions[answer].IsAnswer = true;
                if (_mCQQuestionsService.Update(question))
                {
                    _message.update(this);
                    return RedirectToAction("Index");
                }

            }
            _message.custom(this, "No data updated.");
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName", model.SubjectId);
            ViewBag.ChapterId = new SelectList(_chapterService.GetChapterBySubjectId(model.SubjectId), "Id", "ChapterName", model.ChapterId);
            ViewBag.TopicId = new SelectList(_topicService.GetTopicByChapterId(model.ChapterId), "Id", "TopicName", model.TopicId);
            return View(model);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(string currentFilter, int? id, int? page = 1, int? NoOfRows = 10)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (_mCQQuestionsService.Delete(id ?? 0))
            {
                _message.delete(this);
            }
            else
            {
                _message.custom(this, "No data deleted.");
            }
            return RedirectToAction("Index", new { searchString = currentFilter, currentFilter = currentFilter, page = page, NoOfRows = NoOfRows });
        }
        [AllowAnonymous]
        public JsonResult GetChapterBySubjectId(List<int> subjectId)
        {
            //var chapters = _chapterService.GetChapterBySubjectId(subjectId);
            List<Chapter> res = new List<Chapter>();
            foreach (var item in subjectId)
            {
                var chapters = _chapterService.GetChapterBySubjectId(item);
                res.AddRange(chapters);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetTopicByChapterId(List<int> chapterId)
        {
            List<Topic> res = new List<Topic>();
            foreach (var item in chapterId)
            {
                var topics = _topicService.GetTopicByChapterId(item);
                res.AddRange(topics);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetChapterBySingleSubjectId(int subjectId)
        {
            var chapters = _chapterService.GetChapterBySubjectId(subjectId);

            return Json(chapters, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetTopicBySingleChapterId(int chapterId)
        {
            var topics = _topicService.GetTopicByChapterId(chapterId);
            return Json(topics, JsonRequestBehavior.AllowGet);
        }
    }
}
