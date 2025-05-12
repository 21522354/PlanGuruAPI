using PlanGuruAPI.DTOs.GroupDTOs;
using PlanGuruAPI.Interfaces;
using System.Linq;

namespace PlanGuruAPI.CoR
{
    public class ProfanityFilterHandler : PostApprovalHandler
    {
        private readonly List<string> bannedWords = new() { "badword", "curse" };

        public override async Task<bool> HandleAsync(CreatePostInGroupRequest request)
        {
            bool containsBannedWord = bannedWords.Any(word =>
                request.Title.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                request.Description.Contains(word, StringComparison.OrdinalIgnoreCase)
            );

            if (containsBannedWord)
            {
                Console.WriteLine("Profanity detected in title or description.");
                return false;
            }

            return await base.HandleAsync(request);
        }
    }
}
