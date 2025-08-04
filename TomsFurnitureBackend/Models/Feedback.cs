using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public int? ParentFeedbackId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public virtual ICollection<Feedback> InverseParentFeedback { get; set; } = new List<Feedback>();

    public virtual Feedback? ParentFeedback { get; set; }

    public virtual User? User { get; set; }
}
