using System;
using System.Collections.Generic;

namespace Project7Candy.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public string? VoucherCode { get; set; }

    public decimal? DiscountValue { get; set; }

    public DateOnly? ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }

    public decimal? MinimumCartValue { get; set; }

    public int? ProductId { get; set; }

    public int? MaxUsagePerUser { get; set; }

    public int? MaxTotalUsage { get; set; }

    public int? IsActive { get; set; }

    public virtual ICollection<UserVoucherUsage> UserVoucherUsages { get; set; } = new List<UserVoucherUsage>();
}
