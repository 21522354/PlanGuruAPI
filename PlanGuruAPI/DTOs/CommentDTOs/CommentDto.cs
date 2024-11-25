namespace PlanGuruAPI.DTOs.CommentDTOs
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Message { get; set; }
        public int NumberOfUpvote { get; set; }
        public int NumberOfDevote { get; set; }
        public List<CommentDto> ReplyComment { get; set; } = new List<CommentDto>();
    }
}
