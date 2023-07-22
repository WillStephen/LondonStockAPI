using LondonStockApi.Data;
using LondonStockApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockApi.Controllers
{
    [ApiController]
    [Route("stocktransaction")]
    public class StockTransactionController : ControllerBase
    {
        private readonly IStockContext stockContext;
        private readonly ILogger<StockTransactionController> logger;

        public StockTransactionController(IStockContext stockContext, ILogger<StockTransactionController> logger)
        {
            this.stockContext = stockContext;
            this.logger = logger;
        }

        /// <summary>
        /// Send a transaction to the exchange.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<StockTransactionDTO>> Post([FromBody] StockTransactionDTO stockTransaction)
        {
            this.logger.LogInformation("Received stock transaction {stockTransaction}", stockTransaction);

            // TODO: map DTO to entity and commit to DB

            return StatusCode(201);
        }
    }
}