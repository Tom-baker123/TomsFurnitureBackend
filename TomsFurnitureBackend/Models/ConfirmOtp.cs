using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.Models;

public partial class ConfirmOtp
{
    public int Id { get; set; }

    public string Otpcode { get; set; } = null!;

    public DateTime? ExpiredDate { get; set; }

    public bool CheckActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
