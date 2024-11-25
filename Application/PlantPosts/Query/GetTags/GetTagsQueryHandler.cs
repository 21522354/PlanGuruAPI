using Application.Common.Interface.Persistence;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlantPosts.Query.GetTags
{
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<string>>, IRequestHandler<GetFiltersQuery, List<string>>
    {
        private readonly ITagRepository _tagRepository;

        public GetTagsQueryHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<string>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            return await _tagRepository.GetTagsAsync();
        }

        public async Task<List<string>> Handle(GetFiltersQuery request, CancellationToken cancellationToken)
        {
            return await _tagRepository.GetFiltersAsync();
        }
    }

}
