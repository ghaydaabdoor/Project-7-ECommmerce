using System;
using System.Collections.Generic;

namespace Project7Candy.Models;

public partial class UserVoucherUsage
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? VoucherId { get; set; }

    public int? UsageCount { get; set; }

    public virtual User? User { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
