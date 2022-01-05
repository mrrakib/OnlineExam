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
    public interface IBatchWiseSubjectService : IDisposable
    {
        List<BatchWiseSubject> GetAll();
        bool Add(BatchWiseSubject model);
        bool AddMultiple(List<BatchWiseSubject> models);
        bool Update(BatchWiseSubject model);
        bool Delete(int Id);
        List<BatchWiseSubject> GetAllByBatchId(int batchId);
    }

    public class BatchWiseSubjectService : IBatchWiseSubjectService
    {

        private readonly ApplicationDbContext _context;

        public BatchWiseSubjectService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<BatchWiseSubject> GetAll()
        {
            return _context.BatchWiseSubjects.ToList();
        }
        public List<BatchWiseSubject> GetAllByBatchId(int batchId)
        {
            return _context.BatchWiseSubjects.Where(b => b.BatchId == batchId).ToList();
        }
        public bool Add(BatchWiseSubject model)
        {
            if (model != null)
            {
                try
                {
                    _context.BatchWiseSubjects.Add(model);
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

        public bool AddMultiple(List<BatchWiseSubject> models)
        {
            if (models.Count > 0)
            {
                try
                {
                    _context.BatchWiseSubjects.AddRange(models);
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

        public bool Update(BatchWiseSubject model)
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
                BatchWiseSubject obj = _context.BatchWiseSubjects.Find(Id);
                _context.BatchWiseSubjects.Remove(obj);
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