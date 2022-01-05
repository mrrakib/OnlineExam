using OnlineExam.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OnlineExam.Data.Services
{
    public interface IMCQOptionsService : IDisposable
    {
        List<MCQQuestionOption> GetAll();
        IPagedList<MCQQuestionOption> GetPageList(int pageNo, int rowNo, string searchString);
        int GetCount();
        MCQQuestionOption GetDetails(int Id);
        bool Add(MCQQuestionOption model);
        bool Update(MCQQuestionOption model);
        bool Delete(int Id);
    }

    public class MCQOptionsService : IMCQOptionsService
    {

        private readonly ApplicationDbContext _context;

        public MCQOptionsService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<MCQQuestionOption> GetAll()
        {
            return _context.MCQQuestionOptions.ToList();
        }
        public IPagedList<MCQQuestionOption> GetPageList(int pageNo, int rowNo, string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = _context.MCQQuestionOptions.Count();
                var data = _context.MCQQuestionOptions.OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<MCQQuestionOption>(data.OrderBy(o => o.OptionName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.MCQQuestionOptions.Where(a =>  a.OptionName.Contains(searchString)).Count();
                var data = _context.MCQQuestionOptions.Where(a =>  a.OptionName.Contains(searchString)).OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<MCQQuestionOption>(data.OrderBy(o => o.OptionName), pageNo, rowNo, totalRows);
            }
        }
        public int GetCount()
        {
            return _context.MCQQuestionOptions.Count();
        }
        public MCQQuestionOption GetDetails(int Id)
        {
            return _context.MCQQuestionOptions.Find(Id);
        }
        public bool Add(MCQQuestionOption model)
        {
            if (model != null)
            {
                try
                {
                    _context.MCQQuestionOptions.Add(model);
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

        public bool Update(MCQQuestionOption model)
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
                MCQQuestionOption obj = _context.MCQQuestionOptions.Find(Id);
                _context.MCQQuestionOptions.Remove(obj);
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