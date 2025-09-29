using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.Parent;

namespace aspcts_backend.Services.Interfaces
{
    public interface IParentService
    {
        Task<ParentSearchResult?> FindParentByEmailAsync(string email);
        Task<IEnumerable<ParentSearchResult>> SearchParentsAsync(string query);
        Task<ParentDetailInfo?> GetParentInfoAsync(Guid parentId);
    }
}