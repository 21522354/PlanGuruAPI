using Application.Votes.strategy;
using System;

namespace Application.Votes.factory
{
    public class PostVoteStrategyFactory : VoteStrategyAbstractFactory
    {
        public PostVoteStrategyFactory(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {}

        public override IVoteStrategy CreateStrategy()
        {
            return _serviceProvider.GetService(typeof(PostVoteStrategy)) as IVoteStrategy 
                       ?? throw new InvalidOperationException("Service not found: PostVoteStrategy");
        }
    }
}
