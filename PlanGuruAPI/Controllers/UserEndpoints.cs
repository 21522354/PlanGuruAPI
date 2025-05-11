using Application.Common.Interface.Persistence;

namespace PlanGuruAPI.Controllers
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("api/userss", async (IUserRepository _repo) =>
            {
                return Results.Ok(await _repo.GetAllAsync());
            });
        }
    }
}
