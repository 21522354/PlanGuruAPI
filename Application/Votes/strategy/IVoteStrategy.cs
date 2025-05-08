using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Votes.strategy
{
    public interface IVoteStrategy
    {
        Task HandleVoteAsync(Guid userId, Guid targetId, bool isUpvote);
        Task<int> GetVoteCountAsync(Guid targetId, TargetType targetType, bool isUpvote);
        Task<bool> HasUpvotedAsync(Guid userId, Guid targetId);
        Task<bool> HasDevotedAsync(Guid userId, Guid targetId);
    }
}
