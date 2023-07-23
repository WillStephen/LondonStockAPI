using AutoMapper;
using LondonStockApi.Data.Entities;
using LondonStockApi.DTO;

namespace LondonStockApi.Profiles
{
    public class StockTransactionMapping : Profile
    {
        public StockTransactionMapping()
        {
            CreateMap<StockTransactionDTO, StockTransaction>();
        }
    }
}
