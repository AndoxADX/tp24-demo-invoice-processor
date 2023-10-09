namespace demo_invoice_processor.Models
{
    public class Receivable : BaseEntity
    {
        public Receivable(Guid id, ItemType type, string reference, 
            string currencyCode, decimal openingValue, 
            decimal paidValue, DateTime issueDate, Guid debtorId, 
            Guid companyId)
        {
            Id = id;
            Type = type;
            Reference = reference;
            CurrencyCode = currencyCode;
            OpeningValue = openingValue;
            PaidValue = 0;
            IssueDate = issueDate;
            DebtorId = debtorId;
            CompanyId = companyId;
            Status = ReceivableStatus.Pending;
            
        }

        public ItemType Type { get; set; }
        public string Reference { get; set; }
        public string CurrencyCode { get; set; }
        public decimal OpeningValue { get; set; }
        public decimal PaidValue { get; set; }
        public decimal ClosingValue => OpeningValue - PaidValue;
        public DateTime IssueDate { get; set; }
        //public DateTime DueDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool Cancelled { get; set; }
        public Guid DebtorId { get; set; }
        public Guid CompanyId { get; set; }
        public ReceivableStatus Status { get; set; }
        public string? PaymentId { get; set; }
        public string InvoiceId { get; set; }
    }

    /// <summary>
    /// Only for Invoice now
    /// </summary>
    public class InvoiceRating : BaseEntity
    {
        
        public Guid InvoiceId { get; set; }
        public string Reference { get; set; }
        public decimal AmountDue { get; set; }
        public decimal OfferAmount { get; set; }
        public decimal Rate { get; set; }
    }

    public class Invoice : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Guid DebtorId { get; set; }
        public string Reference { get; set; }
        public ItemType Type => ItemType.Invoice;
        public decimal AmountDue;
        public bool IsPaid { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool Cancelled { get; set; }

    }

    public class Payment : BaseEntity
    {
        public string CurrencyCode { get; set; }
        public string PaymentValue { get; set; }
        public string InvoiceRefId { get; set; }
    }
    public class AccountReceivables : BaseEntity
    {
        public decimal OpeningValue { get; set; }
        public decimal PaidValue { get; set; }
        public decimal ClosingValue { get; set; }
    }

    public enum ItemType
    {
        Invoice,
        CreditNote,
        Receivable
    }

    public enum ReceivableStatus
    {
        Pending,
        Paid,
        Overdue
    }
}