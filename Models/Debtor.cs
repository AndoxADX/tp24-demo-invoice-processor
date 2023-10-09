namespace demo_invoice_processor.Models
{
    public class Debtor : BaseEntity
    {
        public Debtor(Guid debtorId)
        {
            Id = debtorId;
        }
        public Guid Id { get; set; }
        public string DebtorName { get; set; }
        public string DebtorReference { get; set; }
        public string DebtorRegistrationNumber { get; set; }
        public string DebtorCountryCode { get; set; }
        public Address Address { get; set; }
        public Guid CompanyId { get; set; }

    }

    public class Address
    {
        public string DebtorAddress1 { get; set; }
        public string DebtorAddress2 { get; set; }
        public string DebtorTown { get; set; }
        public string DebtorState { get; set; }
        public string DebtorZip { get; set; }
    }

    public class DebtorRisk : BaseEntity
    {
        public Guid DebtorId { get; set; }
        public Guid ApplicationId { get; set; }
        public decimal Risk { get; set; }
        public Guid? LatestApplicationId { get; set; }
    }
}