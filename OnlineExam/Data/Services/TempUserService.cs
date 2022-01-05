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
    public interface ITempUserService : IDisposable
    {
        List<TempUser> GetAll();
        TempUser GetByregisterInfo(RegisterViewModel model);
        int GetCount();
        TempUser GetDetails(int Id);
        bool Add(TempUser model);
        bool Update(TempUser model);
        bool Delete(int Id);
    }

    public class TempUserService : ITempUserService
    {

        private readonly ApplicationDbContext _context;

        public TempUserService(ApplicationDbContext context)
        {
            _context = context;
        }



        public List<TempUser> GetAll()
        {
            return _context.TempUsers.ToList();
        }
        public TempUser GetByregisterInfo(RegisterViewModel model)
        {
            return _context.TempUsers.Where(t=>t.UserName==model.UserName && t.MobileNo==model.MobileNo&& t.Email==model.Email).FirstOrDefault();
        }

        public int GetCount()
        {
            return _context.Chapters.Count();
        }
        public TempUser GetDetails(int Id)
        {
            return _context.TempUsers.Find(Id);
        }
        public bool Add(TempUser model)
        {
            if (model != null)
            {
                try
                {
                    _context.TempUsers.Add(model);
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

        public bool Update(TempUser model)
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
                TempUser obj = _context.TempUsers.Find(Id);
                _context.TempUsers.Remove(obj);
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