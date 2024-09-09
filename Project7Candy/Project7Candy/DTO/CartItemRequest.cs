namespace Project7Candy.DTO
{
    public class CartItemRequest
    {
        public int? CartId { get; set; }

        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
    }
}
