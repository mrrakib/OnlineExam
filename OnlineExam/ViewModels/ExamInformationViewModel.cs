using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class ExamInformationViewModel
    {
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int SubjectId { get; set; }
        public int ChapterId { get; set; }
        public int TopicId { get; set; }
        [Required]
        public string QuestionType { get; set; }
        [DisplayName("Total Question")]
        public int TotalQuestion { get; set; }

        public bool IsFullQuestionLoad { get; set; }
    }
}