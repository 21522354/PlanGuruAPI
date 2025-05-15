using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Post : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }  
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public string Background { get; set; }
        public Guid? GroupId { get; set; }
        public virtual Group Group { get; set; }    
        public bool IsApproved { get; set; }        
        public ICollection<PostUpvote> PostUpvotes { get; set; } = [];
        public ICollection<PostDevote> PostDevotes { get; set; } = [];
        public ICollection<Comment> PostComments { get; set; } = [];
        public ICollection<PostShare> PostShares { get; set; } = [];
        public ICollection<PostImage> PostImages { get; set; } = [];

        //public void Accept(IVoteVisitor visitor, Guid userId)
        //{
        //    visitor.VisitPost(this, userId);
        //}
    }
}
