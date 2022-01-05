using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using System.Net;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class TopicsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        // GET: Subjects
        private readonly IChapterService _chapterService;
        private readonly ITopicService _topicService;
        public TopicsController()
        {
            _topicService = new TopicService(db);
            _chapterService = new ChapterService(db);
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
            return View(_topicService.GetPageList(page.Value, NoOfRows.Value, searchString));
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = _topicService.GetDetails(id ?? 0);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            ViewBag.ChapterId = new SelectList(_chapterService.GetAll(), "Id", "ChapterName");
            return View();
        }

        // POST: Subjects/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Topic model)
        {
            if (ModelState.IsValid)
            {
                if (_topicService.Add(model))
                {
                    _message.save(this);
                    return RedirectToAction("Index");
                }
            }
            _message.custom(this,"No data saved.");
            ViewBag.ChapterId = new SelectList(_chapterService.GetAll(), "Id", "ChapterName", model.ChapterId);
            return View(model);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic model = _topicService.GetDetails(id ?? 0);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChapterId = new SelectList(_chapterService.GetAll(), "Id", "ChapterName", model.ChapterId);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Topic model)
        {
            if (ModelState.IsValid)
            {
                if (_topicService.Update(model))
                {
                    _message.update(this);
                    return RedirectToAction("Index");
                }
                
            }
            _message.custom(this, "No data updated.");
            ViewBag.ChapterId = new SelectList(_chapterService.GetAll(), "Id", "ChapterName", model.ChapterId);
            return View(model);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(string currentFilter, int? id, int? page = 1,  int? NoOfRows = 10)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(_topicService.Delete(id ?? 0))
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
