using Application.Common.Interface.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PlanGuruDBContext _context;

        public CommentRepository(PlanGuruDBContext context)
        {
            _context = context;
        }

        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(Guid? commentId)
        {
            return await _context.Comments.FindAsync(commentId);
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId, Guid? parentCommentId = null)
        {
            //// Step 1: Lấy tất cả các comment theo postId
            //var commentsQuery = _context.Comments.Where(c => c.PostId == postId);
            //var comments = await commentsQuery.ToListAsync();

            //// Step 2: Lọc theo parentCommentId nếu có
            //if (parentCommentId.HasValue && parentCommentId.Value != Guid.Empty)
            //{
            //    commentsQuery = commentsQuery.Where(c => c.ParentCommentId == parentCommentId.Value);
            //}
            //else
            //{
            //    commentsQuery = commentsQuery.Where(c => c.ParentCommentId == null || c.ParentCommentId == Guid.Empty);
            //}
            //comments = await commentsQuery.ToListAsync();

            //// Step 3: Include User
            //commentsQuery = commentsQuery.Include(c => c.User);
            //comments = await commentsQuery.ToListAsync();

            //// Step 4: Include CommentUpvotes
            //commentsQuery = commentsQuery.Include(c => c.CommentUpvotes);
            //comments = await commentsQuery.ToListAsync();

            //// Step 5: Include CommentDevotes
            //commentsQuery = commentsQuery.Include(c => c.CommentDevotes);
            //comments = await commentsQuery.ToListAsync();

            //return comments;

            var allComments = await _context.Comments
                .Where(c => c.PostId == postId &&
                   (parentCommentId == null || c.ParentCommentId == parentCommentId))
                .Include(c => c.User)
                .Include(c => c.CommentUpvotes)
                .Include(c => c.CommentDevotes)
                .ToListAsync();

            // Step 2: Group by ParentCommentId for fast lookup
            var lookup = allComments.ToLookup(c => c.ParentCommentId);

            // Step 3: Recursively build the tree
            foreach (var comment in allComments)
            {
                comment.Replies = lookup[comment.Id].ToList();
            }

            // Step 4: Return root-level comments (no parent)
            return lookup[parentCommentId];
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Guid commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCommentUpvoteAsync(CommentUpvote commentUpvote)
        {
            _context.CommentUpvotes.Add(commentUpvote);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommentUpvoteAsync(CommentUpvote commentUpvote)
        {
            _context.CommentUpvotes.Remove(commentUpvote);
            await _context.SaveChangesAsync();
        }

        public async Task AddCommentDevoteAsync(CommentDevote commentDevote)
        {
            _context.CommentDevotes.Add(commentDevote);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommentDevoteAsync(CommentDevote commentDevote)
        {
            _context.CommentDevotes.Remove(commentDevote);
            await _context.SaveChangesAsync();
        }

        public async Task<CommentUpvote?> GetCommentUpvoteAsync(Guid userId, Guid commentId)
        {
            return await _context.CommentUpvotes
                .FirstOrDefaultAsync(cu => cu.UserId == userId && cu.CommentId == commentId);
        }

        public async Task<CommentDevote?> GetCommentDevoteAsync(Guid userId, Guid commentId)
        {
            return await _context.CommentDevotes
                .FirstOrDefaultAsync(cd => cd.UserId == userId && cd.CommentId == commentId);
        }

        public async Task<int> GetCommentUpvoteCountAsync(Guid commentId)
        {
            return await _context.CommentUpvotes.CountAsync(cu => cu.CommentId == commentId);
        }
    }
}
