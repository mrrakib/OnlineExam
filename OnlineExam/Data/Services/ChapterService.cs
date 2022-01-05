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
    public interface IChapterService : IDisposable
    {
        List<Chapter> GetAll();
        List<Chapter> GetChapterBySubjectId(int subjectId);
        IPagedList<Chapter> GetPageList(int pageNo, int rowNo, string searchString);
        int GetCount();
        Chapter GetDetails(int Id);
        bool Add(Chapter model);
        bool Update(Chapter model);
        bool Delete(int Id);
        List<DDLCommon> GetAllChpaterBySubjectDDL(int subjectId);
    }

    public class ChapterService : IChapterService
    {

        private readonly ApplicationDbContext _context;

        public ChapterService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<Chapter> GetAll()
        {
            return _context.Chapters.ToList();
        }
        public List<Chapter> GetChapterBySubjectId(int subjectId)
        {
            return _context.Chapters.Where(c=>c.SubjectId== subjectId).ToList();
        }
        public IPagedList<Chapter> GetPageList(int pageNo, int rowNo, string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = _context.Chapters.Count();
                var data = _context.Chapters.OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Chapter>(data.OrderBy(o => o.ChapterName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.Chapters.Where(a =>  a.ChapterName.Contains(searchString)).Count();
                var data = _context.Chapters.Where(a =>  a.ChapterName.Contains(searchString)).OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Chapter>(data.OrderBy(o => o.ChapterName), pageNo, rowNo, totalRows);
            }
        }
        public int GetCount()
        {
            return _context.Chapters.Count();
        }
        public Chapter GetDetails(int Id)
        {
            return _context.Chapters.Find(Id);
        }
        public bool Add(Chapter model)
        {
            if (model != null)
            {
                try
                {
                    _context.Chapters.Add(model);
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

        public bool Update(Chapter model)
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
                Chapter obj = _context.Chapters.Find(Id);
                _context.Chapters.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public List<DDLCommon> GetAllChpaterBySubjectDDL(int subjectId)
        {
            var result = _context.Chapters.Where(c => subjectId == 0 ? true : c.SubjectId == subjectId).Select(s => new DDLCommon
            {
                Id = s.Id,
                Name = s.ChapterName
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