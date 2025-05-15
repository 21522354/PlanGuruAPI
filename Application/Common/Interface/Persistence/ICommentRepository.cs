using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Common.Interface.Persistence
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(Comment comment);
        Task<Comment> GetCommentByIdAsync(Guid? commentId);
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId, Guid? parentCommentId = null);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Guid commentId);
        Task AddCommentUpvoteAsync(CommentUpvote commentUpvote);
        Task RemoveCommentUpvoteAsync(CommentUpvote commentUpvote);
        Task AddCommentDevoteAsync(CommentDevote commentDevote);
        Task RemoveCommentDevoteAsync(CommentDevote commentDevote);
        Task<CommentUpvote> GetCommentUpvoteAsync(Guid userId, Guid commentId);
        Task<CommentDevote> GetCommentDevoteAsync(Guid userId, Guid commentId);
        Task<int> GetCommentUpvoteCountAsync(Guid commentId);
    }
}
