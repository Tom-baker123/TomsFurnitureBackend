using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int LikeCount { get; set; }

    public string? Content { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProReviewId { get; set; }

    public int? UserId { get; set; }

    public virtual ProductReview? ProReview { get; set; }

    public virtual User? User { get; set; }
}
