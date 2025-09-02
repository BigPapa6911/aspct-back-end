using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.InterventionPlan;

namespace aspcts_backend.Services.Interfaces
{
    public interface IInterventionPlanService
    {
        Task<InterventionPlanResponse> CreateInterventionPlanAsync(InterventionPlanRequest request, Guid psychologistId);
        Task<InterventionPlanResponse?> GetInterventionPlanByIdAsync(Guid planId, Guid userId, string userRole);
        Task<IEnumerable<InterventionPlanResponse>> GetInterventionPlansByChildIdAsync(Guid childId, Guid userId, string userRole);
        Task<InterventionPlanResponse?> GetActiveInterventionPlanAsync(Guid childId, Guid userId, string userRole);
        Task<InterventionPlanResponse?> UpdateInterventionPlanAsync(Guid planId, InterventionPlanRequest request, Guid psychologistId);
        Task<bool> ArchiveInterventionPlanAsync(Guid planId, Guid psychologistId);
        Task<InterventionGoalResponse> AddGoalAsync(Guid planId, InterventionGoalRequest request, Guid psychologistId);
        Task<InterventionGoalResponse?> UpdateGoalAsync(Guid goalId, InterventionGoalRequest request, Guid psychologistId);
    }
}