using OnlineExam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class VMMCQQuestionOption
    {
        public VMMCQQuestionOption()
        {
            IsAnswer = false;
        }
        public int Id { get; set; }
        public int MCQQuestionId { get; set; }
        public virtual MCQQuestion MCQQuestion { get; set; }
        public string OptionName { get; set; }
        public bool IsAnswer { get; set; }
    }
}