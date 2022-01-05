using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class BulkQuestionCommon
    {
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public int ChapterId { get; set; }
        [Required]
        public int TopicId { get; set; }
        [Required]
        public string QuestionType { get; set; }
    }
}