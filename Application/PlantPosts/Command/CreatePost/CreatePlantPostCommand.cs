using Application.PlantPosts.Common.CreatePlantPost;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlantPosts.Command.CreatePost
{
    public record CreatePlantPostCommand(
        string Title,
        string Description,
        Guid UserId,
        string ImageUrl,
        string Tag,
        string Background
    ) : IRequest<CreatePostResult>;
}
