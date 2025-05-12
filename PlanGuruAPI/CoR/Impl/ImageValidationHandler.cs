using PlanGuruAPI.DTOs.GroupDTOs;
using PlanGuruAPI.Interfaces;
using System.Linq;

namespace PlanGuruAPI.CoR
{
    public class ImageValidationHandler : PostApprovalHandler
    {
        private readonly List<string> allowedExtensions = new() { ".jpg", ".jpeg", ".png" };

        public override async Task<bool> HandleAsync(CreatePostInGroupRequest request)
        {
            if (request.Images == null || !request.Images.Any())
            {
                Console.WriteLine("At least one image is required.");
                return false;
            }

            foreach (var imageName in request.Images)
            {
                var extension = Path.GetExtension(imageName)?.ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    Console.WriteLine($"Invalid image format: {imageName}");
                    return false;
                }
            }

            return await base.HandleAsync(request);
        }
    }

}
