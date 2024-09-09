namespace Project7Candy.DTO
{
    public class cartItemResponseDTO
    {
        public int CartItemId { get; set; }

        public int? CartId { get; set; }

        public int? Quantity { get; set; }

        public ProductMainRequest Product { get; set; }
    }
}
