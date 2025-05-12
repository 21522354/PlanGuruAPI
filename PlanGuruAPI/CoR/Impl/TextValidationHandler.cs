using PlanGuruAPI.DTOs.GroupDTOs;
using PlanGuruAPI.Interfaces;
using System.Linq;

namespace PlanGuruAPI.CoR.Impl
{
    public class TextValidationHandler : PostApprovalHandler
    {
        public override async Task<bool> HandleAsync(CreatePostInGroupRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                Console.WriteLine("Title is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                Console.WriteLine("Description is required.");
                return false;
            }

            return await base.HandleAsync(request);
        }
    }
}
