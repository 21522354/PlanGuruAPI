using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Votes.strategy;

namespace Application.Votes.factory
{
    public class VoteStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public VoteStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IVoteStrategy GetStrategy(string targetType)
        {
            return targetType switch
            {
                "Post" => (IVoteStrategy?)_serviceProvider.GetService(typeof(PostVoteStrategy)) ?? throw new InvalidOperationException("Service not found: PostVoteStrategy"),
                "Comment" => (IVoteStrategy?)_serviceProvider.GetService(typeof(CommentVoteStrategy)) ?? throw new InvalidOperationException("Service not found: CommentVoteStrategy"),
                _ => throw new ArgumentException("Invalid target type", nameof(targetType))
            };
        }
    }
}
