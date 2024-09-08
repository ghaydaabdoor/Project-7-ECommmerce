namespace Project7Candy.DTO
{
    public class VoucherDetailDTO
    {
        public int UserId { get; set; }
        public int VoucherId { get; set; }
        public int UsageCount { get; set; }
        public string? VoucherCode { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateOnly? ValidFrom { get; set; }
        public DateOnly? ValidTo { get; set; }
        public decimal? MinimumCartValue { get; set; }
        public int? ProductId { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public int? MaxTotalUsage { get; set; }
        public int? IsActive { get; set; }
    }
}
