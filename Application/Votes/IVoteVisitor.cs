using Application.Common.Interface.Persistence;
using Domain.Entities;

namespace Application.Votes
{
    public interface IVoteVisitor
    {
        void VisitPost(Post post, Guid userId);
        void VisitComment(Comment comment, Guid userId);
    }

    public class UpvoteVisitor : IVoteVisitor
    {
        private readonly IPlantPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IUserRepository _userRepository;

        public UpvoteVisitor(IPlantPostRepository postRepository, ICommentRepository commentRepository, IVoteRepository voteRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _voteRepository = voteRepository;
            _userRepository = userRepository;
        }

        public async void VisitPost(Post post, Guid userId)
        {
            var existingVote = await _voteRepository.GetVoteAsync(userId, post.Id, TargetType.Post);
            var author = await _userRepository.GetByIdAsync(post.UserId);

            if (author == null)
            {
                return;
            }

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == true)
                {
                    await _voteRepository.RemoveVoteAsync(existingVote);
                    author.TotalExperiencePoints -= 10;
                    if (author.TotalExperiencePoints < 0) author.TotalExperiencePoints = 0;
                    await _userRepository.UpdateAsync(author);
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
                TargetId = post.Id,
                TargetType = TargetType.Post,
                IsUpvote = true
            };

            await _voteRepository.AddVoteAsync(vote);
            author.TotalExperiencePoints += 10;
            if (author.TotalExperiencePoints < 0) author.TotalExperiencePoints = 0;
            await _userRepository.UpdateAsync(author);
        }

        public async void VisitComment(Comment comment, Guid userId)
        {
            var existingVote = await _voteRepository.GetVoteAsync(userId, comment.Id, TargetType.Comment);

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == true)
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
                TargetId = comment.Id,
                TargetType = TargetType.Comment,
                IsUpvote = true
            };

            await _voteRepository.AddVoteAsync(vote);
        }
    }

    public class DevoteVisitor : IVoteVisitor
    {
        private readonly IPlantPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IUserRepository _userRepository;

        public DevoteVisitor(IPlantPostRepository postRepository, ICommentRepository commentRepository, IVoteRepository voteRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _voteRepository = voteRepository;
            _userRepository = userRepository;
        }

        public async void VisitPost(Post post, Guid userId)
        {
            var existingVote = await _voteRepository.GetVoteAsync(userId, post.Id, TargetType.Post);
            var author = await _userRepository.GetByIdAsync(post.UserId);

            if (author == null)
            {
                return;
            }

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == false)
                {
                    await _voteRepository.RemoveVoteAsync(existingVote);
                    author.TotalExperiencePoints -= -10;
                    if (author.TotalExperiencePoints < 0) author.TotalExperiencePoints = 0;
                    await _userRepository.UpdateAsync(author);
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
                TargetId = post.Id,
                TargetType = TargetType.Post,
                IsUpvote = false
            };

            await _voteRepository.AddVoteAsync(vote);
            author.TotalExperiencePoints += -10;
            if (author.TotalExperiencePoints < 0) author.TotalExperiencePoints = 0;
            await _userRepository.UpdateAsync(author);
        }

        public async void VisitComment(Comment comment, Guid userId)
        {
            var existingVote = await _voteRepository.GetVoteAsync(userId, comment.Id, TargetType.Comment);

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == false)
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
                TargetId = comment.Id,
                TargetType = TargetType.Comment,
                IsUpvote = false
            };

            await _voteRepository.AddVoteAsync(vote);
        }
    }
}
