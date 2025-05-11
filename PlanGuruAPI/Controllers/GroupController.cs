using Application.PlantPosts.Common.CreatePlantPost;
using Application.Users.Querry.GetUserById;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs.AdminDTOs;
using PlanGuruAPI.DTOs.GroupDTOs;
using PlanGuruAPI.DTOs.PlantPostDTOs;

namespace PlanGuruAPI.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly PlanGuruDBContext _context;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public GroupController(PlanGuruDBContext context, ISender mediator, IMapper mapper)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllGroup(Guid userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return BadRequest("Can't find this user");
            }
            var listGroup = await _context.Groups
                .Include(p => p.UsersInGroup)
                .Include(p => p.PostInGroup)
                .ToListAsync();
            var listGroupDTO = _mapper.Map<List<GroupReadDTO>>(listGroup);
            foreach (var item in listGroupDTO)
            {
                var checkInGroup = await _context.GroupUsers
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.GroupId == item.Id);
                item.Status = "Not joined";
                if (checkInGroup != null)
                {
                    item.Status = checkInGroup.Status;
                }
            }
            return Ok(listGroupDTO);
        }
        [HttpGet("{userId}/{id}")]
        public async Task<IActionResult> GetGroupById(Guid userId, Guid id)
        {   
            var group = await _context.Groups
                .Include(p => p.PostInGroup)
                .Include(p => p.UsersInGroup)
                .FirstOrDefaultAsync(p => p.Id == id);    
            if (group == null)
            {
                return NotFound("Can't find this group");
            }

            var checkInGroup = await _context.GroupUsers
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.GroupId == group.Id);

            var groupReadDTO = _mapper.Map<GroupReadDTO>(group);
            groupReadDTO.Status = "Not joined";

            if (checkInGroup != null)
            {
                groupReadDTO.Status = checkInGroup.Status;
            }

            return Ok(groupReadDTO);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllGroupForUser(Guid userId)
        {
            var checkUser = await _context.Users.FindAsync(userId);
            if(checkUser == null)
            {
                return BadRequest("This user is not exist");
            }
            var listGroup = await _context.GroupUsers
                .Include(p => p.Group)
                    .ThenInclude(g => g.PostInGroup) // Sử dụng `ThenInclude` để bao gồm quan hệ trong Group
                .Include(p => p.Group)
                    .ThenInclude(g => g.UsersInGroup)
                .Where(p => p.UserId == userId)
                .Select(p => p.Group)
                .ToListAsync();

            var listGroupDTO = _mapper.Map<List<GroupReadDTO>>(listGroup);
            foreach (var item in listGroupDTO)
            {
                var checkInGroup = await _context.GroupUsers
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.GroupId == item.Id);
                item.Status = "Not joined";
                if (checkInGroup != null)
                {
                    item.Status = checkInGroup.Status;
                }
            }
            return Ok(listGroupDTO);
        }
        [HttpGet("search/{searchName}/{userId}")]
        public async Task<IActionResult> SearchGroup(string searchName, Guid userId)
        {
            var listGroup = await _context.Groups
                .Where(p => p.GroupName.ToLower().Contains(searchName.ToLower()))
                .Include(p => p.PostInGroup)
                .Include(p => p.UsersInGroup)
                .ToListAsync();
            var listGroupDTO = _mapper.Map<List<GroupReadDTO>>(listGroup);
            foreach (var item in listGroupDTO)
            {
                var checkInGroup = await _context.GroupUsers
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.GroupId == item.Id);
                item.Status = "Not joined";
                if (checkInGroup != null)
                {
                    item.Status = checkInGroup.Status;
                }
            }
            return Ok(listGroupDTO);
        }
        [HttpGet("posts/pending/{groupId}")]
        public async Task<IActionResult> GetPendingPost(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if(group == null)
            {
                return BadRequest("Can't find this group");
            }
            var listPost = await _context.Posts
                .Where(p => p.GroupId == groupId && p.IsApproved == false)
                .Include(p => p.PostComments)
                .Include(p => p.PostUpvotes)
                .Include(p => p.PostDevotes)
                .Include(p => p.PostShares)
                .Include(p => p.PostImages)
                .Include(p => p.User).ToListAsync();
            return Ok(_mapper.Map<List<PostInGroupDTO>>(listPost));
        }
        [HttpGet("posts/approved/{groupId}")]
        public async Task<IActionResult> GetPostsInGroup(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }
            var listPost = await _context.Posts
                .Where(p => p.GroupId == groupId && p.IsApproved == true)
                .Include(p => p.PostComments)
                .Include(p => p.PostUpvotes)
                .Include(p => p.PostDevotes)
                .Include(p => p.PostShares)
                .Include(p => p.PostImages)
                .Include(p => p.User).ToListAsync();
            return Ok(_mapper.Map<List<PostInGroupDTO>>(listPost));
        }
        [HttpGet("{groupId}/users")]
        public async Task<IActionResult> GetUserInGroup(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }
            var listUser = await _context.GroupUsers
                .Where(p => p.GroupId == groupId && p.Status == "Joined")
                .Include(p => p.User)
                .Select(p => new { p.User.UserId, p.User.Name, p.User.Avatar})
                .ToListAsync();
            return Ok(listUser);    
        }
        [HttpGet("{groupId}/users/pending")]
        public async Task<IActionResult> GetPendingUserInGroup(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }
            var listUser = await _context.GroupUsers
                .Where(p => p.GroupId == groupId && p.Status == "Pending")
                .Include(p => p.User)
                .Select(p => new { p.User.UserId, p.User.Name, p.User.Avatar })
                .ToListAsync();
            return Ok(listUser);
        }
        [HttpGet("ownGroup/{masterUserId}")]
        public async Task<IActionResult> GetOwnGroup(Guid masterUserId)
        {
            var checkUser = await _context.Users.FindAsync(masterUserId);
            if (checkUser == null)
            {
                return BadRequest("Can't find this user");
            }
            var listOwnGroup = await _context.Groups
                .Include(p => p.UsersInGroup)
                .Include(p => p.PostInGroup)    
                .Where(p => p.MasterUserId == masterUserId)
                .ToListAsync();
            return Ok(_mapper.Map<List<GroupReadDTO>>(listOwnGroup));
        }
        [HttpPost("posts/approvePost")]
        public async Task<IActionResult> ApprovePost(ApprovePostRequest request)
        {
            var post = await _context.Posts.FindAsync(request.PostId);
            if (post == null)
            {
                return BadRequest("Can't find this post");
            }
            post.IsApproved = true;
            await _context.SaveChangesAsync();
            return Ok("Post is approved");
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.MasterUserId);
            if (user == null)
            {
                return BadRequest("Can't find this user");
            }
            var group = _mapper.Map<Group>(request);
            group.Id = Guid.NewGuid();
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
            return Ok(new { group.Id, group.GroupName, group.MasterUserId });
        }
        [HttpPost("kickUser")]
        public async Task<IActionResult> BanUser(JoinGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            var group = await _context.Groups.FindAsync(request.GroupId);

            if (user == null)
            {
                return BadRequest("Can't find this user");
            }
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }

            var checkJoined = await _context.GroupUsers
                .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.GroupId == request.GroupId);
            if (checkJoined == null)
            {
                return BadRequest("This user is not in group");
            }
            checkJoined.Status = "Forbidden";
            await _context.SaveChangesAsync();
            return Ok("Kick user out successfully");
        }
        [HttpPost("join")]
        public async Task<IActionResult> JoinGroup(JoinGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);  
            var group = await _context.Groups.FindAsync(request.GroupId);   

            if(user == null)
            {
                return BadRequest("Can't find this user");
            }
            if(group == null)
            {
                return BadRequest("Can't find this group");
            }

            var checkJoined = await _context.GroupUsers
                .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.GroupId == request.GroupId);
            if(checkJoined != null)
            {
                _context.GroupUsers.Remove(checkJoined);
                await _context.SaveChangesAsync();  
                return Ok("Leave group successfully");
            }

            var groupUser = _mapper.Map<GroupUser>(request);
            groupUser.Status = "Pending";
            await _context.GroupUsers.AddAsync(groupUser);
            await _context.SaveChangesAsync();
            return Ok("Send request for join successfully, wating for admin approve");
        }
        [HttpPost("approveJoin")]
        public async Task<IActionResult> approveJoin(JoinGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            var group = await _context.Groups.FindAsync(request.GroupId);

            if (user == null)
            {
                return BadRequest("Can't find this user");
            }
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }

            var checkJoined = await _context.GroupUsers
                .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.GroupId == request.GroupId);
            if(checkJoined == null)
            {
                return BadRequest("Can't find this join request");
            }
            checkJoined.Status = "Joined";
            await _context.SaveChangesAsync();
            return Ok("Approved this user to group");
        }
        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost(CreatePostInGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            var group = await _context.Groups.FindAsync(request.GroupId);
            if (user == null)
            {
                return BadRequest("Can't find this user");
            }
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }

            var post = new Post
            {
                Id = new Guid(),
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId,
                Tag = request.Tag,
                Group = group,
                GroupId = group.Id,
                Background = request.Background,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);   

            foreach (var item in request.Images)
            {
                var postImage = new PostImage()
                {
                    Image = item,
                    Post = post,
                    PostId = post.Id
                };
                _context.PostImages.Add(postImage);
            }

            await _context.SaveChangesAsync();  

            return Ok("Post created successfully, wating for admin approve");
        }
    }
}
