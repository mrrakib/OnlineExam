using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class StudentWiseBatch
    {
        [Key]
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string StudentId { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }

    }
}