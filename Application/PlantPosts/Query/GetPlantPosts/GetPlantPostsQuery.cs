using Application.PlantPosts.Common.GetPlantPosts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlantPosts.Query.GetPlantPosts
{
    public record GetPlantPostsQuery(int Limit, int Page, string? Tag, string? Filter) : IRequest<List<PlantPostDto>>;
}
