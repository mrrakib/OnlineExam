using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class MCQQuestion
    {
        public MCQQuestion()
        {
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Question")]
        public string QuestionName { get; set; }
        public string Explanation { get; set; }
        public int Mark { get; set; }
        public bool IsMCQ { get; set; }

        public int TopicId { get; set; }
        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }
        public int ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public virtual Chapter Chapter { get; set; }
        public int SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
        [NotMapped]
        public int AnswerIndex { get; set; }
        public virtual List<MCQQuestionOption> MCQQuestionOptions { get; set; }

        [NotMapped]
        public int CheckedOptionId { get; set; }
        [NotMapped]
        public string TrueOptionId { get;  set; }
        [NotMapped]
        public string FalseOptionId { get;  set; }

        [NotMapped]
        public string RadioOrCheckBoxGroupName
        {
            get { return "GroupName"+Id; }
        }

        [NotMapped]
        public int ExamId { get; set; }
    }
}