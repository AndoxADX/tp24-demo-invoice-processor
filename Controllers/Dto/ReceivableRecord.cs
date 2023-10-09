using demo_invoice_processor.Data;
using demo_invoice_processor.Models;
using System.ComponentModel.DataAnnotations;

namespace demo_invoice_processor.Controllers.Dto
{
    public record ReceivableRecord
    {
        public ItemType Type { get; init; }
        [Required]
        public string Reference { get; init; }
        [Required]
        public string CurrencyCode { get; init; }
        [Required]
        public DateTime IssueDate { get; init; }
        [Required]
        public decimal OpeningValue { get; init; }
        [Required]
        public decimal PaidValue { get; init; }
        [Required]
        public DateTime DueDate { get; init; }
        public DateTime? ClosedDate { get; init; }
        public bool Cancelled { get; init; }
        [Required]
        public string DebtorName { get; init; }
        public string DebtorReference { get; init; }
        public string DebtorRegistrationNumber { get; init; }
        [Required]
        public string DebtorCountryCode { get; init; }
        public string DebtorAddress1 { get; init; }
        public string DebtorAddress2 { get; init; }
        public string DebtorTown { get; init; }
        public string DebtorState { get; init; }
        public string DebtorZip { get; init; }
    }
}
