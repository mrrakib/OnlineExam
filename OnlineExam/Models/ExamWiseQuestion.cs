using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class ExamWiseQuestion
    {
        [Key]
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }
        [ForeignKey("QuestionId")]
        public MCQQuestion MCQQuestion { get; set; }
    }
}