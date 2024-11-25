using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class SeedData
    {
        public static void seedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<PlanGuruDBContext>();
                Console.WriteLine("Seeding Data");

                // Seed Users
                for (int i = 0; i < 5; i++)
                {
                    User user = new User()
                    {
                        UserId = Guid.NewGuid(),
                        Email = $"gmail{i}@gmail.com",
                        Avatar = $"avatar{i}.png",
                        Password = $"password{i}",
                        Name = $"name{i}"
                    };
                    context.Users.Add(user);
                }
                User user2 = new User()
                {
                    UserId = Guid.NewGuid(),
                    Email = "ndam8175@gmail.com",
                    Password = "123123",
                    Avatar = "assda.png",
                    Name = "namdam"
                };
                context.Users.Add(user2);
                context.SaveChanges();

                // Get the first two users
                var firstTwoUsers = context.Users.Take(2).ToList();

                if (firstTwoUsers.Count >= 2)
                {
                    var firstUser = firstTwoUsers[0];
                    var secondUser = firstTwoUsers[1];

                    // Seed Posts
                    for (int i = 0; i < 3; i++)
                    {
                        Post post = new Post()
                        {
                            UserId = firstUser.UserId,
                            Title = $"Post Title {i + 1}",
                            Description = $"This is the description for post {i + 1}.",
                            ImageUrl = $"image{i + 1}.png",
                            Tag = "Plants",
                            Background = "background.png"
                        };
                        context.Posts.Add(post);
                    }
                    context.SaveChanges();

                    // Get the first post
                    var firstPost = context.Posts.FirstOrDefault();

                    if (firstPost != null)
                    {
                        // Seed Comments
                        for (int i = 0; i < 2; i++)
                        {
                            Comment comment = new Comment()
                            {
                                CommentId = Guid.NewGuid(),
                                PostId = firstPost.Id,
                                UserId = firstUser.UserId,
                                Message = $"This is comment {i + 1} on the first post."
                            };
                            context.Comments.Add(comment);
                        }
                        context.SaveChanges();

                        // Get the first comment
                        var firstComment = context.Comments.FirstOrDefault();

                        if (firstComment != null)
                        {
                            // Seed Upvotes for the first comment by the first two users
                            foreach (var user in firstTwoUsers)
                            {
                                CommentUpvote commentUpvote = new CommentUpvote()
                                {
                                    CommentId = firstComment.CommentId,
                                    UserId = user.UserId
                                };
                                context.CommentUpvotes.Add(commentUpvote);
                            }

                            // Seed Upvotes for the first post by the first two users
                            foreach (var user in firstTwoUsers)
                            {
                                PostUpvote postUpvote = new PostUpvote()
                                {
                                    PostId = firstPost.Id,
                                    UserId = user.UserId
                                };
                                context.PostUpvotes.Add(postUpvote);
                            }

                            context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
