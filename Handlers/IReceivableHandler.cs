using AutoMapper;
using demo_invoice_processor.Controllers.Dto;
using demo_invoice_processor.Data;
using demo_invoice_processor.Models;
using demo_invoice_processor.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace demo_invoice_processor.Handlers
{
    public interface IReceivableHandler
    {
        //Task ProcessReceivableAsync();
        Task<Receivable> GetReceivableAsync(Guid receivableId);
        Task<int> UploadReceivableAsync(ReceivableRecord receivableRecord);
        Task<List<Invoice>> GetPaidDistinctInvoicesForCompanyAsync(Guid companyId);
        Task<List<Invoice>> GetUnpaidDistinctInvoicesForCompanyAsync(Guid companyId);
        Task<List<Invoice>> GetPaidInvoicesForDebtorAsync(Guid companyId, Guid debtorId);
        Task<List<Invoice>> GetQuarterInvoicesForCompanyAsync(Guid companyId);
        Task<List<Receivable>> GetPreviousQuarterReceivablesForCompany(Guid companyId);
        Task<List<Receivable>> GetCurrentQuarterReceivablesForCompany(Guid companyId);
        //Task<List<Invoice>> GetPaidInvoicesForCompanyAsync(Guid companyId);
        //Task<List<Invoice>> GetUnpaidInvoicesForCompanyAsync(Guid companyId);
    }

    public class ReceivableHandler : IReceivableHandler
    {
        private static readonly JsonSerializerOptions _serializerOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public ReceivableHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<Invoice>> GetPaidInvoicesForDebtorAsync(Guid companyId, Guid debtorId) 
        {
            var invoices =  await _dbContext.Invoices.Where(x => x.CompanyId == companyId && x.DebtorId == debtorId).ToListAsync(); 
            return invoices.FindAll(x=>x.IsPaid);
        }

        public async Task<Receivable> GetReceivableAsync(Guid receivableId)
        {
            return await _dbContext.Receivables.FindAsync(receivableId)
                ?? throw new ApplicationException($"{nameof(Receivable)} with Id {receivableId} not found.");
        }

        //public async Task ProcessReceivableAsync()
        //{
        //    //var codeBook = JsonSerializer.Deserialize<Book>(bookJson, _serializerOption);
        //    throw new NotImplementedException();
        //}

        public async Task<int> UploadReceivableAsync(ReceivableRecord receivableRecord)
        {
            var record = _mapper.Map<Receivable>(receivableRecord);
            await _dbContext.Receivables.AddAsync(record);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Invoice>> GetPaidDistinctInvoicesForCompanyAsync(Guid companyId) 
        {
            var a = await _dbContext.Invoices.Where(x => x.CompanyId == companyId).DistinctBy(x => x.Reference).ToListAsync();
            var invoices = await _dbContext.Invoices.Where(x => x.CompanyId == companyId).GroupBy(x=>x.Reference).Select(chunk => chunk.OrderByDescending(i=>i.Created).First()).ToListAsync();
            return invoices.FindAll(x => x.IsPaid && !x.Cancelled);
        }
        public async Task<List<Invoice>> GetUnpaidDistinctInvoicesForCompanyAsync(Guid companyId)
        {
            var invoices = await _dbContext.Invoices.Where(x => x.CompanyId == companyId)
                .GroupBy(x => x.Reference)
                .Select(chunk => chunk.OrderByDescending(i => i.Created).First())
                .ToListAsync();
            // discard overdue
            return invoices.FindAll(x => !x.IsPaid && !x.Cancelled);
        }
        public async Task<List<Invoice>> GetQuarterInvoicesForCompanyAsync(Guid companyId)
        {
            var currentQuarter = DateTime.UtcNow.GetQuarter();
            var invoices = await _dbContext.Invoices
                .Where(x => x.CompanyId == companyId 
                && x.IssueDate.GetQuarter() == currentQuarter
                && !x.Cancelled)
                .ToListAsync();
            return invoices;
        }

        public async Task<Dictionary<Guid, decimal>> GetDebtorsTurnoverForCompanyAsync(Guid companyId)
        {
            var invoices = await _dbContext.Receivables.Where(x => x.CompanyId == companyId).ToListAsync();
            var debtorsTurnover = invoices.GroupBy(x => x.DebtorId).ToDictionary(x => x.Key, x =>
            {
                return x.Sum(y => y.OpeningValue) / (x.Sum(y => y.PaidValue) / x.Count());
            });
            return debtorsTurnover;
        }

        public async Task<List<Receivable>> GetPreviousQuarterReceivablesForCompany(Guid companyId)
        {
            var currentQuarter = DateTime.UtcNow.GetQuarter();
            var previousQuarter = currentQuarter-1;
            return await _dbContext.Receivables
                .Where(x=>x.CompanyId == companyId 
                && x.IssueDate.GetQuarter() == previousQuarter
                && !x.Cancelled)
                .ToListAsync();
        }

        public async Task<List<Receivable>> GetCurrentQuarterReceivablesForCompany(Guid companyId)
        {
            var currentQuarter = DateTime.UtcNow.GetQuarter();
            return await _dbContext.Receivables
                .Where(x => x.CompanyId == companyId
                && x.IssueDate.GetQuarter() == currentQuarter
                && !x.Cancelled)
                .ToListAsync();
        }


        //public async Task<List<Invoice>> GetUnpaidInvoicesForCompanyAsync(Guid companyId)
        //{
        //    var invoices = await _dbContext.Invoices.Where(x => x.CompanyId == companyId).ToListAsync();
        //    // discard overdue
        //    return invoices.FindAll(x => !x.IsPaid);
        //}
    }
}
