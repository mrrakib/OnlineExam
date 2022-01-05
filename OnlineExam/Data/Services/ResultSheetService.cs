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
    public interface IResultSheetService : IDisposable
    {
        List<ResultSheet> GetAll();
        int GetCount();
        ResultSheet GetDetails(int Id);
        bool Add(ResultSheet model);
        bool Add(List<ResultSheet> model);
        bool Update(ResultSheet model);
        bool Delete(int Id);
        int GetMaxExamCount(string userOrStudentId);
        ResultSummary GetResultStatus(string userOrStudentId, int totalExamCount, int subjectId = 0, int chapterId = 0, int topicId = 0);
        ResultSummary GetExamStatus(int examId, string userOrStudentId);
    }

    public class ResultSheetService : IResultSheetService
    {
        private readonly ApplicationDbContext _context;
        public ResultSheetService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<ResultSheet> GetAll()
        {
            return _context.ResultSheets.ToList();
        }

        public int GetCount()
        {
            return _context.ResultSheets.Count();
        }
        public ResultSheet GetDetails(int Id)
        {
            return _context.ResultSheets.Find(Id);
        }
        public bool Add(ResultSheet model)
        {
            if (model != null)
            {
                try
                {
                    _context.ResultSheets.Add(model);
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
        public bool Add(List<ResultSheet> model)
        {
            if (model != null)
            {
                try
                {
                    _context.ResultSheets.AddRange(model);
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
        public ResultSummary GetResultStatus(string userOrStudentId, int totalExamCount, int subjectId = 0, int chapterId = 0, int topicId = 0)
        {
            var sp = $"EXEC GetResultStatus @StudentId='{userOrStudentId}', @ExamCount={totalExamCount}, @SubjectId={subjectId}, @ChapterId={chapterId}, @TopicId={topicId}";
            var resultStatus = _context.Database.SqlQuery<ResultSummary>(sp).FirstOrDefault();
            return resultStatus;
        }
        public ResultSummary GetExamStatus(int examId,string userOrStudentId)
        {
            var sp = $"EXEC GetExamStatus @ExamId='{examId}', @StudentId='{userOrStudentId}'";
            var resultStatus = _context.Database.SqlQuery<ResultSummary>(sp).FirstOrDefault();
            return resultStatus;
        }
        public bool Update(ResultSheet model)
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
                ResultSheet obj = _context.ResultSheets.Find(Id);
                _context.ResultSheets.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetMaxExamCount(string userOrStudentId)
        {
            var maxExamCount = _context.ResultSheets.Where(a => a.StudentId == userOrStudentId && a.ExamCount > 0).OrderByDescending(a => a.Id).FirstOrDefault();
            if (maxExamCount != null && maxExamCount.ExamCount > 0)
                return maxExamCount.ExamCount + 1;
            return 1;
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