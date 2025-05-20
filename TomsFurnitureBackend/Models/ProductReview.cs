using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class ProductReview
{
    public int Id { get; set; }

    public int StarRating { get; set; }

    public string? Content { get; set; }

    public string? RelatedVideo { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserId { get; set; }

    public int? ProId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ImageProductReview> ImageProductReviews { get; set; } = new List<ImageProductReview>();

    public virtual Product? Pro { get; set; }

    public virtual User? User { get; set; }
}
