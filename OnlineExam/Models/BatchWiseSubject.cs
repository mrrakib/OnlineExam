using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class BatchWiseSubject
    {
        [Key]
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int SubjectId { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }
}