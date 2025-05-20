namespace TomsFurnitureBackend.VModels
{
    public class UnitCreateVModel
    {
        public string UnitName { get; set; } = null!;

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
    public class UnitUpdateVModel : UnitCreateVModel
    {
        public int Id { get; set; }
    }
    public class UnitGetVModel : UnitUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
