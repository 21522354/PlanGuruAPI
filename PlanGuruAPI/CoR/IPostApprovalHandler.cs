using PlanGuruAPI.DTOs.GroupDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPostApprovalHandler
    {
        IPostApprovalHandler SetNext(IPostApprovalHandler handler);
        Task<bool> HandleAsync(CreatePostInGroupRequest request);
    }
}
