using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interface.Persistence
{
    public interface IPlantPostRepository
    {
        Task<Post> CreatePostAsync(Post post);
        IQueryable<Post> QueryPosts();
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(Guid postId);

        Task AddPostUpvoteAsync(PostUpvote postUpvote);
        Task RemovePostUpvoteAsync(PostUpvote postUpvote);
        Task AddPostDevoteAsync(PostDevote postDevote);
        Task RemovePostDevoteAsync(PostDevote postDevote);
        Task<PostUpvote> GetPostUpvoteAsync(Guid userId, Guid postId);
        Task<PostDevote> GetPostDevoteAsync(Guid userId, Guid postId);

    }
}
