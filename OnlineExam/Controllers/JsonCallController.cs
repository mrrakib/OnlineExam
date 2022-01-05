using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class JsonCallController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IMCQQuestionsService _questionsService;
        private readonly IMCQOptionsService _optionsService;
        private readonly ISubjectsService _subjectsService;
        private readonly IChapterService _chapterService;
        private readonly ITopicService _topicService;
        private readonly IBatchService _batchService;

        public JsonCallController()
        {
            _questionsService = new MCQQuestionsService(db);
            _optionsService = new MCQOptionsService(db);
            _subjectsService = new SubjectsService(db);
            _chapterService = new ChapterService(db);
            _topicService = new TopicService(db);
            _batchService = new BatchService(db);
        }

        #region Selected Choose Options
        [HttpPost]
        public JsonResult IsTrueOrFalseSelectedOptionSBA(int id, int selectedOptionId)
        {
            bool isCorrect = false;
            var option = _optionsService.GetDetails(selectedOptionId);
            if (option.MCQQuestionId == id)
            {
                isCorrect = option.IsAnswer;
            }

            return Json(new { isCorrect, message = isCorrect ? "Correct!" : "Wrong!", questionId = id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsTrueOrFalseSelectedOptionMCQ(int id, int selectedOptionId, bool selectedValue = false)
        {
            bool isCorrect = false;
            var option = _optionsService.GetDetails(selectedOptionId);
            if (option.MCQQuestionId == id)
            {
                isCorrect = option.IsAnswer;
                if (selectedValue == isCorrect)
                {
                    isCorrect = true;
                }
                else
                {
                    isCorrect = false;
                }
            }
            return Json(new { isCorrect, message = isCorrect ? "Correct!" : "Wrong!", questionId = id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAnswerMCQ(int id)
        {
            var questions = _questionsService.GetDetails(id);
            var tOptionId = questions.MCQQuestionOptions.FindAll(a => a.IsAnswer).Select(a => new { id = "#Q" + a.MCQQuestionId + "O" + a.Id, nameRadioButton = "GroupName" + a.Id, questionId = a.MCQQuestionId }).ToList();

            if (questions != null)
            {
                return Json(new { tOptionId }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAnswerSBA(int id)
        {
            var questions = _questionsService.GetDetails(id);
            var checkedOption = questions.MCQQuestionOptions.FirstOrDefault(a => a.IsAnswer);
            if (questions != null && checkedOption != null)
            {
                return Json(new { questionId = questions.Id, optionId = checkedOption.Id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { questionId = 0, optionId = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetExplanation(int id)
        {
            var questions = _questionsService.GetDetails(id);
            return PartialView("Explanation", questions);
        }
        [HttpGet]
        public JsonResult GetSubjectByBatchId(int batchId)
        {
            var subjects = _batchService.GetAllSubjectByBatchId(batchId);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetChapterBySubjectId(int subjectId)
        {
            var chapters = _chapterService.GetChapterBySubjectId(subjectId);
            return Json(chapters, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetTopicByChapterId(int chapterId)
        {
            var chapters = _topicService.GetTopicByChapterId(chapterId);
            return Json(chapters, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
