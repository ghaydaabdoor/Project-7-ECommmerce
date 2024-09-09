namespace Project7Candy.DTO
{
    public class CommentPRequestDTO
    {
        public int? ProductId { get; set; }

        public int? UserId { get; set; }

        public string? CommentText { get; set; }



        public DateTime? CreatedAt { get; set; }
    }
}
