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
    public interface ISubjectsService : IDisposable
    {
        List<Subject> GetAll();
        List<TOSubject> GetAllListTo();
        IPagedList<Subject> GetPageList(int pageNo, int rowNo, string searchString);
        int GetCount();
        Subject GetDetails(int Id);
        bool Add(Subject model);
        bool Update(Subject model);
        bool Delete(int Id);
        List<DDLCommon> GetAllSubjectDDL();
    }

    public class SubjectsService : ISubjectsService
    {

        private readonly ApplicationDbContext _context;

        public SubjectsService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<Subject> GetAll()
        {
            return _context.Subjects.ToList();
        }
        public List<TOSubject> GetAllListTo()
        {
            return _context.Subjects.Select(s => new TOSubject
            {
                Id = s.Id,
                SubjectName = s.SubjectName
            }).ToList();
        }
        public IPagedList<Subject> GetPageList(int pageNo, int rowNo, string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = _context.Subjects.Count();
                var data = _context.Subjects.OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Subject>(data.OrderBy(o => o.SubjectName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.Subjects.Where(a =>  a.SubjectName.Contains(searchString)).Count();
                var data = _context.Subjects.Where(a =>  a.SubjectName.Contains(searchString)).OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Subject>(data.OrderBy(o => o.SubjectName), pageNo, rowNo, totalRows);
            }
        }
        public int GetCount()
        {
            return _context.Subjects.Count();
        }
        public Subject GetDetails(int Id)
        {
            return _context.Subjects.Find(Id);
        }
        public bool Add(Subject model)
        {
            if (model != null)
            {
                try
                {
                    _context.Subjects.Add(model);
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

        public bool Update(Subject model)
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
                Subject obj = _context.Subjects.Find(Id);
                _context.Subjects.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public List<DDLCommon> GetAllSubjectDDL()
        {
            var result = _context.Subjects.Select(s => new DDLCommon
            {
                Id = s.Id,
                Name = s.SubjectName
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