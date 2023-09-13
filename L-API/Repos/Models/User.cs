using System;
using System.Collections.Generic;

namespace L_API.Models;

public partial class User
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public ulong? Isactive { get; set; }

    public required string Role { get; set; }
}
