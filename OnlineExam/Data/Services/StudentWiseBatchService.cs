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
    public interface IStudentWiseBatchService : IDisposable
    {
        List<StudentWiseBatch> GetAll();
        List<StudentWiseBatch> GetAllByStudentId(string studentId);
        IPagedList<VMStudentWiseBatch> GetPageList(string studentId, int pageNo, int rowNo);
        IPagedList<VMStudentWiseBatch> GetPageListWithStudentInfo(string studentId, int pageNo, int rowNo);
        StudentWiseBatch GetDetails(int Id);
        bool Add(StudentWiseBatch model);
        bool Update(StudentWiseBatch model);
        bool IsExists(int batchId, string studentId);
        bool IsExistsUpdate(int batchId, string studentId, int id);
        bool Delete(int Id);
    }

    public class StudentWiseBatchService : IStudentWiseBatchService
    {

        private readonly ApplicationDbContext _context;

        public StudentWiseBatchService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<StudentWiseBatch> GetAll()
        {
            return _context.StudentWiseBatches.ToList();
        }
        public List<StudentWiseBatch> GetAllByStudentId(string studentId)
        {
            return _context.StudentWiseBatches.Where(s => s.StudentId.TrimEnd().ToLower().Equals(studentId.TrimEnd().ToLower())).ToList();
        }
        public IPagedList<VMStudentWiseBatch> GetPageList(string studentId,int pageNo, int rowNo)
        {
            if (!string.IsNullOrEmpty(studentId))
            {
                int totalRows = _context.StudentWiseBatches.Where(a => a.StudentId.TrimEnd().ToLower().Equals(studentId.TrimEnd().ToLower())).Count();
                var data = _context.StudentWiseBatches.Where(a => a.StudentId.TrimEnd().ToLower().Equals(studentId.TrimEnd().ToLower())).OrderByDescending(a => a.Id).Select(s => new VMStudentWiseBatch { BatchName = s.Batch != null ? s.Batch.BatchName : string.Empty, Id = s.Id, IsActive = s.IsActive })
                    .Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<VMStudentWiseBatch>(data.OrderBy(o => o.BatchName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.StudentWiseBatches.Count(); ;
                List<VMStudentWiseBatch> data = _context.StudentWiseBatches.OrderByDescending(a => a.Id).Select(s => new VMStudentWiseBatch { BatchName = s.Batch != null ? s.Batch.BatchName : string.Empty, Id = s.Id, IsActive = s.IsActive })
                    .Skip((pageNo - 1) * rowNo).Take(rowNo).ToList(); ;
                return new StaticPagedList<VMStudentWiseBatch>(data, pageNo, rowNo, totalRows);
            }
        }

        public IPagedList<VMStudentWiseBatch> GetPageListWithStudentInfo(string studentId, int pageNo, int rowNo)
        {
            if (!string.IsNullOrEmpty(studentId))
            {
                int totalRows = _context.StudentWiseBatches.Where(a => a.StudentId.TrimEnd().ToLower().Equals(studentId.TrimEnd().ToLower())).Count();               
                var data = (from stb in _context.StudentWiseBatches
                             join st in _context.Users on stb.StudentId equals st.StudentId
                             where stb.StudentId.ToLower().Equals(studentId.TrimEnd().ToLower())
                             select new VMStudentWiseBatch
                             {
                                 BatchName = stb.Batch != null ? stb.Batch.BatchName : "",
                                 Id = stb.Id,
                                 StudentID = stb.StudentId,
                                 StudentName = st.FullName,
                                 MobileNo = st.UserName,
                                 Email = st.Email,
                                 IsActive=stb.IsActive
                             }).ToList().Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<VMStudentWiseBatch>(data.OrderBy(o => o.BatchName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.StudentWiseBatches.Count();
                var data = (from stb in _context.StudentWiseBatches
                            join st in _context.Users on stb.StudentId equals st.StudentId     
                            select new VMStudentWiseBatch
                            {
                                BatchName = stb.Batch != null ? stb.Batch.BatchName : "",
                                Id = stb.Id,
                                StudentID = stb.StudentId,
                                StudentName = st.FullName,
                                MobileNo = st.UserName,
                                Email = st.Email,
                                IsActive = stb.IsActive
                            }).ToList().Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<VMStudentWiseBatch>(data.OrderBy(o => o.BatchName), pageNo, rowNo, totalRows);
            }
        }
        public StudentWiseBatch GetDetails(int Id)
        {
            return _context.StudentWiseBatches.Find(Id);
        }
        public bool Add(StudentWiseBatch model)
        {
            if (model != null)
            {
                try
                {
                    _context.StudentWiseBatches.Add(model);
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

        public bool Update(StudentWiseBatch model)
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
                StudentWiseBatch obj = _context.StudentWiseBatches.Find(Id);
                _context.StudentWiseBatches.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool IsExists(int batchId, string studentId)
        {
            var result = _context.StudentWiseBatches.Where(s => s.BatchId == batchId && s.StudentId.TrimEnd().ToLower().Equals(studentId.TrimEnd().ToLower())).ToList();
            return result.Count > 0 ? true : false;
        }

        public bool IsExistsUpdate(int batchId, string studentId, int id)
        {
            var result = _context.StudentWiseBatches.Where(s => s.BatchId == batchId && s.StudentId.TrimEnd().ToLower().Equals(studentId.TrimEnd().ToLower()) && s.Id != id).ToList();
            return result.Count > 0 ? true : false;
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