using demo_invoice_processor.Controllers.Dto;
using demo_invoice_processor.Handlers;
using demo_invoice_processor.Models;
using Microsoft.AspNetCore.Mvc;

namespace demo_invoice_processor.Controllers
{
    [ApiController]
    [Route("api/receivables")]
    public class CompanyController : ControllerBase
    {

        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyHandler _companyHandler;

        public CompanyController(ILogger<CompanyController> logger,
            ICompanyHandler companyHandler)
        {
            _logger = logger;
            _companyHandler = companyHandler;
        }

        [HttpGet(Name = "GetCompanyAsync")]
        [Route("{companyId:string}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Company), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCompanyAsync([FromRoute] Guid companyId)
        {
            try
            {

                var company = await _companyHandler.GetCompanyAsync(companyId);
                return Ok(company);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet(Name = "GetCompanyDebtorsAsync")]
        [Route("{companyId:string}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Debtor), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCompanyDebtorsAsync([FromRoute] Guid companyId)
        {
            var debtors = await _companyHandler.GetCompanyDebtorsAsync(companyId);
            return Ok(debtors);
        }

        [HttpGet(Name = "GetCompanyDebtorRisksAsync")]
        [Route("{companyId:string}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DebtorRisk), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCompanyDebtorRisksAsync([FromRoute] Guid companyId)
        {
            var debtorRisks = await _companyHandler.GetCompanyDebtorRisksAsync(companyId);
            return Ok(debtorRisks);
        }

    }
}
