namespace PlanGuruAPI.DTOs.CommentDTOs
{
    public class UpvoteDto
    {
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; } // PostId hoặc CommentId
    }

    public class DevoteDto
    {
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; } // PostId hoặc CommentId
    }
}
