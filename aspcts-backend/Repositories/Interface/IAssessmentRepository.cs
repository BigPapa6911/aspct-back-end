using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface IAssessmentRepository : IGenericRepository<Assessment>
    {
        Task<IEnumerable<Assessment>> GetByChildIdAsync(Guid childId);
        Task<IEnumerable<Assessment>> GetByPsychologistIdAsync(Guid psychologistId);
        Task<IEnumerable<T>> GetAssessmentsByTypeAsync<T>(Guid childId) where T : Assessment;
        Task<T?> GetLatestAssessmentByTypeAsync<T>(Guid childId) where T : Assessment;
    }
}