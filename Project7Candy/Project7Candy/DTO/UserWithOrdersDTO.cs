namespace Project7Candy.DTO
{
    public class UserWithOrdersDTO
    {
            public int UserId { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public List<OrderDTO> Orders { get; set; }
        }

        public class OrderDTO
        {
            public decimal? TotalAmount { get; set; }

            public string? PaymentMethod { get; set; }
            public string? OrderStatus { get; set; }
        }
    }

