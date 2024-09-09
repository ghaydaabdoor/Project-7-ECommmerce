using System;
using System.Collections.Generic;

namespace Project7Candy.Models;

public partial class UserCheckIn1
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? Points { get; set; }

    public DateTime? LastCheckInDate { get; set; }

    public int? HasVoucher { get; set; }

    public virtual User? User { get; set; }
}
