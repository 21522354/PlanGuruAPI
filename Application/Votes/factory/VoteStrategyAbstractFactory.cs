using Application.Votes.strategy;
using System;

namespace Application.Votes.factory
{
    public abstract class VoteStrategyAbstractFactory
    {
        protected readonly IServiceProvider _serviceProvider;

        protected VoteStrategyAbstractFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public abstract IVoteStrategy CreateStrategy();
    }
}
