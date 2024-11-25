namespace PlanGuruAPI.DTOs.CommentDTOs
{
    public record CreateCommentDto(
            Guid PostId,
            Guid UserId,
            string Message
        );
}
