using OnlineExam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class VMMCQQuestion
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Question")]
        public string QuestionName { get; set; }
        public string Explanation { get; set; }
        public int Mark { get; set; }
        public bool IsMCQ { get; set; }
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }
        public int ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
        public int AnswerIndex { get; set; }
        public virtual List<VMMCQQuestionOption> MCQQuestionOptions { get; set; }
    }
}