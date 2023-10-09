using demo_invoice_processor.Data;
using demo_invoice_processor.Handlers;
using demo_invoice_processor.Models;

namespace demo_invoice_processor.Processor
{
    public interface IDebtorRiskAssessor
    {
        Task<DebtorRisk> AssessDebtorRisk(Guid companyId, Debtor debtor, IEnumerable<Invoice> unpaidInvoicesForCompany, decimal totalAmountDueForCompany);
    }
    public class DebtorRiskAssessor : IDebtorRiskAssessor
    {
        private readonly IReceivableHandler _receivableHandler;

        public DebtorRiskAssessor(
            IReceivableHandler receivableHandler)
        {
            _receivableHandler = receivableHandler;
        }
        public async Task<DebtorRisk> AssessDebtorRisk(
        Guid companyId,
        Debtor debtor,
        IEnumerable<Invoice> unpaidInvoicesForCompany,
        decimal totalAmountDueForCompany
    )
        {
            var debtorPaidInvoices = await _receivableHandler.GetPaidInvoicesForDebtorAsync(companyId, debtor.Id);
            if (debtorPaidInvoices.Count < 2)
            {
                // Discard debtors with fewer than 2 paid invoices (max risk)
                return new DebtorRisk
                {
                    DebtorId = debtor.Id,
                    Risk = 1m
                };
            }

            var debtorUnpaidInvoices = unpaidInvoicesForCompany.Where(y => y.DebtorId == debtor.Id).ToList();
            var totalAmountDueForDebtor = debtorUnpaidInvoices.Sum(y => y.AmountDue);
            var risk = totalAmountDueForDebtor / totalAmountDueForCompany;

            return new DebtorRisk
            {
                DebtorId = debtor.Id,
                Risk = risk
            };
        }
    }
}
