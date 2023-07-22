using LondonStockApi.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockApi.Controllers
{
    [ApiController]
    [Route("stocktransaction")]
    public class StockTransactionController : ControllerBase
    {
        private readonly ILogger<StockTransactionController> logger;

        public StockTransactionController(ILogger<StockTransactionController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Send a transaction to the exchange.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<StockTransactionDTO>> Post([FromBody] StockTransactionDTO stockTransaction)
        {
            this.logger.LogInformation("Received stock transaction {stockTransaction}", stockTransaction);

            return StatusCode(201);
        }
    }
}