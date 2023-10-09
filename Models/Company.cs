namespace demo_invoice_processor.Models
{
    public class Company : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public decimal CreditScore { get; set; }
    }
}
