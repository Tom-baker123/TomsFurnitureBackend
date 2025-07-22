using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class Test
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
