using Application.Common.Interface.Persistence;
using Application.Votes.factory;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using PlanGuruAPI.DTOs;
using System;
using System.Threading.Tasks;
namespace PlanGuruAPI.Controllers
{
    [Route("api/votes")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly VoteStrategyFactory _voteStrategyFactory;

        public VoteController(VoteStrategyFactory voteStrategyFactory)
        {
            _voteStrategyFactory = voteStrategyFactory;
        }

        [HttpPost("vote")]
        public async Task<IActionResult> Vote([FromBody] VoteDto voteDto)
        {
            var strategy = _voteStrategyFactory.GetStrategy(voteDto.TargetType.ToString());
            await strategy.HandleVoteAsync(voteDto.UserId, voteDto.TargetId, voteDto.IsUpvote);

            var voteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, true);

            return Ok(new { status = "success", message = "Vote processed successfully", numberOfUpVotes = voteCount });
        }
    }
}
