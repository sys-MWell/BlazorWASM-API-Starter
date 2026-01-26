namespace App.Models.Dtos
{
    public class ItemDetailsDto
    {
        // Item Details
        public int ItemId { get; set; }
        public required string ItemName { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public bool? Boxed { get; set; }
        public string? Notes { get; set; }

        // Foreign Key Ids
        public int? BrandId { get; set; }
        public int? SubBrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? ConditionId { get; set; }
        public int? ReceiptId { get; set; }
        public int? ImageId { get; set; }
        public int? ItemTypeId { get; set; }

        // Item Type Details
        public string? TypeName { get; set; }
        public string? TypeDescription { get; set; }

        // Brand Details
        public string? BrandName { get; set; }
        public string? BrandType { get; set; }
        public string? BrandDescription { get; set; }

        // Category Details
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        public string? CategoryType { get; set; }

        // Condition Details
        public string? ConditionType { get; set; }

        // Receipt Details
        public DateTime? PurchaseDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? StoreId { get; set; }
        public int? ReceiptImageId { get; set; }

        // Store Details
        public string? StoreName { get; set; }
        public int? ParkId { get; set; }
        public int? LocationId { get; set; }

        // Park Details
        public string? ParkName { get; set; }
        public int? CompanyId { get; set; } // Foreign key to ThemeParks

        // ThemePark Details
        public string? ThemeParkCompanyName { get; set; }
        public int? ThemeParkImageId { get; set; }

        // Location Details
        public string? LocationName { get; set; }
        public string? LocationAddressLine1 { get; set; }
        public string? LocationAddressLine2 { get; set; }
        public string? LocationAddressLine3 { get; set; }
        public string? LocationCity { get; set; }
        public string? LocationState { get; set; }
        public string? LocationCountry { get; set; }
        public string? LocationPostalCode { get; set; }
        public string? LocationNotes { get; set; }

        // Receipt Image Details
        public string? ReceiptImageName { get; set; }
        public string? ReceiptImageLocation { get; set; }
        public string? ReceiptImageCaption { get; set; }

        // Store Image Details
        public int? StoreImageId { get; set; }
        public string? StoreImageName { get; set; }
        public string? StoreImageLocation { get; set; }
        public string? StoreImageCaption { get; set; }
    }

}
