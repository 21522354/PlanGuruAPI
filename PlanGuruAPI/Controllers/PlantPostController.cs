using Application.Common.Interface.Persistence;
using Application.PlantPosts.Command.CreatePost;
using Application.PlantPosts.Query.GetPlantPosts;
using Application.PlantPosts.Query.GetTags;
using Application.Votes.factory;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs;
using PlanGuruAPI.DTOs.CommentDTOs;
using PlanGuruAPI.DTOs.GroupDTOs;
using PlanGuruAPI.DTOs.PlantPostDTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace PlanGuruAPI.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PlantPostController : ControllerBase
    {
        private readonly PlanGuruDBContext _context;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IPlantPostRepository _postRepository;
        private readonly VoteStrategyFactory _voteStrategyFactory;

        public PlantPostController(PlanGuruDBContext context,
            ISender mediator,
            IMapper mapper,
            IPlantPostRepository postRepository,
            VoteStrategyFactory voteStrategyFactory)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
            _postRepository = postRepository;
            _voteStrategyFactory = voteStrategyFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePlantPostRequest request)
        {
            var command = _mapper.Map<CreatePlantPostCommand>(request);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("test/get-all")]
        public async Task<IActionResult> GetAllPlantPosts()
        {
            return Ok(await _postRepository.GetApprovedPost());
        }
        [HttpGet("getCountStatistic")]
        public async Task<IActionResult> GetPlantPostUserCount()
        {
            var wikis = await _context.Wikis.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var posts = await _context.Posts.ToListAsync();
            return Ok(new { numberOfUser = users.Count, numberOfPost = posts.Count, numberOfWiki = wikis.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] Guid userId, [FromQuery] int limit = 9, [FromQuery] int page = 1, [FromQuery] string? tag = null, [FromQuery] string? filter = "time")
        {
            var query = new GetPlantPostsQuery(limit, page, userId, tag, filter);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("test/get-all-tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _mediator.Send(new GetTagsQuery());
            return Ok(tags);
        }

        [HttpGet("test/get-all-filters")]
        public async Task<IActionResult> GetFilters()
        {
            var filters = await _mediator.Send(new GetFiltersQuery());
            return Ok(filters);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostForUser(Guid userId)
        {
            var checkUser = await _context.Users.FindAsync(userId);
            if(checkUser == null)
            {
                return NotFound("User not found");
            }
            var listPostForUser = await _context.Posts
                                     .Include(p => p.PostComments)
                                     .Include(p => p.PostShares)
                                     .Include(p => p.PostUpvotes)
                                     .Include(p => p.PostDevotes)
                                     .Include(p => p.User)
                                     .Include(p => p.PostImages)
                                     .Where(p => p.UserId == userId && p.IsApproved == true).ToListAsync();
            return Ok(_mapper.Map<List<PostInGroupDTO>>(listPostForUser));

            
        }

        [HttpPost("upvote")]
        public async Task<IActionResult> UpvotePost([FromBody] UpvoteDto upvoteDto)
        {
            var voteDto = new VoteDto
            {
                UserId = upvoteDto.UserId,
                TargetId = upvoteDto.TargetId,
                TargetType = TargetType.Post,
                IsUpvote = true
            };

            var strategy = _voteStrategyFactory.GetStrategy(voteDto.TargetType.ToString());
            await strategy.HandleVoteAsync(voteDto.UserId, voteDto.TargetId, voteDto.IsUpvote);

            var upvoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, true);
            var devoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, false);

            return Ok(new { status = "success", message = "Upvote processed successfully", numberOfUpvotes = upvoteCount, numberOfDevotes = devoteCount });
        }

        [HttpPost("devote")]
        public async Task<IActionResult> DevotePost([FromBody] DevoteDto devoteDto)
        {
            var voteDto = new VoteDto
            {
                UserId = devoteDto.UserId,
                TargetId = devoteDto.TargetId,
                TargetType = TargetType.Post,
                IsUpvote = false
            };

            var strategy = _voteStrategyFactory.GetStrategy(voteDto.TargetType.ToString());
            await strategy.HandleVoteAsync(voteDto.UserId, voteDto.TargetId, voteDto.IsUpvote);

            var upvoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, true);
            var devoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, false);

            return Ok(new { status = "success", message = "Devote processed successfully", numberOfUpvotes = upvoteCount, numberOfDevotes = devoteCount });
        }
    }
}
