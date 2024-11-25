using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Command
{
    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(c => c.PostId)
                .NotEmpty().WithMessage("PostId không được để trống")
                .Must(postId => Guid.TryParse(postId.ToString(), out _))
                .WithMessage("PostId phải là một GUID hợp lệ");

            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("UserId không được để trống")
                .Must(userId => Guid.TryParse(userId.ToString(), out _))
                .WithMessage("UserId phải là một GUID hợp lệ");
        }
    }
}
