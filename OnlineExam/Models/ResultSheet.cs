using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExam.Models
{
    [Table("ResultSheets")]
    public class ResultSheet
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int ExamCount { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual MCQQuestion Question { get; set; }

        public double ActualMark { get; set; }

        public double ObtainMark { get; set; }

        public int CorrectOptionCount { get; set; }

        public bool IsMCQ { get; set; }

        public DateTime ExamDateTime { get; set; }
        public int? ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
    }
}
