﻿namespace TomsFurnitureBackend.VModels
{
    public class SliderCreateVModel
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? LinkUrl { get; set; }

        public bool? IsPoster { get; set; }
        public string? Position { get; set; }
        public int DisplayOrder { get; set; }
        public int? ProductId { get; set; }
    }

    public class SliderUpdateVModel : SliderCreateVModel
    {
        public bool? IsActive { get; set; }
        public int Id { get; set; }
    }

    public class SliderGetVModel : SliderUpdateVModel
    {
        public string ImageUrl { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
