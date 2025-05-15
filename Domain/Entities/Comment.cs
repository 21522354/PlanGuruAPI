using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
        public Guid? ParentCommentId { get; set; }
        public virtual Comment? ParentComment { get; set; }

        public string Message { get; set; }
        public ICollection<CommentUpvote> CommentUpvotes { get; set; }
        public ICollection<CommentDevote> CommentDevotes { get; set; }

        // Composite pattern
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public void AddReply(Comment reply)
        {
            if (reply == null) throw new ArgumentNullException(nameof(reply));
            reply.ParentComment = this;
            reply.ParentCommentId = this.Id;
            reply.PostId = this.PostId;
            Replies.Add(reply);
        }

        public void RemoveReply(Comment reply)
        {
            if (reply == null) throw new ArgumentNullException(nameof(reply));
            if (Replies.Remove(reply))
            {
                reply.ParentComment = null;
                reply.ParentCommentId = null;
            }
        }
    }
}
    