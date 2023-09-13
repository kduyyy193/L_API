using System;
using System.Collections.Generic;

namespace L_API.Models;

public partial class Customer
{
    public required string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public decimal? Creditlimit { get; set; }

    public bool IsActive { get; set; }

    public int? Taxcode { get; set; }
}
