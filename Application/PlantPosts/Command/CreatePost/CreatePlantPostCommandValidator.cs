using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlantPosts.Command.CreatePost
{
    public class CreatePlantPostCommandValidator : AbstractValidator<CreatePlantPostCommand>
    {
        public CreatePlantPostCommandValidator()
        {
            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("UserId không được để trống")
                .Must(userId => Guid.TryParse(userId.ToString(), out _))
                .WithMessage("UserId phải là một GUID hợp lệ");

            RuleFor(p => p.Tag)
                .NotEmpty().WithMessage("Tag không được để trống");

            RuleFor(p => p.Background)
                .NotEmpty().WithMessage("Background không được để trống");
        }
    }
}
