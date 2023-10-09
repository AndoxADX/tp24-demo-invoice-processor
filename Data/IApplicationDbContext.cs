using demo_invoice_processor.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_invoice_processor.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Receivable> Receivables { get; }
        DbSet<Debtor> Debtors { get; }
        DbSet<DebtorRisk> DebtorRisks { get; }
        DbSet<Company> Companies { get; }
        DbSet<LoanApplication> LoanApplications { get; }
        DbSet<Invoice> Invoices { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);

    }
}