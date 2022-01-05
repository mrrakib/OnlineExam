﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SubjectName { get; set; }
    }
}