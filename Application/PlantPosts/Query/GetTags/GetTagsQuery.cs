using MediatR;

namespace Application.PlantPosts.Query.GetTags
{
    public class GetTagsQuery : IRequest<List<string>>
    {
    }

    public class GetFiltersQuery : IRequest<List<string>>
    {
    }
}
