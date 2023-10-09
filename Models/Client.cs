//namespace demo_invoice_processor.Models
////{
//    public class Client : BaseEntity
//    {
//        public Guid Id { get; set; }
//        public string RegistrationNumber { get; set; }
//        public List<Address> Addresses { get; set; }

//        public bool IsUnitedKingdomClient()
//        {
//            return Addresses.All(x => x.CountryCode == "44");
//        }
//    }

//    public class Address
//    {
//        public string CountryCode { get; set; }
//    }

//    public class ClientRisk : BaseEntity
//    {
//        public Guid ClientId { get; set; }
//        public decimal Risk { get; set; }
//    }

//}
