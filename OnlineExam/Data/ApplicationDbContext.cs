using Microsoft.AspNet.Identity.EntityFramework;
using OnlineExam.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OnlineExam.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Database.CreateIfNotExists();
        }
        public DbSet<TempUser> TempUsers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<MCQQuestion> MCQQuestions { get; set; }
        public DbSet<MCQQuestionOption> MCQQuestionOptions { get; set; }
        public DbSet<ResultSheet> ResultSheets { get; set; }
        public DbSet<AnswerSheet> AnswerSheets { get; set; }
        public DbSet<ResultSummary> ResultSummaries { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<BatchWiseSubject> BatchWiseSubjects { get; set; }
        public DbSet<StudentWiseBatch> StudentWiseBatches { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamWiseQuestion> ExamWiseQuestions { get; set; }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}