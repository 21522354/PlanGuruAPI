using Application.Votes;
using Domain.Entities;

namespace PlanGuruAPI.DTOs
{
    public class VoteDto
    {
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; } // Id of Post, Comment, or Wiki
        public TargetType TargetType { get; set; } // Type of the target: "Post", "Comment", "Wiki"
        public bool IsUpvote { get; set; } // True for upvote, false for devote

        public Post? Post { get; set; }
        public Comment? Comment { get; set; }

        public void Accept(IVoteVisitor visitor, Guid userId)
        {
            if (TargetType == TargetType.Post)
                visitor.VisitPost(this.Post, userId);
            else if (TargetType == TargetType.Comment)
                visitor.VisitComment(this.Comment, userId);
        }
    }
}
