using AutoMapper;
using LondonStockApi.Data.Entities;
using LondonStockApi.DTO;

namespace LondonStockApi.Profiles
{
    /// <summary>Maps <see cref="StockTransactionDTO"/> to <see cref="StockTransaction"/>.</summary>
    public class StockTransactionMapping : Profile
    {
        public StockTransactionMapping()
        {
            CreateMap<StockTransactionDTO, StockTransaction>();
        }
    }
}
