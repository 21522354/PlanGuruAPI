using Application.Users.Command.SetNameAndAvatar;
using Application.Users.Command.SignUp;
using Application.Users.Common;
using Application.Users.Querry.GetUserById;
using Application.Users.Querry.Login;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs.UserDTOs;
using System.Diagnostics.Eventing.Reader;

namespace PlanGuruAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PlanGuruDBContext _context;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public UserController(PlanGuruDBContext context, ISender mediator, IMapper mapper)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var querry = _mapper.Map<LoginQuerry>(loginRequest);
            var loginResult = await _mediator.Send(querry); 
            return Ok(loginResult);
        }
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(p => p.Email == signUpRequest.email);
            if (existUser != null)
            {
                return BadRequest("This email is already exist");
            } 
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Avatar = signUpRequest.avatar,
                Email = signUpRequest.email,
                Password = signUpRequest.password,
                Name = signUpRequest.name,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new {userId = user.Id});
        }
        [HttpPost("setNameAndAvatar")]
        public async Task<IActionResult> SetNameAndAvatar(SetNameAndAvatarRequest setNameAndAvatarRequest)
        {
            var command = _mapper.Map<SetNameAndAvatarCommand>(setNameAndAvatarRequest);
            var result = await _mediator.Send(command);
            return Ok(result);      
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await _context.Users.Select(p => new {p.UserId, p.Name, p.Avatar, p.Email, p.IsHavePremium}).ToListAsync());
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return NotFound("Can't find this user");
            }

            return Ok(user);
        }
        [HttpPost("goPremium")]
        public async Task<IActionResult> GoPremium(GoPremiumDTO request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if(user == null)
            {
                return NotFound("Can't find this user");
            }
            if (user.IsHavePremium)
            {
                return BadRequest("This user is already have premium");
            }
            user.IsHavePremium = true;
            var membershipHistory = new MembershipHistory()
            {
                PackageName = request.PackageName,
                PackagePrice = request.PackagePrice,
                User = user,
                UserId = request.UserId,
                CreatedAt = DateTime.Now
            };
            _context.MembershipsHistory.Add(membershipHistory);
            await _context.SaveChangesAsync();
            return Ok("Upgrade account successfully");
        }
        [HttpPost("removePremium")]
        public async Task<IActionResult> RemovePremium(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Can't find this user");
            }
            if (!user.IsHavePremium)
            {
                return BadRequest("This user don't have premium");
            }
            user.IsHavePremium = false;
            await _context.SaveChangesAsync();
            return Ok("Remove premium from this account successfully");
        }

        [HttpGet("{userId}/experience-points")]
        public async Task<IActionResult> GetExperiencePoints(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new { experiencePoints = user.TotalExperiencePoints });
        }

        [HttpPut("{userId}/experience-points")]
        public async Task<IActionResult> UpdateExperiencePoints(Guid userId, [FromBody] int points)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            user.TotalExperiencePoints = points;
            await _context.SaveChangesAsync();
            return Ok(new { experiencePoints = user.TotalExperiencePoints });
        }

        [HttpPut("{userId}/add-experience-points")]
        public async Task<IActionResult> AddExperiencePoints(Guid userId, [FromBody] int points)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            user.TotalExperiencePoints += points;
            await _context.SaveChangesAsync();
            return Ok(new { experiencePoints = user.TotalExperiencePoints });
        }

        [HttpGet("{userId}/contribution-points")]
        public async Task<IActionResult> GetContributionPoints(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new { contributionPoints = user.TotalContributionPoints });
        }

        [HttpPut("{userId}/contribution-points")]
        public async Task<IActionResult> UpdateContributionPoints(Guid userId, [FromBody] int points)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            user.TotalContributionPoints = points;
            await _context.SaveChangesAsync();
            return Ok(new { contributionPoints = user.TotalContributionPoints });
        }
    }
}
