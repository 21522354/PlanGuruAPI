using System.Net.NetworkInformation;
using Application.PlantPosts.Command.CreatePost;
using Application.PlantPosts.Query.GetPlantPosts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs.PlantPostDTOs;

namespace PlanGuruAPI.Controllers
{
    public static class PostEndpoints
    {
	    public static void MapPostEndpoints (this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Post").WithTags(nameof(Post));

            group.MapPost("/", async (CreatePlantPostRequest request, ISender mediator, IMapper mapper) =>
            {
                var createPlantPostCommand = mapper.Map<CreatePlantPostCommand>(request);

                var result = await mediator.Send(createPlantPostCommand);

                return TypedResults.Ok(result);
            }).WithName("CreatePost").WithOpenApi();

            group.MapGet("/", async (Guid userId, ISender mediator ,int limit = 9, int page = 1, string tag = null, string filter = "time") =>
            {
                var querry = new GetPlantPostsQuery(limit, page, userId, tag, filter);

                var result = await mediator.Send(querry);

                return TypedResults.Ok(result);
            }).WithName("GetPost").WithOpenApi();


            group.MapGet("/getCountStatistic", async (PlanGuruDBContext context) =>
            {
                var listUser = await context.Users.ToListAsync();
                var listPost = await context.Posts.ToListAsync();
                var listWiki = await context.Wikis.ToListAsync();

                return TypedResults.Ok(new { NumberOfUser = listUser.Count, NumberOfPost = listPost.Count, listWiki.Count });
            }).WithName("GetCountStatistic").WithOpenApi();

        }
    }
}
