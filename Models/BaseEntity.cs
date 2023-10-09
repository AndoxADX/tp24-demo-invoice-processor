namespace demo_invoice_processor.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime Created { get; set; } 

        public DateTime? LastModified { get; set; } 
        
        

    }
}