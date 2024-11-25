using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlantPosts.Common.CreatePlantPost
{
    public record CreatePostResult(
        string Status,
        Guid PostId,
        string Message
    );
}
