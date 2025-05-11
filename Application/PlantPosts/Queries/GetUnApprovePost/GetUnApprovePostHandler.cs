using MediatR;
using Application.Common.Interface.Persistence;
using Application.PlantPosts.DTOs;

namespace Application.PlantPosts.Queries.GetUnApprovePost
{
    public class GetUnApprovePostQuery : IRequest<List<PostReadDTO>>
    {
    }

    public class GetUnApprovePostHandler : IRequestHandler<GetUnApprovePostQuery, List<PostReadDTO>>
    {
        private readonly IPlantPostRepository _plantPostRepository;
        private readonly IUserRepository _userRepository;

        public GetUnApprovePostHandler(IPlantPostRepository plantPostRepository, IUserRepository userRepository)
        {
            _plantPostRepository = plantPostRepository;
            _userRepository = userRepository;
        }

        public async Task<List<PostReadDTO>> Handle(GetUnApprovePostQuery request, CancellationToken cancellationToken)
        {
            var listUnApprovedPost = await _plantPostRepository.GetUnApprovedPost();
            listUnApprovedPost = listUnApprovedPost.Where(p => p.GroupId == null).ToList();
            listUnApprovedPost = listUnApprovedPost.OrderByDescending(p => p.CreatedAt).ToList();

            var listUnApprovedPostReadDTO = new List<PostReadDTO>();
            foreach (var post in listUnApprovedPost)
            {
                var user = await _userRepository.GetByIdAsync(post.UserId);

                var listImages = await _plantPostRepository.GetImageForPostAsync(post.Id);
                var listImagesString = listImages.Select(p => p.Image);

                var postReadDTO = new PostReadDTO()
                {
                    UserId = user.UserId,
                    UserAvatar = user.Avatar,
                    UserNickName = user.Name,
                    Background = post.Background,
                    CreatedDate = FormatCreatedAt(post.CreatedAt),
                    CreatedDateDatetime = post.CreatedAt,
                    Description = post.Description,
                    NumberOfComment = post.PostComments.Count,
                    NumberOfDevote = post.PostDevotes.Count,
                    NumberOfShare = post.PostShares.Count,
                    NumberOfUpvote = post.PostUpvotes.Count,
                    PostId = post.Id,
                    Tag = post.Tag,
                    Title = post.Title,
                    Images = listImagesString
                };
                listUnApprovedPostReadDTO.Add(postReadDTO);
            }
            return listUnApprovedPostReadDTO;
        }

        private static string FormatCreatedAt(DateTime createdAt)
        {
            var timeSpan = DateTime.UtcNow - createdAt;
            if (timeSpan.TotalMinutes < 1)
            {
                return "just now";
            }
            else if (timeSpan.TotalHours < 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else if (timeSpan.TotalHours < 24)
            {
                int hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeSpan.TotalDays < 7)
            {
                int days = (int)timeSpan.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else
            {
                return createdAt.ToString("dd-MM-yyyy HH:mm");
            }
        }
    }
}
