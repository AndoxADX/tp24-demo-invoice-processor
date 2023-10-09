using demo_invoice_processor.Models;

namespace demo_invoice_processor.Processor
{
    public interface IInvoiceRatingAssesor
    {
        //InvoiceRating AssessInvoice(Invoice invoice);
        InvoiceRating AssessInvoice(Invoice invoice, decimal companyCreditHealthRate, bool overdue = false);
    }

    public class InvoiceRatingAssesor : IInvoiceRatingAssesor
    {
        public InvoiceRatingAssesor()
        {

        }
        //public InvoiceRating AssessInvoice(Invoice invoice)
        //{
        //    var terms = invoice.DueDate - invoice.IssueDate;
        //    var daysLeftToPay = invoice.DueDate - DateTime.Today;
        //    var ratio = daysLeftToPay / terms; // 10/100
        //    var rate = decimal.Round((decimal)(5 - 4 * ratio), 1);
        //    return new InvoiceRating
        //    {
        //        InvoiceId = invoice.Id,
        //        Reference = invoice.Reference,
        //        AmountDue = invoice.AmountDue,
        //        OfferAmount = invoice.AmountDue * 0.9m,
        //        Rate = rate
        //    };
        //}

        public InvoiceRating AssessInvoice(Invoice invoice, decimal companyCreditHealthRate, bool overdue = false)
        {
            var terms = invoice.DueDate - invoice.IssueDate;
            var daysLeftToPay = invoice.DueDate - DateTime.Today;
            var ratio = overdue ? GetOverdueRatio(invoice) : daysLeftToPay / terms; // 10/100
            var rate = decimal.Round((decimal)(5 - 4 * ratio), 1);
            return new InvoiceRating
            {
                InvoiceId = invoice.Id,
                Reference = invoice.Reference,
                AmountDue = invoice.AmountDue,
                OfferAmount = invoice.AmountDue * companyCreditHealthRate,
                Rate = rate
            };
        }

        private double GetOverdueRatio(Invoice invoice)
        {
            int[] riskCategory = { 30, 120 };
            var overdueDays = (DateTime.Today - invoice.DueDate).Days;
            if(overdueDays >= riskCategory[1]) return 0.9;
            if(overdueDays <= riskCategory[0]) return 0.5;
            return 0.7;
        }
    }
}