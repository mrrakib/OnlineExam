using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        public int ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public virtual Chapter Chapter { get; set; }
        [Required]
        public string TopicName { get; set; }
    }
}