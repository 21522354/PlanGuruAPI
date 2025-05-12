using Domain.Interfaces;
using PlanGuruAPI.DTOs.GroupDTOs;

namespace PlanGuruAPI.Interfaces
{
    public abstract class PostApprovalHandler : IPostApprovalHandler
    {
        private IPostApprovalHandler _next;

        public IPostApprovalHandler SetNext(IPostApprovalHandler handler)
        {
            _next = handler;
            return handler;
        }

        public virtual async Task<bool> HandleAsync(CreatePostInGroupRequest request)
        {
            if (_next != null)
            {
                return await _next.HandleAsync(request);
            }

            return true;
        }
    }

}
