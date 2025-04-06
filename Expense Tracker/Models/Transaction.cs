using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Range(1,int.MaxValue,ErrorMessage ="Please Select a Category......")]
        public int categoryId { get; set; }
        public Category? category { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0...")]
        public int Amount { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [NotMapped]
        public string? CategoryTitleWithIcon { 
            get
            {
                return category == null ? " " : category.Icon + " " + category.Title;
            }
        }
        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((category == null || category.Type=="Expense") ? "- " : "+ " )+  Amount.ToString("C0");
            }
        }

    }
}
