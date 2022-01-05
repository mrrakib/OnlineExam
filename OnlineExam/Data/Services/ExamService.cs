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
    public interface IExamService : IDisposable
    {
        List<Exam> GetAll();
        IPagedList<Exam> GetPageList(int pageNo, int rowNo, string searchString);
        int GetCount();
        Exam GetDetails(int Id);
        bool Add(Exam model);
        bool Update(Exam model);
        bool Delete(int Id);
        bool DeleteBulk(int Id);
        bool SaveBulk(Exam model);
        bool UpdateBulk(Exam model);
    }

    public class ExamService : IExamService
    {

        private readonly ApplicationDbContext _context;

        public ExamService(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool SaveBulk(Exam model)
        {
            if (model != null)
            {
                try
                {
                    _context.Exams.Add(model);

                    _context.SaveChanges();
                    List<ExamWiseQuestion> examWiseQuestions = new List<ExamWiseQuestion>();
                    if (model.QuestionForModelTests.Where(s => s.IsIncluded).ToList().Count > 0)
                    {
                        foreach (var question in model.QuestionForModelTests.Where(s => s.IsIncluded))
                        {
                            ExamWiseQuestion examWiseQuestion = new ExamWiseQuestion();
                            examWiseQuestion.QuestionId = question.QuestionId;
                            examWiseQuestion.ExamId = model.Id;
                            examWiseQuestions.Add(examWiseQuestion);
                        }
                        _context.ExamWiseQuestions.AddRange(examWiseQuestions);
                        _context.SaveChanges();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    string msg = ex.ToString();
                    return false;

                }


            }
            return false;
        }

        public List<Exam> GetAll()
        {
            return _context.Exams.ToList();
        }
        public IPagedList<Exam> GetPageList(int pageNo, int rowNo, string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = _context.Exams.Count();
                var data = _context.Exams.OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Exam>(data.OrderBy(o => o.ExamName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.Exams.Where(a => a.ExamName.Contains(searchString)).Count();
                var data = _context.Exams.Where(a => a.ExamName.Contains(searchString)).OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Exam>(data.OrderBy(o => o.ExamName), pageNo, rowNo, totalRows);
            }
        }
        public int GetCount()
        {
            return _context.Exams.Count();
        }
        public Exam GetDetails(int Id)
        {
            return _context.Exams.Find(Id);
        }
        public bool Add(Exam model)
        {
            if (model != null)
            {
                try
                {
                    _context.Exams.Add(model);
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

        public bool Update(Exam model)
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

        public bool UpdateBulk(Exam model)
        {
            if (model != null)
            {
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        List<ExamWiseQuestion> examWiseQuestions = _context.ExamWiseQuestions.Where(e => e.ExamId == model.Id).ToList();
                        if (examWiseQuestions.Count > 0)
                        {
                            _context.ExamWiseQuestions.RemoveRange(examWiseQuestions);
                        }

                        _context.Entry(model).State = EntityState.Modified;

                        List<ExamWiseQuestion> examWiseQuestionList = new List<ExamWiseQuestion>();
                        if (model.QuestionForModelTests.Where(s => s.IsIncluded).ToList().Count > 0)
                        {
                            foreach (var question in model.QuestionForModelTests.Where(s => s.IsIncluded))
                            {
                                ExamWiseQuestion examWiseQuestion = new ExamWiseQuestion();
                                examWiseQuestion.ExamId = model.Id;
                                examWiseQuestion.QuestionId = question.QuestionId;
                                examWiseQuestionList.Add(examWiseQuestion);
                            }
                            _context.ExamWiseQuestions.AddRange(examWiseQuestionList);
                        }

                        _context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }


            }
            return false;
        }

        public bool Delete(int Id)
        {
            try
            {
                Exam obj = _context.Exams.Find(Id);
                _context.Exams.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool DeleteBulk(int Id)
        {
            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Exam obj = _context.Exams.Find(Id);
                    _context.Exams.Remove(obj);

                    List<ExamWiseQuestion> examWiseQuestions = _context.ExamWiseQuestions.Where(b => b.ExamId == obj.Id).ToList();
                    _context.ExamWiseQuestions.RemoveRange(examWiseQuestions);

                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
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