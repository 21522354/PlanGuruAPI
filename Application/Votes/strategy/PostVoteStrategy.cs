using Application.Common.Interface.Persistence;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Votes.strategy
{
    public class PostVoteStrategy : IVoteStrategy
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IUserRepository _userRepo;
        private readonly IPlantPostRepository _postRepo;

        public PostVoteStrategy(IVoteRepository voteRepository, IUserRepository userRepo, IPlantPostRepository postRepo)
        {
            _voteRepository = voteRepository;
            _userRepo = userRepo;
            _postRepo = postRepo;
        }

        public async Task HandleVoteAsync(Guid userId, Guid targetId, bool isUpvote)
        {
            var existingVote = await _voteRepository.GetVoteAsync(userId, targetId, TargetType.Post);
            var post = await _postRepo.GetPostByIdAsync(targetId);
            var author = await _userRepo.GetByIdAsync(post.UserId);

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == isUpvote)
                {
                    await _voteRepository.RemoveVoteAsync(existingVote);
                    author.TotalExperiencePoints -= isUpvote ? 10 : -10;
                    if (author.TotalExperiencePoints < 0) author.TotalExperiencePoints = 0;
                    await _userRepo.UpdateAsync(author);
                    return;
                }
                else
                {
                    await _voteRepository.RemoveVoteAsync(existingVote);
                    author.TotalExperiencePoints -= existingVote.IsUpvote ? 10 : -10;
                }
            }

            var vote = new Vote
            {
                UserId = userId,
                TargetId = targetId,
                TargetType = TargetType.Post,
                IsUpvote = isUpvote
            };

            await _voteRepository.AddVoteAsync(vote);
            author.TotalExperiencePoints += isUpvote ? 10 : -10;
            if (author.TotalExperiencePoints < 0) author.TotalExperiencePoints = 0;
            await _userRepo.UpdateAsync(author);
        }

        public async Task<int> GetVoteCountAsync(Guid targetId, TargetType targetType, bool isUpvote)
        {
            return await _voteRepository.GetVoteCountAsync(targetId, targetType, isUpvote);
        }

        public async Task<bool> HasUpvotedAsync(Guid userId, Guid targetId)
        {
            var vote = await _voteRepository.GetVoteAsync(userId, targetId, TargetType.Post);
            return vote != null && vote.IsUpvote;
        }

        public async Task<bool> HasDevotedAsync(Guid userId, Guid targetId)
        {
            var vote = await _voteRepository.GetVoteAsync(userId, targetId, TargetType.Post);
            return vote != null && !vote.IsUpvote;
        }
    }
}
