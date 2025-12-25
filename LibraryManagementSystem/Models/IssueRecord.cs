using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class IssueRecord
    {
        [Key]
        public int IssueRecordId { get; set; }

        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Issue Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime IssueDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Fine Amount (Rs)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FineAmount { get; set; } = 0;

        [Display(Name = "Fine Paid")]
        public bool IsFinePaid { get; set; } = false;

        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        [ForeignKey("MemberId")]
        public Member? Member { get; set; }
    }
}
