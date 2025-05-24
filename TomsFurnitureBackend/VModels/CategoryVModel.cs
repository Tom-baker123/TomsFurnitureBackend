namespace TomsFurnitureBackend.VModels
{
    // CreateVModel
    public class CategoryCreateVModel
    {
        public string? CategoryName { get; set; }

        public string? Descriptions { get; set; }

        public string? ImageUrl { get; set; }

        public bool? IsActive { get; set; }
    }
    
    // UpdateVModel - CreateVModel
    public class CategoryUpdateVModel : CategoryCreateVModel
    {
        public int Id { get; set; }
    }

    // GetVModel - UpdateVModel
    public class CategoryGetVModel : CategoryUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
