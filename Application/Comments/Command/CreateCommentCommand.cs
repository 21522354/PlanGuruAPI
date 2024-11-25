using Application.Comments.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Command
{
    public record CreateCommentCommand(Guid PostId, Guid UserId, string Message) : IRequest<CreateCommentResponse>;

}
