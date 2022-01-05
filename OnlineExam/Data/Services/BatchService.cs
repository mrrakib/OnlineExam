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
    public interface IBatchService : IDisposable
    {
        List<Subject> GetAllSubjectByBatchId(int batchId);
        List<Batch> GetAllActiveBatchByStudentID(string studentID);
        List<Batch> GetAll();
        IPagedList<Batch> GetPageList(int pageNo, int rowNo, string searchString);
        int GetCount();
        Batch GetDetails(int Id);
        bool Add(Batch model);
        bool Update(Batch model);
        bool Delete(int Id);
        bool DeleteBulk(int Id);
        List<DDLCommon> GetAllSubjectDDL();
        List<DDLCommon> GetAllBatchDDL();
        bool SaveBulk(Batch model);
        bool UpdateBulk(Batch model);
    }

    public class BatchService : IBatchService
    {

        private readonly ApplicationDbContext _context;

        public BatchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool SaveBulk(Batch model)
        {
            if (model != null)
            {
                try
                {
                    _context.Batches.Add(model);

                    _context.SaveChanges();
                    List<BatchWiseSubject> batchWiseSubjects = new List<BatchWiseSubject>();
                    if (model.Subjects.Where(s => s.IsSelected).ToList().Count > 0)
                    {
                        foreach (var subject in model.Subjects.Where(s => s.IsSelected))
                        {
                            BatchWiseSubject batchWiseSubject = new BatchWiseSubject();
                            batchWiseSubject.BatchId = model.Id;
                            batchWiseSubject.SubjectId = subject.Id;
                            batchWiseSubjects.Add(batchWiseSubject);
                        }
                        _context.BatchWiseSubjects.AddRange(batchWiseSubjects);
                        _context.SaveChanges();
                    }

                    return true;
                }
                catch (Exception)
                {

                    return false;

                }


            }
            return false;
        }

        public List<Batch> GetAll()
        {
            return _context.Batches.ToList();
        }

        public List<Batch> GetAllActiveBatchByStudentID(string studentID)
        {
            return _context.StudentWiseBatches.Where(m => m.StudentId.TrimEnd().ToLower().Equals(studentID.TrimEnd().ToLower()) && m.IsActive).ToList().Select(b=> new Batch { Id=b.Batch.Id,BatchName=b.Batch.BatchName}).ToList();
            
        }
        public IPagedList<Batch> GetPageList(int pageNo, int rowNo, string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = _context.Batches.Count();
                var data = _context.Batches.OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Batch>(data.OrderBy(o => o.BatchName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.Batches.Where(a => a.BatchName.Contains(searchString)).Count();
                var data = _context.Batches.Where(a => a.BatchName.Contains(searchString)).OrderByDescending(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<Batch>(data.OrderBy(o => o.BatchName), pageNo, rowNo, totalRows);
            }
        }
        public int GetCount()
        {
            return _context.Batches.Count();
        }
        public Batch GetDetails(int Id)
        {
            return _context.Batches.Find(Id);
        }
        public bool Add(Batch model)
        {
            if (model != null)
            {
                try
                {
                    _context.Batches.Add(model);
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

        public bool Update(Batch model)
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

        public bool UpdateBulk(Batch model)
        {
            if (model != null)
            {
                using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        List<BatchWiseSubject> batchWiseSubjects = _context.BatchWiseSubjects.Where(b => b.BatchId == model.Id).ToList();
                        if (batchWiseSubjects.Count > 0)
                        {
                            _context.BatchWiseSubjects.RemoveRange(batchWiseSubjects);
                        }

                        _context.Entry(model).State = EntityState.Modified;

                        List<BatchWiseSubject> batchWiseSubjectList = new List<BatchWiseSubject>();
                        if (model.Subjects.Where(s => s.IsSelected).ToList().Count > 0)
                        {
                            foreach (var subject in model.Subjects.Where(s => s.IsSelected))
                            {
                                BatchWiseSubject batchWiseSubject = new BatchWiseSubject();
                                batchWiseSubject.BatchId = model.Id;
                                batchWiseSubject.SubjectId = subject.Id;
                                batchWiseSubjectList.Add(batchWiseSubject);
                            }
                            _context.BatchWiseSubjects.AddRange(batchWiseSubjectList);
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
                Batch obj = _context.Batches.Find(Id);
                _context.Batches.Remove(obj);
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
                    Batch obj = _context.Batches.Find(Id);
                    _context.Batches.Remove(obj);

                    List<BatchWiseSubject> batchWiseSubjects = _context.BatchWiseSubjects.Where(b => b.BatchId == obj.Id).ToList();
                    _context.BatchWiseSubjects.RemoveRange(batchWiseSubjects);

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
        public List<Subject> GetAllSubjectByBatchId(int batchId)
        {
            var subjects=_context.BatchWiseSubjects.Where(m => m.BatchId == batchId).ToList().Select(s=>new Subject { SubjectName=s.Subject.SubjectName,Id=s.Subject.Id}).ToList();
            return subjects;
        }
        public List<DDLCommon> GetAllSubjectDDL()
        {
            var result = _context.Batches.Select(s => new DDLCommon
            {
                Id = s.Id,
                Name = s.BatchName
            }).ToList();

            return result;
        }
        public List<DDLCommon> GetAllBatchDDL()
        {
            var result = _context.Batches.Select(s => new DDLCommon
            {
                Id = s.Id,
                Name = s.BatchName
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