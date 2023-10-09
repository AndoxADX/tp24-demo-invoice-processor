using demo_invoice_processor.Handlers;
using demo_invoice_processor.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using demo_invoice_processor.Data;
using Microsoft.EntityFrameworkCore;

namespace demo_invoice_processor.Processor
{
    public interface IFinanceRatingProcessor
    {
        Task<List<InvoiceRating>> ProcessFinancingForApplication(Guid loanApplicationId);

    }
    public class FinanceRatingProcessor : IFinanceRatingProcessor
    {
        private readonly IApplicationDbContext _context;
        private readonly ILoanApplicationHandler _applicationHandler;
        private readonly ICompanyHandler _companyHandler;
        private readonly IReceivableHandler _receivableHandler;
        private readonly IDebtorRiskAssessor _debtorRiskAccessor;
        private readonly IInvoiceRatingAssesor _invoiceRatingAssesor;
        private readonly CreditScoreRating _ratingParameters;
        public FinanceRatingProcessor(IApplicationDbContext context, ILoanApplicationHandler applicationHandler,
            IReceivableHandler receivableHandler, IDebtorRiskAssessor debtorRiskAccessor,
            CreditScoreRating ratingParameters, IInvoiceRatingAssesor invoiceRatingAssesor, 
            ICompanyHandler companyHandler)
        {
            _context = context;
            _applicationHandler = applicationHandler;
            _receivableHandler = receivableHandler;
            _debtorRiskAccessor = debtorRiskAccessor;
            _ratingParameters = ratingParameters;
            _invoiceRatingAssesor = invoiceRatingAssesor;
            _companyHandler = companyHandler;
        }

        public async Task<List<InvoiceRating>> ProcessFinancingForApplication(Guid loanApplicationId)
        {
            var application = await _applicationHandler.GetLoanApplicationAsync(loanApplicationId);
            await _applicationHandler.UpdateLoanApplicationStatus(loanApplicationId, LoanApplicationStatus.Processing);

            try
            {
                var invoiceRatings = await ProcessInvoicesAsync(application.CompanyId);
                //_applicationHandler.AddReceivableRatings(loanApplicationId, invoiceRatings);
                await _applicationHandler.UpdateLoanApplicationStatus(loanApplicationId, LoanApplicationStatus.Complete);
                await _context.SaveChangesAsync();

                return invoiceRatings;
            }
            catch (Exception)
            {
                await _applicationHandler.UpdateLoanApplicationStatus(loanApplicationId, LoanApplicationStatus.Error);
                throw;
            }
        }

        private async Task<List<InvoiceRating>> ProcessInvoicesAsync(Guid companyId)
        {
            // Get all latest unpaid invoices for the company
            var unpaidInvoices = await _receivableHandler.GetUnpaidDistinctInvoicesForCompanyAsync(companyId);
            var totalAmountDueForCompany = unpaidInvoices.Sum(x => x.AmountDue);

            // Add Overdue client. Remove it from unpaid invoices debtor risk id.
            var overdueClients = unpaidInvoices.Where(x => x.DueDate.Date <= DateTime.UtcNow.Date)
                .Select(x=>x.DebtorId)?.Distinct();
            // Get debtors by id from the unpaid invoices and not overdue
            var debtorIds = unpaidInvoices.Select(x => x.DebtorId).Distinct().ToList();
            debtorIds.Except(overdueClients);
            var debtors = await _context.Debtors.Where(x=>x.CompanyId == companyId && debtorIds.Contains(x.Id)).ToListAsync();

            // Assess risk for each debtor, discard those above concentration threshold
            var debtorRiskTasks = debtors.Select(x => _debtorRiskAccessor.AssessDebtorRisk(companyId, x, unpaidInvoices, totalAmountDueForCompany));
            var debtorRisks = await Task.WhenAll(debtorRiskTasks);
            var lowRiskDebtorIds = debtorRisks.Where(x => x.Risk < _ratingParameters.RiskConcentrationThreshold)
                .Select(x => x.DebtorId);
            var highRiskDebtorIds = overdueClients;
            var companyCredit = await GetCompCreditByReceivableTurnoverRatio(companyId);
            await _companyHandler.UpdateCompanyCreditScore(companyId, companyCredit);
            // Calculate decisions per unpaid invoice for debtors with low risk
            var lowRiskInvoiceRatings = unpaidInvoices.Where(x => lowRiskDebtorIds.Contains(x.DebtorId))
                .Where(
                    x =>
                    {
                        // Discard invoices with fewer than 14 days left to pay
                        var daysLeftToPay = x.DueDate - DateTime.Today;
                        return daysLeftToPay >= TimeSpan.FromDays(14);
                    }
                )
                .Select(_=> _invoiceRatingAssesor.AssessInvoice(_,companyCredit))
                .ToList();
            var highRiskInvoiceRating = unpaidInvoices.Where(x => overdueClients.Contains(x.DebtorId))
                .Select(_ => _invoiceRatingAssesor.AssessInvoice(_,companyCredit))
                .ToList();


            return lowRiskInvoiceRatings;
        }

        

        public async Task<decimal> GetCompCreditByReceivableTurnoverRatio(Guid companyId)
        {
            // Get company average turnover ratio:
            // total sales for the period ÷ ((receivables at the beginning of the period + receivables at the end of the period) ÷ 2)
            var lastQReceivables = await _receivableHandler.GetPreviousQuarterReceivablesForCompany(companyId);
            var lastClosingValues = lastQReceivables.Sum(x => x.ClosingValue);

            var currentQReceivables = await _receivableHandler.GetCurrentQuarterReceivablesForCompany(companyId);
            var currentClosingValues = currentQReceivables.Sum(x => x.ClosingValue);

            var quarterInvoices = await _receivableHandler.GetQuarterInvoicesForCompanyAsync(companyId);
            var totalSales = quarterInvoices.Sum(x => x.AmountDue);
            var companyTurnoverRatio = totalSales/ ((lastClosingValues+ currentClosingValues)/2);
            var companyCredit = 1 / companyTurnoverRatio;
            return companyCredit;
        }

        // Discarded
        ////  TotalSales = Sum(invoice.OpeningValue)
        ////  Average receivables by debtors = (sum(invoice.PaidValue)/count(invoice.count)) 
        //// turnover rate per debtor is used as credit score per debtor.
        //private async Task<Dictionary<Guid, decimal>> GetDebtorTurnover(List<Invoice> invoices)
        //{
        //    var debtorsTurnover = invoices.GroupBy(x => x.DebtorId).ToDictionary(x => x.Key, x =>
        //    {
        //        return x.Sum(y => y.OpeningValue) / (x.Sum(y => y.PaidValue) / x.Count());
        //    });
        //    return debtorsTurnover;
        //}

        /// Discarded.
        //private async Task<decimal> GetCompanyCreditHealthRate(Guid companyId)
        //{
        //    // get ratio of unpaid invoices, if its <50%. 0.9. if >50%, 0.6m
        //    var unpaidInvoicesRatio = 
        //    // if any overdue, 
        //    decimal[] companyCreditHealthRateRange = { 0.5m ,0.7m, 0.8m, 0.9m };
        //    decimal companyCreditHealthRate = 
        //    return companyCreditHealthRate;
        //}
    }
}
