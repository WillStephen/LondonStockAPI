using AutoMapper;
using LondonStockApi.Data;
using LondonStockApi.Data.Entities;
using LondonStockApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockTransactionController : ControllerBase
    {
        private readonly IStockContext stockContext;
        private readonly IMapper mapper;
        private readonly ILogger<StockTransactionController> logger;

        public StockTransactionController(
            IStockContext stockContext,
            IMapper mapper,
            ILogger<StockTransactionController> logger)
        {
            this.stockContext = stockContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Send a transaction to the exchange.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<StockTransactionDTO>> Post([FromBody] StockTransactionDTO stockTransaction)
        {
            this.logger.LogInformation("Received stock transaction from client {stockTransaction}", stockTransaction);

            StockTransaction entity = this.mapper.Map<StockTransaction>(stockTransaction);

            await this.stockContext.StockTransactions.AddAsync(entity);
            await this.stockContext.SaveChangesAsync();

            this.logger.LogInformation("Persisted stock transaction {stockTransaction}", stockTransaction);

            return StatusCode(201);
        }
    }
}