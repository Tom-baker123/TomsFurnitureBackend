﻿using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Slider
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string LinkUrl { get; set; } = null!;

    public bool? IsPoster { get; set; }

    public string? Position { get; set; }

    public int DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProductId { get; set; }

    public virtual Product? Product { get; set; }
}
