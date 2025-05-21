namespace TomsFurnitureBackend.VModels
{
    public class ProductVariantCreateVModel
    {
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int StockQty { get; set; }
        public string? ImageUrl { get; set; }

        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public int MaterialId { get; set; }
        public int UnitId { get; set; }

        public bool? IsActive { get; set; }
    }

    public class ProductVariantUpdateVModel : ProductVariantCreateVModel
    {
        public int Id { get; set; }
    }

    public class ProductVariantGetVModel : ProductVariantUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public string? ProductName { get; set; }
        public string? ColorName { get; set; }
        public string? SizeName { get; set; }
        public string? MaterialName { get; set; }
        public string? UnitName { get; set; }
    }
}
