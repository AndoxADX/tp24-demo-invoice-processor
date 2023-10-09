using AutoMapper;
using demo_invoice_processor.Controllers.Dto;
using demo_invoice_processor.Data;
using demo_invoice_processor.Models;
using demo_invoice_processor.Processor;
using Microsoft.EntityFrameworkCore;

namespace demo_invoice_processor.Handlers
{
    public interface ILoanApplicationHandler
    {
        Task<LoanApplication> CreateLoanApplication( Guid companyId);
        Task<LoanApplication> GetLoanApplicationAsync(Guid applicationId);
        Task<CompanyInvoiceSummary> GetCompanyCreditScore(Guid companyId);
        Task<int> UpdateLoanApplicationStatus(Guid applicationId, LoanApplicationStatus status);
    }

    public class LoanApplicationHandler : ILoanApplicationHandler
    {
        private readonly ILoanApplicationProcessor _applicationProcessor;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public LoanApplicationHandler(ILoanApplicationProcessor applicationProcessor,
            IApplicationDbContext dbContext,
            IMapper mapper)
        {
            _applicationProcessor = applicationProcessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<LoanApplication> CreateLoanApplication( Guid companyId)
        {
            var result = await _dbContext.LoanApplications.AddAsync(new LoanApplication
            {
                CompanyId = companyId,
                Id = Guid.NewGuid(),
                Status = LoanApplicationStatus.Started
            });
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<int> UpdateLoanApplicationStatus(Guid applicationId, LoanApplicationStatus status)
        {
            var application = await _dbContext.LoanApplications.FindAsync(applicationId);
            application.Status = status;
            return await _dbContext.SaveChangesAsync();
        }
        public async Task<LoanApplication?> GetLoanApplicationAsync(Guid applicationId)
        {
            return await _dbContext.LoanApplications.FindAsync(applicationId);
        }

        public async Task<CompanyInvoiceSummary> GetCompanyCreditScore(Guid companyId)
        {
            // Get client company
            var company = await _dbContext.Companies.FindAsync(companyId);
            // Create application
            var loanApplication = await _dbContext.LoanApplications.Where(x=>x.CompanyId == companyId).LastOrDefaultAsync();
            // return summary with list of invoice rating
            return new CompanyInvoiceSummary()
            {
                CompanyId = loanApplication.CompanyId,
                CreditScore = company.CreditScore
            };
        }

       

        
    }
}
