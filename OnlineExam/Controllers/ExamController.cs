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
    public class ExamController : Controller
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

        public ExamController()
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
            ViewBag.BatchId = new SelectList(_batchService.GetAllActiveBatchByStudentID(User.GETSTUDENTID()), "Id", "BatchName");
            ViewBag.SubjectId = new SelectList(_batchService.GetAllSubjectByBatchId(0), "Id", "SubjectName");
            ViewBag.ChapterId = new SelectList(_chapterService.GetChapterBySubjectId(0), "Id", "ChapterName");
            ViewBag.TopicId = new SelectList(_topicService.GetTopicByChapterId(0), "Id", "TopicName");
            ViewBag.QuestionType = new SelectList(_questionsService.GetAllQuestionType(), "Name", "Name");

            var model = new ExamInformationViewModel() { TotalQuestion = 20 };
            return View(model);
        }

        // GET: Exam/Start
        [HttpPost]
        public ActionResult Start(ExamInformationViewModel model)
        {
            bool isMcq = false;
            if (model.QuestionType.Equals("MCQ"))
                isMcq = true;
            var questions = _questionsService.GetQuestions(subjectId: model.SubjectId, chapterId: model.ChapterId, topicId: model.TopicId, isMcq: isMcq, isFullQuestionLoad: model.IsFullQuestionLoad, totalNoOfQuestion: model.TotalQuestion);
            ViewBag.ExamTimeInMinutes = questions.Count * 0.5;  //Per question will be 30 seconds

            if (isMcq)
                return View("StartMCQ", questions);

            return View("StartSBA", questions);
        }

        // GET: Exam/Finish
        [HttpPost]
        public ActionResult Finish(List<MCQQuestion> model)
        {
            if (model == null)
                return HttpNotFound();

            int totalExamCount = _resultSheetService.GetMaxExamCount(userOrStudentId: User.GETSTUDENTID());
            var resultSheets = new List<ResultSheet>();
            var answerSheets = new List<AnswerSheet>();

            foreach (var question in model)
            {
                var originalAnswer = _questionsService.GetDetails(question.Id);
                if (originalAnswer is null)
                    continue;

                var result = new ResultSheet();
                result.QuestionId = question.Id;
                result.StudentId = User.GETSTUDENTID();
                result.ActualMark = originalAnswer.Mark;
                result.IsMCQ = originalAnswer.IsMCQ;
                result.ExamCount = totalExamCount;
                result.ExamDateTime = DateTime.Now;

                var answerSheet = new AnswerSheet();
                answerSheet.QuestionId = question.Id;
                answerSheet.StudentId = User.GETSTUDENTID();
                answerSheet.ExamCount = totalExamCount;
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
           
            var resultStatus = _resultSheetService.GetResultStatus(userOrStudentId: User.GETSTUDENTID(), totalExamCount: totalExamCount, subjectId: model.FirstOrDefault().SubjectId, chapterId: model.FirstOrDefault().ChapterId, topicId: model.FirstOrDefault().TopicId);
     
            var addResultSummary = _resultSummaryService.Add(resultStatus);
            var questionHistory = _answerSheetService.GetExamHistory(studentId: User.GETSTUDENTID(), examCount: totalExamCount);
           
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
            var resultSummary = _resultSummaryService.GetPageList(page, NoOfRows, "", User.GETSTUDENTID());
            return View(resultSummary);
        }

        // GET: Exam/Details
        [HttpGet]
        public ActionResult Details(string studentId, int examCount)
        {
            ViewBag.RoleName = User.GETROLENAME();

            var data = _answerSheetService.GetExamHistory(studentId: studentId, examCount: examCount);

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
