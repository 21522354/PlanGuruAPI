using Application.Votes.strategy;
using System;

namespace Application.Votes.factory
{
    public class CommentVoteStrategyFactory : VoteStrategyAbstractFactory
    {
        public CommentVoteStrategyFactory(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {}

        public override IVoteStrategy CreateStrategy()
        {
            return _serviceProvider.GetService(typeof(CommentVoteStrategy)) as IVoteStrategy 
                       ?? throw new InvalidOperationException("Service not found: CommentVoteStrategy");
        }
    }
}
