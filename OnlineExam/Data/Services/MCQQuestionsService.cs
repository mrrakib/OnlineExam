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
    public interface IMCQQuestionsService : IDisposable
    {
        List<MCQQuestion> GetAll();
        int GetCount();
        MCQQuestion GetDetails(int Id);
        bool Add(MCQQuestion model);
        bool Update(MCQQuestion model);
        bool Delete(int Id);
        MCQQuestion VMToQuestionModel(VMMCQQuestion vm);
        VMMCQQuestion ModelToQuestionVM(MCQQuestion vm);
        List<MCQQuestion> GetQuestions(int subjectId, int chapterId, int topicId, bool isMcq, bool isFullQuestionLoad = false, int totalNoOfQuestion = 10);
        IPagedList<MCQQuestion> GetPageList(int pageNo, int rowNo, string searchString, bool isMcq = false);
        IPagedList<MCQQuestion> GetQuestionsWithPageList(int page, int noOfRows, string searchString, int subjectId, int chapterId, int topicId, bool isMcq, bool isFullQuestionLoad = false, int totalNoOfQuestion = 10);
        List<DDLCommon> GetAllQuestionType();
        List<Exam> GetAllExamsByStudentId(string studentId);
        Exam GetExamDetails(int Id);
        List<MCQQuestion> GetQuestionsByExamId(int examId);
        bool AddMultiple(List<MCQQuestion> models);
        List<VMQuestionForModelTest> GetAllListTo();
    }

    public class MCQQuestionsService : IMCQQuestionsService
    {
        private readonly ApplicationDbContext _context;
        public MCQQuestionsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MCQQuestion> GetAll()
        {
            return _context.MCQQuestions.ToList();
        }
        public List<VMQuestionForModelTest> GetAllListTo()
        {
            List< VMQuestionForModelTest> result = _context.MCQQuestions.Select(q => new VMQuestionForModelTest
            {
                ChapterName = q.Chapter.ChapterName,
                IsIncluded = false,
                QuestionId = q.Id,
                QuestionName = q.QuestionName,
                QuestionTypeName = q.IsMCQ ? "MCQ" : "SBA",
                SubjectName = q.Subject.SubjectName
            }).ToList();
            return result;
        }
        public List<MCQQuestion> GetQuestions(int subjectId, int chapterId, int topicId, bool isMcq, bool isFullQuestionLoad = false, int totalNoOfQuestion = 10)
        {
            var questions = _context.MCQQuestions.AsQueryable();
            if (subjectId > 0)
                questions = questions.Where(a => a.SubjectId == subjectId);
            if (chapterId > 0)
                questions = questions.Where(a => a.ChapterId == chapterId);
            if (topicId > 0)
                questions = questions.Where(a => a.TopicId == topicId);
            questions = questions.Where(a => a.IsMCQ == isMcq);

            if (isFullQuestionLoad)
                return questions.OrderBy(x => Guid.NewGuid()).ToList();
            return questions.OrderBy(x => Guid.NewGuid()).Take(totalNoOfQuestion).ToList();
        }

        public IPagedList<MCQQuestion> GetPageList(int pageNo, int rowNo, string searchString, bool isMcq = false)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = _context.MCQQuestions.Where(a => a.IsMCQ == isMcq).Count();
                var data = _context.MCQQuestions.Where(a => a.IsMCQ == isMcq).OrderBy(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<MCQQuestion>(data.OrderBy(o => o.QuestionName), pageNo, rowNo, totalRows);
            }
            else
            {
                int totalRows = _context.MCQQuestions.Where(a => a.IsMCQ == isMcq && a.QuestionName.Contains(searchString)).Count();
                var data = _context.MCQQuestions.Where(a => a.IsMCQ == isMcq && a.QuestionName.Contains(searchString)).OrderBy(a => a.Id).Skip((pageNo - 1) * rowNo).Take(rowNo).ToList();
                return new StaticPagedList<MCQQuestion>(data.OrderBy(o => o.QuestionName), pageNo, rowNo, totalRows);
            }
        }
        public int GetCount()
        {
            return _context.MCQQuestions.Count();
        }
        public MCQQuestion GetDetails(int Id)
        {
            return _context.MCQQuestions.Find(Id);
        }
        public bool Add(MCQQuestion model)
        {
            if (model != null)
            {
                if (model.MCQQuestionOptions != null)
                {
                    model.MCQQuestionOptions = model.MCQQuestionOptions.Where(o => !string.IsNullOrWhiteSpace(o.OptionName)).ToList();
                }
                try
                {
                    _context.MCQQuestions.Add(model);
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

        public bool AddMultiple(List<MCQQuestion> models)
        {
            if (models.Count > 0)
            {
                try
                {
                    _context.MCQQuestions.AddRange(models);
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

        public bool Update(MCQQuestion model)
        {


            if (model != null)
            {
                List<MCQQuestionOption> List = _context.MCQQuestionOptions.AsNoTracking().Where(o => o.MCQQuestionId == model.Id).ToList();

                List<MCQQuestionOption> addList = new List<MCQQuestionOption>();
                List<MCQQuestionOption> delList = new List<MCQQuestionOption>();
                List<MCQQuestionOption> editList = new List<MCQQuestionOption>();

                if (model.MCQQuestionOptions != null)
                {
                    model.MCQQuestionOptions = model.MCQQuestionOptions.Where(o => !string.IsNullOrWhiteSpace(o.OptionName)).ToList();

                    addList = model.MCQQuestionOptions.Where(p => List.All(p2 => p2.Id != p.Id)).ToList();

                    delList = List.Where(p => model.MCQQuestionOptions.All(p2 => p2.Id != p.Id)).ToList();

                    editList = model.MCQQuestionOptions.Where(p => List.Any(p2 => p2.Id == p.Id)).ToList();


                }

                try
                {
                    if (addList.Count > 0)
                    {
                        addList.ForEach(m => m.MCQQuestionId = model.Id);
                        _context.MCQQuestionOptions.AddRange(addList);
                        _context.SaveChanges();

                    }
                    if (editList.Count > 0)
                    {
                        foreach (var item in editList)
                        {
                            _context.Entry(item).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                    }
                    if (delList.Count > 0)
                    {
                        foreach (var item in delList)
                        {
                            _context.Entry(item).State = EntityState.Deleted;
                            _context.SaveChanges();
                        }
                    }
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
                MCQQuestion obj = _context.MCQQuestions.Find(Id);
                _context.MCQQuestions.Remove(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public MCQQuestion VMToQuestionModel(VMMCQQuestion vm)
        {
            MCQQuestion question = new MCQQuestion();
            question.Id = vm.Id;
            question.QuestionName = vm.QuestionName;
            question.Explanation = vm.Explanation;
            question.Mark = vm.Mark;
            question.IsMCQ = vm.IsMCQ;
            question.SubjectId = vm.SubjectId;
            question.ChapterId = vm.ChapterId;
            question.TopicId = vm.TopicId;
            question.Subject = vm.Subject;
            question.Chapter = vm.Chapter;
            question.Topic = vm.Topic;

            List<MCQQuestionOption> optionList = new List<MCQQuestionOption>();
            MCQQuestionOption optionOne = new MCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 0 && vm.MCQQuestionOptions[0] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[0].OptionName))
            {
                optionOne.Id = vm.MCQQuestionOptions[0].Id;
                optionOne.MCQQuestionId = vm.MCQQuestionOptions[0].MCQQuestionId;
                optionOne.MCQQuestion = vm.MCQQuestionOptions[0].MCQQuestion;
                optionOne.OptionName = vm.MCQQuestionOptions[0].OptionName;
                optionOne.IsAnswer = vm.MCQQuestionOptions[0].IsAnswer;
            }

            MCQQuestionOption optionTwo = new MCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 1 && vm.MCQQuestionOptions[1] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[1].OptionName))
            {
                optionTwo.Id = vm.MCQQuestionOptions[1].Id;
                optionTwo.MCQQuestionId = vm.MCQQuestionOptions[1].MCQQuestionId;
                optionTwo.MCQQuestion = vm.MCQQuestionOptions[1].MCQQuestion;
                optionTwo.OptionName = vm.MCQQuestionOptions[1].OptionName;
                optionTwo.IsAnswer = vm.MCQQuestionOptions[1].IsAnswer;
            }
            MCQQuestionOption optionThree = new MCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 2 && vm.MCQQuestionOptions[2] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[2].OptionName))
            {
                optionThree.Id = vm.MCQQuestionOptions[2].Id;
                optionThree.MCQQuestionId = vm.MCQQuestionOptions[2].MCQQuestionId;
                optionThree.MCQQuestion = vm.MCQQuestionOptions[2].MCQQuestion;
                optionThree.OptionName = vm.MCQQuestionOptions[2].OptionName;
                optionThree.IsAnswer = vm.MCQQuestionOptions[2].IsAnswer;
            }
            MCQQuestionOption optionFour = new MCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 3 && vm.MCQQuestionOptions[3] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[3].OptionName))
            {
                optionFour.Id = vm.MCQQuestionOptions[3].Id;
                optionFour.MCQQuestionId = vm.MCQQuestionOptions[3].MCQQuestionId;
                optionFour.MCQQuestion = vm.MCQQuestionOptions[3].MCQQuestion;
                optionFour.OptionName = vm.MCQQuestionOptions[3].OptionName;
                optionFour.IsAnswer = vm.MCQQuestionOptions[3].IsAnswer;
            }
            MCQQuestionOption optionFive = new MCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 4 && vm.MCQQuestionOptions[4] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[4].OptionName))
            {
                optionFive.Id = vm.MCQQuestionOptions[4].Id;
                optionFive.MCQQuestionId = vm.MCQQuestionOptions[4].MCQQuestionId;
                optionFive.MCQQuestion = vm.MCQQuestionOptions[4].MCQQuestion;
                optionFive.OptionName = vm.MCQQuestionOptions[4].OptionName;
                optionFive.IsAnswer = vm.MCQQuestionOptions[4].IsAnswer;
            }
            optionList.Add(optionOne);
            optionList.Add(optionTwo);
            optionList.Add(optionThree);
            optionList.Add(optionFour);
            optionList.Add(optionFive);

            question.MCQQuestionOptions = optionList;

            return question;
        }
        public VMMCQQuestion ModelToQuestionVM(MCQQuestion vm)
        {
            VMMCQQuestion question = new VMMCQQuestion();
            question.Id = vm.Id;
            question.QuestionName = vm.QuestionName;
            question.Explanation = vm.Explanation;
            question.Mark = vm.Mark;
            question.IsMCQ = vm.IsMCQ;
            question.SubjectId = vm.SubjectId;
            question.ChapterId = vm.ChapterId;
            question.TopicId = vm.TopicId;
            question.Subject = vm.Subject;
            question.Chapter = vm.Chapter;
            question.Topic = vm.Topic;

            List<VMMCQQuestionOption> optionList = new List<VMMCQQuestionOption>();
            VMMCQQuestionOption optionOne = new VMMCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 0 && vm.MCQQuestionOptions[0] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[0].OptionName))
            {
                optionOne.Id = vm.MCQQuestionOptions[0].Id;
                optionOne.MCQQuestionId = vm.MCQQuestionOptions[0].MCQQuestionId;
                optionOne.MCQQuestion = vm.MCQQuestionOptions[0].MCQQuestion;
                optionOne.OptionName = vm.MCQQuestionOptions[0].OptionName;
                optionOne.IsAnswer = vm.MCQQuestionOptions[0].IsAnswer;
            }

            VMMCQQuestionOption optionTwo = new VMMCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 1 && vm.MCQQuestionOptions[1] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[1].OptionName))
            {
                optionTwo.Id = vm.MCQQuestionOptions[1].Id;
                optionTwo.MCQQuestionId = vm.MCQQuestionOptions[1].MCQQuestionId;
                optionTwo.MCQQuestion = vm.MCQQuestionOptions[1].MCQQuestion;
                optionTwo.OptionName = vm.MCQQuestionOptions[1].OptionName;
                optionTwo.IsAnswer = vm.MCQQuestionOptions[1].IsAnswer;
            }
            VMMCQQuestionOption optionThree = new VMMCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 2 && vm.MCQQuestionOptions[2] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[2].OptionName))
            {
                optionThree.Id = vm.MCQQuestionOptions[2].Id;
                optionThree.MCQQuestionId = vm.MCQQuestionOptions[2].MCQQuestionId;
                optionThree.MCQQuestion = vm.MCQQuestionOptions[2].MCQQuestion;
                optionThree.OptionName = vm.MCQQuestionOptions[2].OptionName;
                optionThree.IsAnswer = vm.MCQQuestionOptions[2].IsAnswer;
            }
            VMMCQQuestionOption optionFour = new VMMCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 3 && vm.MCQQuestionOptions[3] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[3].OptionName))
            {
                optionFour.Id = vm.MCQQuestionOptions[3].Id;
                optionFour.MCQQuestionId = vm.MCQQuestionOptions[3].MCQQuestionId;
                optionFour.MCQQuestion = vm.MCQQuestionOptions[3].MCQQuestion;
                optionFour.OptionName = vm.MCQQuestionOptions[3].OptionName;
                optionFour.IsAnswer = vm.MCQQuestionOptions[3].IsAnswer;
            }
            VMMCQQuestionOption optionFive = new VMMCQQuestionOption();
            if (vm.MCQQuestionOptions.Count > 4 && vm.MCQQuestionOptions[4] != null && !string.IsNullOrWhiteSpace(vm.MCQQuestionOptions[4].OptionName))
            {
                optionFive.Id = vm.MCQQuestionOptions[4].Id;
                optionFive.MCQQuestionId = vm.MCQQuestionOptions[4].MCQQuestionId;
                optionFive.MCQQuestion = vm.MCQQuestionOptions[4].MCQQuestion;
                optionFive.OptionName = vm.MCQQuestionOptions[4].OptionName;
                optionFive.IsAnswer = vm.MCQQuestionOptions[4].IsAnswer;
            }
            optionList.Add(optionOne);
            optionList.Add(optionTwo);
            optionList.Add(optionThree);
            optionList.Add(optionFour);
            optionList.Add(optionFive);

            question.MCQQuestionOptions = optionList;

            return question;
        }

        public IPagedList<MCQQuestion> GetQuestionsWithPageList(int page, int noOfRows, string searchString, int subjectId, int chapterId, int topicId, bool isMcq,bool isFullQuestionLoad=false, int totalNoOfQuestion=10)
        {
            var questions = _context.MCQQuestions.Where(a => a.IsMCQ == isMcq).AsQueryable();
            if (subjectId > 0)
                questions = questions.Where(a => a.SubjectId == subjectId);
            if (chapterId > 0)
                questions = questions.Where(a => a.ChapterId == chapterId);
            if (topicId > 0)
                questions = questions.Where(a => a.TopicId == topicId);

            if (!String.IsNullOrEmpty(searchString))
            {
                questions = questions.Where(a => a.QuestionName.Contains(searchString));
            }
            if (isFullQuestionLoad)
            {
                int totalRows = questions.Count();
                var data = questions.OrderBy(a => a.Id).Skip((page - 1) * noOfRows).Take(noOfRows).ToList();
                return new StaticPagedList<MCQQuestion>(data.OrderBy(x => x.QuestionName), page, noOfRows, totalRows);          
            }

            int rowsCount = questions.Take(totalNoOfQuestion).Count();
            var rows = questions.Take(totalNoOfQuestion).OrderBy(a => a.Id).Skip((page - 1) * noOfRows).Take(noOfRows).ToList();
            return new StaticPagedList<MCQQuestion>(rows.OrderBy(x => x.QuestionName), page, noOfRows, rowsCount);
        }
        public List<DDLCommon> GetAllQuestionType()
        {
            List<DDLCommon> result = new List<DDLCommon>();
            result.Add(new DDLCommon { Id = 1, Name = "MCQ" });
            result.Add(new DDLCommon { Id = 0, Name = "SBA" });
            return result;
        }

        public List<Exam> GetAllExamsByStudentId(string studentId)
        {
            return _context.Exams.ToList();
        }
        public Exam GetExamDetails(int Id)
        {
            return _context.Exams.FirstOrDefault(m=>m.Id==Id);
        }

        public List<MCQQuestion> GetQuestionsByExamId(int examId)
        {
            List<MCQQuestion> mCQQuestions = null;
            var exam=_context.Exams.FirstOrDefault(m => m.Id == examId);
            if (exam != null && exam.ExamWiseQuestions!=null && exam.ExamWiseQuestions.Count()>0)
            {
                var questionIds = exam.ExamWiseQuestions.Select(m => m.QuestionId).ToList();
                mCQQuestions = _context.MCQQuestions.Where(m => questionIds.Contains(m.Id)).ToList();
            }
            return mCQQuestions;
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