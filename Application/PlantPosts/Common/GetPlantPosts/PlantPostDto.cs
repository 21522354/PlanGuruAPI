using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlantPosts.Common.GetPlantPosts
{
    public record PlantPostDto(
            Guid PostId,
            Guid UserId,
            string UserNickName,
            string UserAvatar,
            string Title,
            string Description,
            string ImageUrl,
            string Tag,
            string Background,
            int NumberOfUpvote,
            int NumberOfDevote,
            int NumberOfComment,
            int NumberOfShare,
            string CreatedDate
        );
}
