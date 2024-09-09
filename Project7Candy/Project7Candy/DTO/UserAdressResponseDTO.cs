namespace Project7Candy.DTO
{
    public class UserAdressResponseDTO
    {
        public int AddressId { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? HomeNumberCode { get; set; }
        public UserDTOo User { get; set; }
    }

    public class UserDTOo
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

    }
}
