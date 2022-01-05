using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExam.Models
{
    [Table("AnswerSheets")]
    public class AnswerSheet
    {
        public AnswerSheet()
        {
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int ExamCount { get; set; }
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual MCQQuestion Question { get; set; }
        public bool IsMCQ { get; set; }

        public string TrueOptionId { get; set; }
        public string FalseOptionId { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
    }
}