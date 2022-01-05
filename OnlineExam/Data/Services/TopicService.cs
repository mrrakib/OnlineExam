using OnlineExam.Models;
using OnlineExam.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OnlineExam.Data.Services
{
    public interface ITopicService : IDisposable
    {
        List<Topic> GetAll();
        List<Topic> GetTopicByChapterId(int chapterId);
        Topic GetFirstTopicByChapterId(int chapterId);
        IPagedList<Topic> GetPageList(int pageNo, int rowNo, string searchString);
        int GetCount();
        Topic GetDetails(int Id);
        bool Add(Topic model);
        bool Update(Topic model);
        bool Delete(int Id);
        List<DDLCommon> GetAllTopicsByChapterDDL(int chapterId);
    }

    public class TopicService : ITopicService
    {

        private readonly ApplicationDbContext _context;

        public TopicService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<Topic> GetAll()
        {
            return _context.Topics.ToList();
        }
        public List<Topic> GetTopicByChapterId(int chapterId)
        {
            return _context.Topics.Where(t=>t.ChapterId==chapterId).ToList();
        }

        public Topic GetFirstTopicByChapterId(int chapterId)
        {
            return _context.Topics.FirstOrDefault(t => t.ChapterId == chapterId);
        }
        public IPagedList<Topic> GetPageList(int pageNo, int rowNo, string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = _context.Topics.Count();
                var data = _context.Topics.OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Topic>(data.OrderBy(o => o.TopicName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.Topics.Where(a =>  a.TopicName.Contains(searchString)).Count();
                var data = _context.Topics.Where(a =>  a.TopicName.Contains(searchString)).OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Topic>(data.OrderBy(o => o.TopicName), pageNo, rowNo, totalRows);
            }
        }
        public int GetCount()
        {
            return _context.Topics.Count();
        }
        public Topic GetDetails(int Id)
        {
            return _context.Topics.Find(Id);
        }
        public bool Add(Topic model)
        {
            if (model != null)
            {
                try
                {
                    _context.Topics.Add(model);
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }

            }
            return false;
        }

        public bool Update(Topic model)
        {


            if (model != null)
            {
                try
                {                   
                    _context.Entry(model).State = EntityState.Modified;
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {

                    return false;
                }

            }
            return false;
        }

        public bool Delete(int Id)
        {
            try
            {
                Topic obj = _context.Topics.Find(Id);
                _context.Topics.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public List<DDLCommon> GetAllTopicsByChapterDDL(int chapterId)
        {
            var result = _context.Topics.Where(t => chapterId == 0 ? true : t.ChapterId == chapterId).Select(s => new DDLCommon
            {
                Id = s.Id,
                Name = s.TopicName
            }).ToList();

            return result;
        }

        #region Disposed
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion

    }
}