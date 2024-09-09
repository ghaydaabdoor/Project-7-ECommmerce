namespace Project7Candy.DTO
{
    public class ProductMainRequest
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductDescription { get; set; }

        public decimal? Price { get; set; }

        public int? Stock { get; set; }

        public decimal? Discount { get; set; }

        public decimal? Rate { get; set; }

        public string? ProductImage { get; set; }
    }
}
