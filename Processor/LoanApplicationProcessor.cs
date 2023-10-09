using demo_invoice_processor.Data;
using demo_invoice_processor.Handlers;
using demo_invoice_processor.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using demo_invoice_processor.Controllers.Dto;

namespace demo_invoice_processor.Processor
{
    public interface ILoanApplicationProcessor
    {
        Task<LoanApplication> CreateLoanApplication(Guid companyId);
        //Task<LoanApplication> GetLoanApplication(Guid applicationId);

        Task<int> UpdateLoanApplicationStatus(Guid applicationId, LoanApplicationStatus status);
        Task<CompanyInvoiceSummary> UpdateCompanyCreditScore(Guid companyId);

    }
    public class LoanApplicationProcessor : ILoanApplicationProcessor
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILoanApplicationProcessor _loanApplicationProcessor;
        private readonly IFinanceRatingProcessor _financeRatingProcessor;
        private readonly ILoanApplicationHandler _loanApplicationHandler;

        public LoanApplicationProcessor(IApplicationDbContext context,
             IFinanceRatingProcessor financeRatingProcessor,
             ILoanApplicationProcessor loanApplicationProcessor,
             ILoanApplicationHandler loanApplicationHandler)
        {
            _dbContext = context;
            _financeRatingProcessor = financeRatingProcessor;
            _loanApplicationProcessor = loanApplicationProcessor;
            _loanApplicationHandler = loanApplicationHandler;
        }

        public async Task<LoanApplication> CreateLoanApplication(Guid companyId)
        {
            var application = await _loanApplicationHandler.CreateLoanApplication(companyId);

            // trigger financeProcessor
            await _financeRatingProcessor.ProcessFinancingForApplication(companyId);

            return application;
        }

        public async Task<int> UpdateLoanApplicationStatus(Guid applicationId, LoanApplicationStatus status)
        {
            var la = await _dbContext.LoanApplications.FindAsync(applicationId);
            la.Status = status;
            _dbContext.LoanApplications.Update(la);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<CompanyInvoiceSummary> UpdateCompanyCreditScore(Guid companyId)
        {
            var duplicateApplication = await _dbContext.LoanApplications.Where(x=>x.CompanyId == companyId && x.Created == DateTime.Today).FirstOrDefaultAsync() ?? await CreateLoanApplication(companyId);

            // Trigger Invoice Process 
            await _financeRatingProcessor.ProcessFinancingForApplication(companyId);
            // return summary with list of invoice rating

            return new CompanyInvoiceSummary();
        }

        //internal async Task<CompanyInvoiceSummary> ProcessLoanApplication(LoanApplication application)
        //{
            
        //}
    }
}
