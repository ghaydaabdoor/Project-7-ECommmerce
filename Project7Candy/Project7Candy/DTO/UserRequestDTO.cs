namespace Project7Candy.DTO
{
    public class UserRequestDTO
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Password { get; set; }
        public string? Email { get; set; }


        public string? Phone { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}
