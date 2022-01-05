using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.ViewModels;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class QuestionBankController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly Message _message = new Message();
        private readonly IMCQQuestionsService _questionsService;
        private readonly IMCQOptionsService _optionsService;
        private readonly IBatchService _batchService;
        private readonly IChapterService _chapterService;
        private readonly ISubjectsService _subjectsService;
        private readonly ITopicService _topicService;
        public QuestionBankController()
        {
            _batchService = new BatchService(db);
            _questionsService = new MCQQuestionsService(db);
            _optionsService = new MCQOptionsService(db);
            _subjectsService = new SubjectsService(db);
            _chapterService = new ChapterService(db);
            _topicService = new TopicService(db);
        }

        #region Choose Options
        public ActionResult ChooseTopics()
        {
            return View();
        }
        #endregion

        // GET: QuestionBank/ReadQuestions
        [HttpGet]
        public ActionResult ReadQuestions()
        {
            ViewBag.BatchId = new SelectList(_batchService.GetAllActiveBatchByStudentID(User.GETSTUDENTID()), "Id", "BatchName");
            ViewBag.SubjectId = new SelectList(_batchService.GetAllSubjectByBatchId(0), "Id", "SubjectName");
            ViewBag.ChapterId = new SelectList(_chapterService.GetChapterBySubjectId(0), "Id", "ChapterName");
            ViewBag.TopicId = new SelectList(_topicService.GetTopicByChapterId(0), "Id", "TopicName");
            ViewBag.QuestionType = new SelectList(_questionsService.GetAllQuestionType(), "Name", "Name");

            var model = new ExamInformationViewModel() { TotalQuestion = 20 };
            return View(model);
        }

        [HttpPost]
        public ActionResult ReadQuestions(ExamInformationViewModel model, int page = 1, int noOfRows = 10, string searchString = "")
        {
            if (model.QuestionType.Contains("MCQ"))
            {
                var mcq = _questionsService.GetQuestionsWithPageList(page: page, noOfRows: noOfRows, searchString: searchString, subjectId: model.SubjectId, chapterId: model.ChapterId, topicId: model.TopicId, isMcq: true, isFullQuestionLoad: model.IsFullQuestionLoad, totalNoOfQuestion: model.TotalQuestion);
                return View("ReadMCQ", mcq);
            }
            var sba = _questionsService.GetQuestionsWithPageList(page: page, noOfRows: noOfRows, searchString: searchString, subjectId: model.SubjectId, chapterId: model.ChapterId, topicId: model.TopicId, isMcq: false, isFullQuestionLoad: model.IsFullQuestionLoad, totalNoOfQuestion: model.TotalQuestion);
            return View("ReadSBA", sba);
        }
    }
}
