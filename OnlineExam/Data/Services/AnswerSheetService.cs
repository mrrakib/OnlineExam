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
    public interface IAnswerSheetService
    {
        List<AnswerSheet> GetAll();
        int GetCount();
        AnswerSheet GetDetails(int Id);
        bool Add(AnswerSheet model);
        bool Add(List<AnswerSheet> model);
        bool Update(AnswerSheet model);
        bool Delete(int Id);
        List<MCQQuestion> GetExamHistory(string studentId = "", int examCount = 0);
        List<MCQQuestion> GetExamHistoryExamId(string studentId = "", int examId = 0);
    }

    public class AnswerSheetService : IAnswerSheetService
    {

        private readonly ApplicationDbContext _context;

        public AnswerSheetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AnswerSheet> GetAll()
        {
            return _context.AnswerSheets.ToList();
        }
        public int GetCount()
        {
            return _context.Chapters.Count();
        }
        public AnswerSheet GetDetails(int Id)
        {
            return _context.AnswerSheets.Find(Id);
        }
        public bool Add(AnswerSheet model)
        {
            if (model != null)
            {
                try
                {
                    _context.AnswerSheets.Add(model);
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
        public bool Add(List<AnswerSheet> model)
        {
            if (model != null)
            {
                try
                {
                    _context.AnswerSheets.AddRange(model);
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
        public bool Update(AnswerSheet model)
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
                AnswerSheet obj = _context.AnswerSheets.Find(Id);
                _context.AnswerSheets.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<MCQQuestion> GetExamHistoryExamId(string studentId = "", int examId = 0)
        {
            var data = new List<AnswerSheet>();
            if (string.IsNullOrWhiteSpace(studentId) || examId == 0)
                data = _context.AnswerSheets.ToList();
            else
                data = _context.AnswerSheets.Where(a => a.StudentId == studentId && a.ExamId == examId).ToList();

            var mappedData = data.Select(a => new MCQQuestion()
            {
                Id = a.Question.Id,
                TopicId = a.Question.TopicId,
                ChapterId = a.Question.ChapterId,
                Explanation = a.Question.Explanation,
                IsMCQ = a.Question.IsMCQ,
                Mark = a.Question.Mark,
                QuestionName = a.Question.QuestionName,
                SubjectId = a.Question.SubjectId,
                MCQQuestionOptions = a.Question.MCQQuestionOptions,
                TrueOptionId = a.TrueOptionId,
                FalseOptionId = a.FalseOptionId
            }).ToList();

            return mappedData;
        }
        public List<MCQQuestion> GetExamHistory(string studentId = "", int examCount = 0)
        {
            var data = new List<AnswerSheet>();
            if (string.IsNullOrWhiteSpace(studentId) || examCount == 0)
                data = _context.AnswerSheets.ToList();
            else
                data = _context.AnswerSheets.Where(a => a.StudentId == studentId && a.ExamCount == examCount).ToList();

            var mappedData = data.Select(a => new MCQQuestion()
            {
                Id = a.Question.Id, 
                TopicId = a.Question.TopicId, 
                ChapterId = a.Question.ChapterId, 
                Explanation = a.Question.Explanation, 
                IsMCQ = a.Question.IsMCQ, 
                Mark = a.Question.Mark,
                QuestionName = a.Question.QuestionName, 
                SubjectId = a.Question.SubjectId, 
                MCQQuestionOptions = a.Question.MCQQuestionOptions, 
                TrueOptionId = a.TrueOptionId, 
                FalseOptionId = a.FalseOptionId
            }).ToList();
          
            return mappedData;
        }
    }
}
