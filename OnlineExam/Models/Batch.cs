using OnlineExam.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Batch
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Batch name is required")]
        public string BatchName { get; set; }

        [NotMapped]
        public List<TOSubject> Subjects { get; set; }
    }
}