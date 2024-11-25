using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Common
{
    public class CreateCommentResponse
    {
        public string Status { get; set; }
        public Guid CommentId { get; set; }
        public string Message { get; set; }
    }
}
