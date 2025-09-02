using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.Child;

namespace aspcts_backend.Services.Interfaces
{
    public interface IChildService
    {
        Task<ChildResponse> CreateChildAsync(ChildCreateRequest request, Guid psychologistId);
        Task<ChildResponse?> GetChildByIdAsync(Guid childId, Guid userId, string userRole);
        Task<IEnumerable<ChildResponse>> GetChildrenAsync(Guid userId, string userRole);
        Task<ChildResponse?> UpdateChildAsync(Guid childId, ChildUpdateRequest request, Guid psychologistId);
        Task<bool> DeleteChildAsync(Guid childId, Guid psychologistId);
        Task<bool> CanAccessChildAsync(Guid childId, Guid userId, string userRole);
    }
}