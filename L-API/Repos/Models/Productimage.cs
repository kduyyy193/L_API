using System;
using System.Collections.Generic;

namespace L_API.Models;

public partial class Productimage
{
    public int Id { get; set; }

    public string? Productcode { get; set; }

    public byte[]? Productimage1 { get; set; }
}
