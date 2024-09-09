namespace Project7Candy.DTO
{
    public class CommentRequest
    {
        public int CommentId { get; set; }

        public int? ProductId { get; set; }

        public int? UserId { get; set; }

        public string? CommentText { get; set; }

        public UserrRequest UserrRequest { get; set; }
    }
}
