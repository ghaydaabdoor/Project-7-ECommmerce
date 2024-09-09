using System;
using System.Collections.Generic;

namespace Project7Candy.Models;

public partial class UserAction
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? Points { get; set; }

    public string? Action { get; set; }

    public int? TotalPoints { get; set; }

    public virtual User? User { get; set; }
}
