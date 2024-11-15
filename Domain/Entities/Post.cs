﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Post
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }  
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }        
        public ICollection<PostUpvote> PostUpvotes { get; set; }
        public ICollection<PostDevote> PostDevotes { get; set; }
        public ICollection<PostShare> PostShares { get; set; }  
    }
}
