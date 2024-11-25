using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }      
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }      
        public Guid ParentCommentId { get; set; }
        public virtual Comment ParentComment { get; set; }

        public string Message { get; set; }
        public ICollection<CommentUpvote> CommentUpvotes { get; set; }      
        public ICollection<CommentDevote> CommentDevotes { get; set; }  
    }
}
    