using demo_invoice_processor.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace demo_invoice_processor.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Receivable> Receivables => Set<Receivable>();

        public DbSet<Debtor> Debtors => Set<Debtor>();

        public DbSet<Company> Companies => Set<Company>();

        public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

        public DbSet<Invoice> Invoices => Set<Invoice>();

        public DbSet<DebtorRisk> DebtorRisks => Set<DebtorRisk>();

        protected override void OnConfiguring
      (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "DemoDb");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}