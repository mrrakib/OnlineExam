using OnlineExam.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(150)]
        public string ExamName { get; set; }
        public int BatchId { get; set; }
        [ForeignKey("BatchId")]
        public Batch Batch { get; set; }

        public virtual List<ExamWiseQuestion> ExamWiseQuestions { get; set; }
        [NotMapped]
        public List<VMQuestionForModelTest> QuestionForModelTests { get; set; }
    }
}