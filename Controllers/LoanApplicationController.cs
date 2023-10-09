using demo_invoice_processor.Controllers.Dto;
using demo_invoice_processor.Handlers;
using demo_invoice_processor.Models;
using Microsoft.AspNetCore.Mvc;

namespace demo_invoice_processor.Controllers
{
    [ApiController]
    [Route("api/loanApplication")]
    public class LoanApplicationController : ControllerBase
    {

        private readonly ILogger<LoanApplicationController> _logger;
        private readonly ILoanApplicationHandler _loanApplicationHandler;

        public LoanApplicationController(ILogger<LoanApplicationController> logger, ILoanApplicationHandler loanApplicationHandler)
        {
            _logger = logger;
            _loanApplicationHandler = loanApplicationHandler;
        }

        [HttpGet(Name = "GetLoanApplication")]
        [Route("{loanApplicationId:string}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(LoanApplication), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLoanApplicationAsync([FromRoute] Guid loanApplicationId)
        {
            try
            {

                var loanApplication = await _loanApplicationHandler.GetLoanApplicationAsync(loanApplicationId);
                return Ok(loanApplication);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost(Name = "CreateLoanApplication")]
        [Route("createLoanApplication/{companyId:string}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReceivableRecord), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateLoanApplication([FromRoute] Guid companyId)
        {
            var result = await _loanApplicationHandler.CreateLoanApplication(companyId);
            return Ok(result);
        }

        [HttpGet(Name = "GetCurrentCreditScore")]
        [Route("{debtorId:string}")]
        [Produces("application/json")]
        //[ProducesResponseType(typeof(CreateReceivableRecord), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCurrentCreditScore([FromRoute] Guid debtorId)
        {
            try
            {

                var receivable = await _loanApplicationHandler.GetCompanyCreditScore(debtorId);
                return Ok(receivable);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

    }
}
