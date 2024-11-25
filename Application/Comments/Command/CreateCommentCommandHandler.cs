using Application.Comments.Common;
using Application.Common.Interface.Persistence;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Command
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CreateCommentResponse>
    {
        private readonly ICommentRepository _commentRepository;

        public CreateCommentCommandHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<CreateCommentResponse> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var commentId = Guid.NewGuid();
            var comment = new Comment
            {
                CommentId = commentId,
                PostId = request.PostId,
                UserId = request.UserId,
                Message = request.Message,
                // Initialize other properties if needed
            };

            await _commentRepository.AddCommentAsync(comment);

            return new CreateCommentResponse
            {
                Status = "success",
                CommentId = commentId,
                Message = "Comment Post successfully"
            };
        }
    }
}
