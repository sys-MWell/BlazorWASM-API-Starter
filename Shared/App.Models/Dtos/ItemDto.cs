using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Dtos
{
    public class ItemDto
    {
        [Required]
        public required string ItemName { get; set; }

        public int? BrandId { get; set; }

        public int? SubBrandId { get; set; }

        public int? CategoryId { get; set; }

        public int? ConditionId { get; set; }

        public int? ReceiptId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Price { get; set; }

        public int? Quantity { get; set; } = 1;

        public bool? Boxed { get; set; }

        public string? Notes { get; set; }

        public int? ImageId { get; set; }

        public int? ItemTypeId { get; set; }
    }
}