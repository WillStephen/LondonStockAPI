using AutoMapper;
using LondonStockApi.Data.Entities;
using LondonStockApi.DTO;

namespace LondonStockApi.MappingProfiles
{
    /// <summary>Maps <see cref="StockQuote"/> to <see cref="StockQuoteDTO"/>.</summary>
    public class StockQuoteMapping : Profile
    {
        public StockQuoteMapping()
        {
            CreateMap<StockQuote, StockQuoteDTO>();
        }
    }
}
