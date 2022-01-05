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
    public interface IExamWiseQuestionService : IDisposable
    {
        List<ExamWiseQuestion> GetAll();
        bool Add(ExamWiseQuestion model);
        bool AddMultiple(List<ExamWiseQuestion> models);
        bool Update(ExamWiseQuestion model);
        bool Delete(int Id);
        List<ExamWiseQuestion> GetAllByExamId(int examId);
    }

    public class ExamWiseQuestionService : IExamWiseQuestionService
    {

        private readonly ApplicationDbContext _context;

        public ExamWiseQuestionService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<ExamWiseQuestion> GetAll()
        {
            return _context.ExamWiseQuestions.ToList();
        }
        public List<ExamWiseQuestion> GetAllByExamId(int examId)
        {
            return _context.ExamWiseQuestions.Where(e => e.ExamId == examId).ToList();
        }
        public bool Add(ExamWiseQuestion model)
        {
            if (model != null)
            {
                try
                {
                    _context.ExamWiseQuestions.Add(model);
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

        public bool AddMultiple(List<ExamWiseQuestion> models)
        {
            if (models.Count > 0)
            {
                try
                {
                    _context.ExamWiseQuestions.AddRange(models);
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

        public bool Update(ExamWiseQuestion model)
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
                ExamWiseQuestion obj = _context.ExamWiseQuestions.Find(Id);
                _context.ExamWiseQuestions.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
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