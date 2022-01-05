using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class VMQuestionForModelTest
    {
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public string SubjectName { get; set; }
        public string ChapterName { get; set; }
        public string QuestionTypeName { get; set; }
        public bool IsIncluded { get; set; }
    }
}