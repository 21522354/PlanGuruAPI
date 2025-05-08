using Application.Comments.Command;
using Application.Common.Interface.Persistence;
using Application.PlantPosts.Query.GetPlantPosts;
using Application.Votes.factory;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanGuruAPI.DTOs;
using PlanGuruAPI.DTOs.CommentDTOs;

namespace PlanGuruAPI.Controllers
{
    [ApiController]
    [Route("api/posts/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ICommentRepository _commentRepository;
        private readonly VoteStrategyFactory _voteStrategyFactory;
        private readonly CommentVoteStrategyFactory _commentVoteStrategyFactory;

        public CommentsController(ISender mediator, ICommentRepository commentRepository, VoteStrategyFactory voteStrategyFactory, CommentVoteStrategyFactory commentVoteStrategyFactory)
        {
            _mediator = mediator;
            _commentRepository = commentRepository;
            _voteStrategyFactory = voteStrategyFactory;
            _commentVoteStrategyFactory = commentVoteStrategyFactory;
        }


        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            var command = new CreateCommentCommand(createCommentDto.PostId, createCommentDto.UserId, createCommentDto.Message);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        [HttpGet("test/get-all")]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentRepository.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.Message = updateCommentDto.Message;
            // Update other properties if needed

            await _commentRepository.UpdateCommentAsync(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            await _commentRepository.DeleteCommentAsync(id);
            return NoContent();
        }

        [HttpGet("posts/{postId}/comments")]
        public async Task<IActionResult> GetCommentsByPostId(Guid postId, [FromQuery] Guid userId, [FromQuery] Guid? parentCommentId = null)
        {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId, parentCommentId);

            var commentDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var commentVoteStrategy = _commentVoteStrategyFactory.CreateStrategy();
                var upvoteCount = await commentVoteStrategy.GetVoteCountAsync(comment.Id, TargetType.Comment, true);
                var devoteCount = await commentVoteStrategy.GetVoteCountAsync(comment.Id, TargetType.Comment, false);

                var hasUpvoted = await commentVoteStrategy.HasUpvotedAsync(userId, comment.Id);
                var hasDevoted = await commentVoteStrategy.HasDevotedAsync(userId, comment.Id);

                var createdAt = GetPlantPostsQueryHandler.FormatCreatedAt(comment.CreatedAt);

                var commentDto = new CommentDto
                {
                    CommentId = comment.Id,
                    UserId = comment.UserId,
                    Name = comment.User.Name,
                    Avatar = comment.User.Avatar,
                    Message = comment.Message,
                    CreatedAt = createdAt,
                    NumberOfUpvote = upvoteCount,
                    NumberOfDevote = devoteCount,
                    HasUpvoted = hasUpvoted, 
                    HasDevoted = hasDevoted  
                };

                commentDtos.Add(commentDto);
            }

            return Ok(commentDtos);
        }

        [HttpPost("upvote")]
        public async Task<IActionResult> UpvoteComment([FromBody] UpvoteDto upvoteDto)
        {
            var voteDto = new VoteDto
            {
                UserId = upvoteDto.UserId,
                TargetId = upvoteDto.TargetId,
                TargetType = TargetType.Comment,
                IsUpvote = true
            };

            var strategy = _voteStrategyFactory.GetStrategy(voteDto.TargetType.ToString());
            await strategy.HandleVoteAsync(voteDto.UserId, voteDto.TargetId, voteDto.IsUpvote);

            var upvoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, true);
            var devoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, false);

            return Ok(new { status = "success", message = "Upvote processed successfully", numberOfUpvotes = upvoteCount, numberOfDevotes = devoteCount });
        }

        [HttpPost("devote")]
        public async Task<IActionResult> DevoteComment([FromBody] DevoteDto devoteDto)
        {
            var voteDto = new VoteDto
            {
                UserId = devoteDto.UserId,
                TargetId = devoteDto.TargetId,
                TargetType = TargetType.Comment,
                IsUpvote = false
            };

            var strategy = _voteStrategyFactory.GetStrategy(voteDto.TargetType.ToString());
            await strategy.HandleVoteAsync(voteDto.UserId, voteDto.TargetId, voteDto.IsUpvote);

            var upvoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, true);
            var devoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, false);

            return Ok(new { status = "success", message = "Devote processed successfully", numberOfUpvotes = upvoteCount, numberOfDevotes = devoteCount });
        }

        [HttpPost("reply")]
        public async Task<IActionResult> ReplyComment([FromBody] ReplyCommentDto replyCommentDto)
        {
            var parentComment = await _commentRepository.GetCommentByIdAsync(replyCommentDto.ParentCommentId);
            if (parentComment == null)
            {
                return NotFound(new { message = "Parent comment not found" });
            }

            // Check if the parent comment is already a reply to another comment
            if (parentComment.ParentCommentId != Guid.Empty)
            {
                var grandParentComment = await _commentRepository.GetCommentByIdAsync(parentComment.ParentCommentId);
                if (grandParentComment?.ParentCommentId != Guid.Empty)
                {
                    return BadRequest(new { message = "Cannot reply to a comment more than 2 levels deep" });
                }
            }

            var replyComment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = replyCommentDto.UserId,
                PostId = parentComment.PostId,
                ParentCommentId = replyCommentDto.ParentCommentId,
                Message = replyCommentDto.Message,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddCommentAsync(replyComment);

            return Ok(new { message = "Reply comment created successfully", replyCommentId = replyComment.Id });
        }
    }
}
