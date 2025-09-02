using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.Assessment;

namespace aspcts_backend.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<AssessmentResponse> CreateMilestonesAssessmentAsync(MilestonesAssessmentRequest request, Guid psychologistId);
        Task<AssessmentResponse> CreateBarriersAssessmentAsync(BarriersAssessmentRequest request, Guid psychologistId);
        Task<AssessmentResponse> CreateTransitionAssessmentAsync(TransitionAssessmentRequest request, Guid psychologistId);
        Task<IEnumerable<AssessmentResponse>> GetAssessmentsByChildIdAsync(Guid childId, Guid userId, string userRole);
        Task<AssessmentResponse?> GetAssessmentByIdAsync(Guid assessmentId, Guid userId, string userRole);
        Task<Dictionary<string, object>> GetProgressDataAsync(Guid childId, Guid userId, string userRole);
    }
}