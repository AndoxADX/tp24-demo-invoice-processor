using demo_invoice_processor.Data;
using demo_invoice_processor.Handlers;
using demo_invoice_processor.Models;
using demo_invoice_processor.Processor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace demo_invoice_processor
{
    public static class IoC
    {
        public static IServiceCollection Bind(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CreditScoreRating>
                (configuration.GetSection("AppSettings:InvoiceFinancingParameters"));
            services
                .AddInfrastructure(configuration)
                .AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddSingleton<IReceivableHandler, ReceivableHandler>()
                .AddSingleton<ILoanApplicationHandler, LoanApplicationHandler>()
                .AddSingleton<ICompanyHandler, CompanyHandler>()
                .AddSingleton<IDebtorRiskAssessor, DebtorRiskAssessor>()
                .AddSingleton<IFinanceRatingProcessor, FinanceRatingProcessor>()
                .AddSingleton<IInvoiceRatingAssesor, InvoiceRatingAssesor>()
                .AddSingleton<ILoanApplicationProcessor, LoanApplicationProcessor>();
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("DemoDb"));
            }
            else
            {
                throw new Exception("InvalidDB");
            }

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            return services;
        }
    }
}
