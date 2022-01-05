using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using System.Net;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class ChaptersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        // GET: Subjects
        private readonly ISubjectsService _subjectsService;
        private readonly IChapterService _chapterService;
        private readonly ITopicService _topicService;
        public ChaptersController()
        {
            _subjectsService = new SubjectsService(db);
            _chapterService = new ChapterService(db);
            _topicService = new TopicService(db);
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
            return View(_chapterService.GetPageList(page.Value, NoOfRows.Value, searchString));
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter subject = _chapterService.GetDetails(id ?? 0);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName");
            return View();
        }

        // POST: Subjects/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Chapter model)
        {
            if (ModelState.IsValid)
            {
                if (_chapterService.Add(model))
                {
                    var topic = new Topic {ChapterId=model.Id,TopicName=model.ChapterName+"DemmyTopic", };
                    _topicService.Add(topic);
                    _message.save(this);
                    return RedirectToAction("Index");
                }
            }
            _message.custom(this,"No data saved.");
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName",model.SubjectId);
            return View(model);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter model = _chapterService.GetDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName", model.SubjectId);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Chapter model)
        {
            if (ModelState.IsValid)
            {
                if (_chapterService.Update(model))
                {
                    var topic=_topicService.GetFirstTopicByChapterId(model.Id);
                    if (topic == null)
                    {
                        var newTopic = new Topic { ChapterId = model.Id, TopicName = model.ChapterName + "DemmyTopic", };
                        _topicService.Add(newTopic);
                    }
                    _message.update(this);
                    return RedirectToAction("Index");
                }
                
            }
            _message.custom(this, "No data updated.");
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAll(), "Id", "SubjectName", model.SubjectId);
            return View(model);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(string currentFilter, int? id, int? page = 1,  int? NoOfRows = 10)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(_chapterService.Delete(id ?? 0))
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
