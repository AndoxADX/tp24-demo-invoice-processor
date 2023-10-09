using AutoMapper;
using demo_invoice_processor.Controllers.Dto;
using demo_invoice_processor.Data;
using demo_invoice_processor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace demo_invoice_processor.Handlers
{
    public interface ICompanyHandler
    {
        Task<Company> GetCompanyAsync(Guid companyId);
        Task<List<DebtorRisk>> GetCompanyDebtorRisksAsync(Guid companyId);
        Task<List<Debtor>> GetCompanyDebtorsAsync(Guid companyId);
        Task<int> UpdateCompanyCreditScore(Guid companyId, decimal creditScore);

    }

    public class CompanyHandler : ICompanyHandler
    {
        private static readonly JsonSerializerOptions _serializerOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CompanyHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            return await _dbContext.Companies.FindAsync(companyId);
        }

        public async Task<List<DebtorRisk>> GetCompanyDebtorRisksAsync(Guid companyId)
        {
            var applicationIds = await _dbContext.LoanApplications.Where(x=>x.CompanyId == companyId).Select(x=>x.Id).ToListAsync();
            return await _dbContext.DebtorRisks.Where(x => applicationIds.Contains(x.ApplicationId)).ToListAsync();
        }

        public async Task<List<Debtor>> GetCompanyDebtorsAsync(Guid companyId)
        {
            return await _dbContext.Debtors.Where(x => x.CompanyId == companyId).ToListAsync();
        }

        public async Task<int> UpdateCompanyCreditScore(Guid companyId, decimal creditScore)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
            if (company == null)
                throw new Exception("Company Not found!");
            company.CreditScore = creditScore;
            return await _dbContext.SaveChangesAsync();

        }
    }
}
