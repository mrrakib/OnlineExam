using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using OnlineExam.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    public class PublishedExamController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly Message _message = new Message();
        private readonly IMCQQuestionsService _questionsService;
        private readonly IMCQOptionsService _optionsService;
        private readonly IChapterService _chapterService;
        private readonly ISubjectsService _subjectsService;
        private readonly ITopicService _topicService;
        private readonly IResultSheetService _resultSheetService;
        private readonly IResultSummaryService _resultSummaryService;
        private readonly IAnswerSheetService _answerSheetService;
        private readonly IBatchService _batchService;

        public PublishedExamController()
        {
            _questionsService = new MCQQuestionsService(db);
            _optionsService = new MCQOptionsService(db);
            _subjectsService = new SubjectsService(db);
            _chapterService = new ChapterService(db);
            _topicService = new TopicService(db);
            _resultSheetService = new ResultSheetService(db);
            _resultSummaryService = new ResultSummaryService(db);
            _answerSheetService = new AnswerSheetService(db);
            _batchService = new BatchService(db);
        }

        // GET: Exam
        public ActionResult Index()
        {
            ViewBag.ExamId = new SelectList(_questionsService.GetAllExamsByStudentId(User.GETSTUDENTID()), "Id", "ExamName");
            return View();
        }

        // GET: Exam/Start
        [HttpPost]
        public ActionResult Start(int ExamId)
        {
            var res=_resultSummaryService.GetDetailsByExamIdAndStudentId(ExamId,User.GETSTUDENTID());
            if (res != null)
            {
                _message.custom(this, "You already attened on your selected exam.");
                return RedirectToAction("Index");
            }
            else
            {
                var questions = _questionsService.GetQuestionsByExamId(ExamId);
                if (questions != null)
                {
                    List<MCQQuestion> finalQuestions = new List<MCQQuestion>();
                    List<MCQQuestion> mCQQuestions = questions.Where(m => m.IsMCQ).ToList();
                    finalQuestions.AddRange(mCQQuestions);
                    List<MCQQuestion> sBAQuestions = questions.Where(m => !m.IsMCQ).ToList();
                    finalQuestions.AddRange(sBAQuestions);
                    finalQuestions.ForEach(m => m.ExamId = ExamId);
                    ViewBag.ExamTimeInMinutes = finalQuestions.Count * 0.5;  //Per question will be 30 seconds
                    return View("StartExam", finalQuestions);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            
            
            
        }

        // GET: Exam/Finish
        [HttpPost]
        public ActionResult Finish(List<MCQQuestion> model)
        {
            if (model == null)
                return HttpNotFound();
            var examId = model.First().ExamId;
            //int totalExamCount = _resultSheetService.GetMaxExamCount(userOrStudentId: User.GETSTUDENTID());
            var resultSheets = new List<ResultSheet>();
            var answerSheets = new List<AnswerSheet>();

            foreach (var question in model)
            {
                var originalAnswer = _questionsService.GetDetails(question.Id);
                if (originalAnswer is null)
                    continue;

                var result = new ResultSheet();
                result.ExamId = examId;
                result.QuestionId = question.Id;
                result.StudentId = User.GETSTUDENTID();
                result.ActualMark = originalAnswer.Mark;
                result.IsMCQ = originalAnswer.IsMCQ;
                result.ExamCount = 0;
                result.ExamDateTime = DateTime.Now;

                var answerSheet = new AnswerSheet();
                answerSheet.ExamId = examId;
                answerSheet.QuestionId = question.Id;
                answerSheet.StudentId = User.GETSTUDENTID();
                answerSheet.ExamCount = 0;
                answerSheet.IsMCQ = originalAnswer.IsMCQ;

                var trueAnswerOptionList = new List<int>();
                var falseAnswerOptionList = new List<int>();

                if (originalAnswer.IsMCQ)
                {
                    int totalOptionCount = originalAnswer.MCQQuestionOptions.Count;
                    int mark = originalAnswer.Mark;
                    double perOptionCorrectMark = (Convert.ToDouble(mark) / Convert.ToDouble(totalOptionCount));

                    foreach (var originalOption in originalAnswer.MCQQuestionOptions)
                    {
                        var optioncheckingByStudent = question.MCQQuestionOptions.FirstOrDefault(a => a.Id == originalOption.Id);
                        if (optioncheckingByStudent is null)
                            continue;

                        if (optioncheckingByStudent != null && originalOption.Id == optioncheckingByStudent.CheckedOptionId)
                            trueAnswerOptionList.Add(originalOption.Id);

                        if (originalOption.IsAnswer && originalOption.Id == optioncheckingByStudent.CheckedOptionId || !originalOption.IsAnswer && optioncheckingByStudent.CheckedOptionId == 0)
                        {
                            result.CorrectOptionCount = result.CorrectOptionCount + 1;
                            result.ObtainMark = result.ObtainMark + perOptionCorrectMark;
                        }
                    }
                }
                else
                {
                    var correctAns = originalAnswer.MCQQuestionOptions.FirstOrDefault(a => a.Id.Equals(question.CheckedOptionId));
                    if (correctAns != null && correctAns.IsAnswer)
                    {
                        trueAnswerOptionList.Add(correctAns.Id);
                        result.ObtainMark = originalAnswer.Mark;
                        result.CorrectOptionCount = 1;
                    }
                    else if (correctAns != null && correctAns.IsAnswer == false)
                    {
                        falseAnswerOptionList.Add(correctAns.Id);
                        result.ObtainMark = 0;
                        result.CorrectOptionCount = 0;
                    }
                    else
                    {
                        result.ObtainMark = 0;
                        result.CorrectOptionCount = 0;
                    }
                }
                resultSheets.Add(result);
                answerSheet.TrueOptionId = String.Join(",", trueAnswerOptionList);
                answerSheet.FalseOptionId = String.Join(",", falseAnswerOptionList);
                answerSheets.Add(answerSheet);
            }
            
            var saveResult = _resultSheetService.Add(resultSheets);
            var saveAnswerOptionResult = _answerSheetService.Add(answerSheets);
            var resultStatus = _resultSheetService.GetExamStatus(examId,userOrStudentId: User.GETSTUDENTID());
            var addResultSummary = _resultSummaryService.Add(resultStatus);
            var questionHistory = _answerSheetService.GetExamHistoryExamId(studentId: User.GETSTUDENTID(), examId: examId);
           
            resultStatus.QuestionHistory = questionHistory;
            ViewBag.IsMCQ = model.First().IsMCQ;

            return View(resultStatus);
        }

        // GET: Exam/History
        [HttpGet]
        public ActionResult History(int page = 1, int NoOfRows = 30)
        {
            ViewBag.page = page;
            ViewBag.NoOfRows = NoOfRows;
            ViewBag.searchString = "";
            ViewBag.RoleName = User.GETROLENAME();
            if (User.GETROLENAME() == "Admin")
            {
                var data = _resultSummaryService.GetPageList(page, NoOfRows, "");
                return View(data);
            }
            var resultSummary = _resultSummaryService.GetPageList(page, NoOfRows, "", User.GETSTUDENTID());
            return View(resultSummary);
        }

        [HttpPost]
        public ActionResult History(int page = 1, int NoOfRows = 30, string searchString = "")
        {
            ViewBag.page = page;
            ViewBag.NoOfRows = NoOfRows;
            ViewBag.searchString = searchString;
            ViewBag.RoleName = User.GETROLENAME();
            if (User.GETROLENAME() == "Admin")
            {
                var data = _resultSummaryService.GetPageList(page, NoOfRows, searchString);
                return View(data);
            }
            var resultSummary = _resultSummaryService.GetPageList(page, NoOfRows, searchString, User.GETSTUDENTID());
            return View(resultSummary);
        }

        // GET: Exam/Details
        [HttpGet]
        public ActionResult Details(string studentId, int examId)
        {
            ViewBag.RoleName = User.GETROLENAME();

            var data = _answerSheetService.GetExamHistoryExamId(studentId: studentId, examId: examId);

            if (data.Count > 0 && data.First().IsMCQ)
            {
                return View("DetailMCQ", data);
            }
            return View("DetailSBA", data);
        }

        // GET: Exam/Timer
        [HttpGet]
        public ActionResult Timer()
        {
            ViewBag.ExamTimeInMinutes = 1;
            return View();
        }
    }
}
