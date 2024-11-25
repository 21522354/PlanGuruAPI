using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlantPosts.Query.GetPlantPosts
{
    public class GetPlantPostsQueryValidator : AbstractValidator<GetPlantPostsQuery>
    {
        public GetPlantPostsQueryValidator()
        {
            RuleFor(query => query.Limit)
                .GreaterThan(0).WithMessage("Limit phải lớn hơn 0.")
                .LessThanOrEqualTo(100).WithMessage("Limit không được vượt quá 100.");

            RuleFor(query => query.Page)
                .GreaterThan(0).WithMessage("Page phải lớn hơn 0.");
        }

    }
}
