namespace Project7Candy.DTO
{
    public class VoucherRequestDTO
    {
        public string? VoucherCode { get; set; }

        public decimal? DiscountValue { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }


        public decimal? MinimumCartValue { get; set; }

        public int? ProductId { get; set; }

        public int? MaxUsagePerUser { get; set; }

        public int? MaxTotalUsage { get; set; }

        public int? IsActive { get; set; }
    }
}
