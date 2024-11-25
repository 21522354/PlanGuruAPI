using Application.Common.Interface.Persistence;
using Application.PlantPosts.Command.CreatePost;
using Application.PlantPosts.Query.GetPlantPosts;
using Application.PlantPosts.Query.GetTags;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs.CommentDTOs;
using PlanGuruAPI.DTOs.PlantPostDTOs;

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

        public PlantPostController(PlanGuruDBContext context, 
            ISender mediator, 
            IMapper mapper,
            IPlantPostRepository postRepository)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
            _postRepository = postRepository;

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
            return Ok(await _context.Posts.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] int limit = 9, [FromQuery] int page = 1, [FromQuery] string? tag = null, [FromQuery] string? filter = "time")
        {
            var query = new GetPlantPostsQuery(limit, page, tag, filter);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("test/get-all-tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _mediator.Send(new GetTagsQuery());
            return Ok(tags);
        }

        [HttpPost("upvote")]
        public async Task<IActionResult> UpvotePost([FromBody] UpvoteDto upvoteDto)
        {
            var postUpvote = new PostUpvote
            {
                UserId = upvoteDto.UserId,
                PostId = upvoteDto.TargetId
            };

            var existingDevote = await _postRepository.GetPostDevoteAsync(upvoteDto.UserId, upvoteDto.TargetId);

            if (existingDevote != null)
            {
                await _postRepository.RemovePostDevoteAsync(existingDevote);
            }

            var existingUpvote = await _postRepository.GetPostUpvoteAsync(upvoteDto.UserId, upvoteDto.TargetId);

            if (existingUpvote != null)
            {
                await _postRepository.RemovePostUpvoteAsync(existingUpvote);
                await _postRepository.RemovePostUpvoteAsync(existingUpvote);
                var response2 = new
                {
                    status = "success",
                    message = "Remove upvote post successfully"
                };
                return Ok(response2);
            }

            await _postRepository.AddPostUpvoteAsync(postUpvote);
            var response = new
            {
                status = "success",
                message = "Upvote post successfully"
            };

            return Ok(response);
        }

        [HttpPost("devote")]
        public async Task<IActionResult> DevotePost([FromBody] DevoteDto devoteDto)
        {
            var postDevote = new PostDevote
            {
                UserId = devoteDto.UserId,
                PostId = devoteDto.TargetId
            };

            var existingUpvote = await _postRepository.GetPostUpvoteAsync(devoteDto.UserId, devoteDto.TargetId);

            if (existingUpvote != null)
            {
                await _postRepository.RemovePostUpvoteAsync(existingUpvote);
            }

            var existingDevote = await _postRepository.GetPostDevoteAsync(devoteDto.UserId, devoteDto.TargetId);

            if (existingDevote != null)
            {
                await _postRepository.RemovePostDevoteAsync(existingDevote);
                var response2 = new
                {
                    status = "success",
                    message = "Remove devote post successfully"
                };
                return Ok(response2);
            }

            await _postRepository.AddPostDevoteAsync(postDevote);
            var response = new
            {
                status = "success",
                message = "Devote post successfully"
            };

            return Ok(response);
        }
    }
}
