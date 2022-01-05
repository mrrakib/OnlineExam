using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    [Table("ResultSummaries")]
    public class ResultSummary
    {
        public ResultSummary()
        {
            QuestionHistory = new List<MCQQuestion>();
        }
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int ExamCount { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime ExamDate { get; set; }
        public decimal ActualMark { get; set; }
        public decimal ObtainMark { get; set; }
        public decimal ObtainMarkPercentage { get; set; }
        public decimal MaxObtainMarks { get; set; }
        public decimal MaxObtainMarksPercentage { get; set; }
        public decimal AvgObtainMarks { get; set; }
        public decimal AvgObtainMarksPercentage { get; set; }
        public int? ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [NotMapped]
        public List<MCQQuestion> QuestionHistory { get;  set; }
    }
}
