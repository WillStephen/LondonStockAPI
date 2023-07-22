using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LondonStockApi.Data.Entities
{
    [Table("StockTransactions")]
    public class StockTransaction
    {
        /// <summary>Unique ID of this record</summary>
        /// <remarks>Auto-incrementing column - no need to set in code.</remarks>
        [Key]
        public int Id { get; init; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public required string Ticker { get; init; }

        [Required]
        [Column(TypeName = "decimal(19,5)")]
        public decimal PriceInSterling { get; init; }

        [Required]
        [Column(TypeName = "decimal(19,5)")]
        public decimal NumberOfShares { get; init; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid BrokerId { get; init; }
    }
}
