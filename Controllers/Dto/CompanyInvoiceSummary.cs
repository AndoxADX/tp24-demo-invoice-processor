namespace demo_invoice_processor.Controllers.Dto
{
    public record CompanyInvoiceSummary
    {
        public Guid CompanyId { get; init; }
        public decimal CreditScore { get; init; } // Rate: Average of Total rate from unpaid * unpaid
    }
}