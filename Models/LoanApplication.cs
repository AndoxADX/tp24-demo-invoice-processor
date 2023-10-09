using System.Text.Json.Serialization;

namespace demo_invoice_processor.Models
{
    public class LoanApplication : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }

        public LoanApplicationStatus Status { get; set; }
       
        public List<InvoiceRating>? InvoiceRatings { get; set; }
    }
    public enum LoanApplicationStatus
    {
        Started,
        Processing,
        Error,
        Complete
    }
}
