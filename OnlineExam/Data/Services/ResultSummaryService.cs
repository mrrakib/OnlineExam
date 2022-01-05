using Microsoft.Ajax.Utilities;
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
    public interface IResultSummaryService : IDisposable
    {
        List<ResultSummary> GetAll();
        ResultSummary GetDetails(int Id);
        ResultSummary GetDetailsByExamIdAndStudentId(int examId, string studentId);
        bool Add(ResultSummary model);
        bool Update(ResultSummary model);
        bool Delete(int Id);
        bool IsExamHeld(int examId);
        List<ResultSummary> GetResultSummary(string userOrStudentId);
        IPagedList<ResultSummary> GetPageList(int pageNo, int rowNo, string searchString, string studentId="");

    }

    public class ResultSummaryService : IResultSummaryService
    {
        private readonly ApplicationDbContext _context;
        public ResultSummaryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<ResultSummary> GetAll()
        {
            return _context.ResultSummaries.ToList();
        }
        public ResultSummary GetDetails(int Id)
        {
            return _context.ResultSummaries.Find(Id);
        }
        public bool IsExamHeld(int examId)
        {
            var result = _context.ResultSummaries.Where(r => r.ExamId != null && r.ExamId == examId).ToList();
            return result.Count > 0 ? true : false;
        }

        public ResultSummary GetDetailsByExamIdAndStudentId(int examId,string studentId)
        {
            return _context.ResultSummaries.FirstOrDefault(m=>m.ExamId==examId && m.StudentId==studentId);
        }
        public bool Add(ResultSummary model)
        {
            if (model != null)
            {
                try
                {
                    _context.ResultSummaries.Add(model);
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
        public bool Add(List<ResultSummary> model)
        {
            if (model != null)
            {
                try
                {
                    _context.ResultSummaries.AddRange(model);
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

        public bool Update(ResultSummary model)
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
                ResultSummary obj = _context.ResultSummaries.Find(Id);
                _context.ResultSummaries.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<ResultSummary> GetResultSummary(string userOrStudentId)
        {
            var history = _context.ResultSummaries.Where(a => userOrStudentId == null ? true : a.StudentId == userOrStudentId).OrderByDescending(a => a.Id).ToList();
            return history;
        }

        public IPagedList<ResultSummary> GetPageList(int pageNo, int rowNo, string searchString, string studentId="")
        {
            var data = _context.ResultSummaries.AsQueryable();
            if (!String.IsNullOrEmpty(studentId))
            {
                data = data.Where(a => a.StudentId == studentId);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                data = data.Where(a => a.StudentId == searchString);
            }

            int totalRows = data.Count();
            var finalData = data.OrderByDescending(a => a.ExamDate).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
            return new StaticPagedList<ResultSummary>(finalData.OrderByDescending(o => o.ExamDate), pageNo, rowNo, totalRows);
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