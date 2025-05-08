using Application.Common.Interface.Persistence;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Votes.strategy
{
    public class CommentVoteStrategy : IVoteStrategy
    {
        private readonly IVoteRepository _voteRepository;

        public CommentVoteStrategy(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task HandleVoteAsync(Guid userId, Guid targetId, bool isUpvote)
        {
            var existingVote = await _voteRepository.GetVoteAsync(userId, targetId, TargetType.Comment);

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == isUpvote)
                {
                    await _voteRepository.RemoveVoteAsync(existingVote);
                    return;
                }
                else
                {
                    await _voteRepository.RemoveVoteAsync(existingVote);
                }
            }

            var vote = new Vote
            {
                UserId = userId,
                TargetId = targetId,
                TargetType = TargetType.Comment,
                IsUpvote = isUpvote
            };

            await _voteRepository.AddVoteAsync(vote);
        }

        public async Task<int> GetVoteCountAsync(Guid targetId, TargetType targetType, bool isUpvote)
        {
            return await _voteRepository.GetVoteCountAsync(targetId, targetType, isUpvote);
        }

        public async Task<bool> HasUpvotedAsync(Guid userId, Guid targetId)
        {
            var vote = await _voteRepository.GetVoteAsync(userId, targetId, TargetType.Comment);
            return vote != null && vote.IsUpvote;
        }

        public async Task<bool> HasDevotedAsync(Guid userId, Guid targetId)
        {
            var vote = await _voteRepository.GetVoteAsync(userId, targetId, TargetType.Comment);
            return vote != null && !vote.IsUpvote;
        }
    }
}
