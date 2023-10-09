using AutoMapper;
using demo_invoice_processor.Controllers.Dto;
using demo_invoice_processor.Handlers;
using demo_invoice_processor.Models;
using Microsoft.AspNetCore.Mvc;

namespace demo_invoice_processor.Controllers
{
    [ApiController]
    [Route("api/receivables")]
    public class ReceivableController : ControllerBase
    {

        private readonly ILogger<ReceivableController> _logger;
        private readonly IReceivableHandler _receivableHandler;
        private readonly IMapper _mapper;

        public ReceivableController(ILogger<ReceivableController> logger, 
            IReceivableHandler receivableHandler, IMapper mapper)
        {
            _logger = logger;
            _receivableHandler = receivableHandler;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetReceivable")]
        [Route("{receivableId:string}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReceivableRecord), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReceivableAsync([FromRoute] Guid receivableId)
        {
            try
            {

                var receivable = await _receivableHandler.GetReceivableAsync(receivableId);
                return Ok(_mapper.Map<ReceivableRecord>(receivable));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost(Name = "UploadReceivableAsync")]
        [Route("uploadReceivableAsync")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReceivableRecord), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UploadReceivableAsync([FromBody] ReceivableRecord receivable)
        {
            var result = await _receivableHandler.UploadReceivableAsync(receivable);
            return Ok(result);
        }

    }
}
