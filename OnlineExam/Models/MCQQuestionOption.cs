using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExam.Models
{
    public class MCQQuestionOption
    {
        public MCQQuestionOption()
        {
            IsAnswer = false;
        }

        [Key]
        public int Id { get; set; }
        public int MCQQuestionId { get; set; }
        [ForeignKey("MCQQuestionId")]
        public virtual MCQQuestion MCQQuestion { get; set; }
        [Required]
        public string OptionName { get; set; }
        public bool IsAnswer { get; set; }

        [NotMapped]
        public bool? CheckedOption { get; set; }
        [NotMapped]
        public int CheckedOptionId { get; set; }

        [NotMapped]
        public string RadioOrCheckBoxQuestionOptionId
        {
            get { return "Q" + MCQQuestionId + "O" + Id; }
        }
        [NotMapped]
        public string RadioOrCheckBoxGroupName
        {
            get { return "GroupName" + Id; }
        }
    }
}
