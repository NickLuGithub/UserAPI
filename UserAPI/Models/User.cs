using System;
using System.Collections.Generic;

namespace UserAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? Age { get; set; }

    public string? Sex { get; set; }

    public string? Area { get; set; }
}
