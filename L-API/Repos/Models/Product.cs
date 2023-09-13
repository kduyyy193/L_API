using System;
using System.Collections.Generic;

namespace L_API.Models;

public partial class Product
{
    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public decimal? Price { get; set; }
}
