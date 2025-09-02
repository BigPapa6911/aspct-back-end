using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.InterventionPlan;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Services
{
    public class InterventionPlanService : IInterventionPlanService
    {
        private readonly IInterventionPlanRepository _interventionPlanRepository;
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public InterventionPlanService(
            IInterventionPlanRepository interventionPlanRepository,
            IChildRepository childRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _interventionPlanRepository = interventionPlanRepository;
            _childRepository = childRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<InterventionPlanResponse> CreateInterventionPlanAsync(InterventionPlanRequest request, Guid psychologistId)
        {
            var child = await _childRepository.GetByIdAsync(request.ChildId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                throw new ArgumentException("Criança não encontrada ou acesso negado");

            // Archive any existing active plan
            var existingPlan = await _interventionPlanRepository.GetActiveByChildIdAsync(request.ChildId);
            if (existingPlan != null)
            {
                existingPlan.Status = "Archived";
                existingPlan.EndDate = DateTime.UtcNow;
                _interventionPlanRepository.Update(existingPlan);
            }

            var plan = new InterventionPlan
            {
                ChildId = request.ChildId,
                PsychologistId = psychologistId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Goals = request.Goals,
                Status = "Active"
            };

            await _interventionPlanRepository.AddAsync(plan);
            await _interventionPlanRepository.SaveChangesAsync();

            // Add goals
            foreach (var goalRequest in request.InterventionGoals)
            {
                var goal = _mapper.Map<InterventionGoal>(goalRequest);
                goal.PlanId = plan.PlanId;

                // Note: Would need InterventionGoal repository or use context directly
            }

            var createdPlan = await _interventionPlanRepository.GetWithGoalsAsync(plan.PlanId);
            return _mapper.Map<InterventionPlanResponse>(createdPlan);
        }

        public async Task<InterventionPlanResponse?> GetInterventionPlanByIdAsync(Guid planId, Guid userId, string userRole)
        {
            var plan = await _interventionPlanRepository.GetWithGoalsAsync(planId);
            if (plan == null)
                return null;

            var canAccess = await CanAccessChild(plan.ChildId, userId, userRole);
            if (!canAccess)
                return null;

            return _mapper.Map<InterventionPlanResponse>(plan);
        }

        public async Task<IEnumerable<InterventionPlanResponse>> GetInterventionPlansByChildIdAsync(Guid childId, Guid userId, string userRole)
        {
            var canAccess = await CanAccessChild(childId, userId, userRole);
            if (!canAccess)
                return new List<InterventionPlanResponse>();

            var plans = await _interventionPlanRepository.GetByChildIdAsync(childId);
            return plans.Select(p => _mapper.Map<InterventionPlanResponse>(p)).ToList();
        }

        public async Task<InterventionPlanResponse?> GetActiveInterventionPlanAsync(Guid childId, Guid userId, string userRole)
        {
            var canAccess = await CanAccessChild(childId, userId, userRole);
            if (!canAccess)
                return null;

            var plan = await _interventionPlanRepository.GetActiveByChildIdAsync(childId);
            return plan != null ? _mapper.Map<InterventionPlanResponse>(plan) : null;
        }

        public async Task<InterventionPlanResponse?> UpdateInterventionPlanAsync(Guid planId, InterventionPlanRequest request, Guid psychologistId)
        {
            var plan = await _interventionPlanRepository.GetWithGoalsAsync(planId);
            if (plan == null || plan.PsychologistId != psychologistId)
                return null;

            plan.StartDate = request.StartDate;
            plan.EndDate = request.EndDate;
            plan.Goals = request.Goals;
            plan.UpdatedAt = DateTime.UtcNow;

            _interventionPlanRepository.Update(plan);
            await _interventionPlanRepository.SaveChangesAsync();

            var updatedPlan = await _interventionPlanRepository.GetWithGoalsAsync(planId);
            return _mapper.Map<InterventionPlanResponse>(updatedPlan);
        }

        public async Task<bool> ArchiveInterventionPlanAsync(Guid planId, Guid psychologistId)
        {
            var plan = await _interventionPlanRepository.GetByIdAsync(planId);
            if (plan == null || plan.PsychologistId != psychologistId)
                return false;

            plan.Status = "Archived";
            plan.EndDate = DateTime.UtcNow;
            plan.UpdatedAt = DateTime.UtcNow;

            _interventionPlanRepository.Update(plan);
            await _interventionPlanRepository.SaveChangesAsync();

            return true;
        }

        public async Task<InterventionGoalResponse> AddGoalAsync(Guid planId, InterventionGoalRequest request, Guid psychologistId)
        {
            var plan = await _interventionPlanRepository.GetByIdAsync(planId);
            if (plan == null || plan.PsychologistId != psychologistId)
                throw new ArgumentException("Plano não encontrado ou acesso negado");

            var goal = _mapper.Map<InterventionGoal>(request);
            goal.PlanId = planId;

            // Note: Would need to implement goal addition logic
            // For now, returning a mock response
            return _mapper.Map<InterventionGoalResponse>(goal);
        }

        public async Task<InterventionGoalResponse?> UpdateGoalAsync(Guid goalId, InterventionGoalRequest request, Guid psychologistId)
        {
            // Note: Would need InterventionGoal repository to implement this properly
            // For now, returning null
            await Task.CompletedTask;
            return null;
        }

        private async Task<bool> CanAccessChild(Guid childId, Guid userId, string userRole)
        {
            var child = await _childRepository.GetWithDetailsAsync(childId);
            if (child == null || !child.IsActive)
                return false;

            if (userRole == "Psychologist")
            {
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);
                return psychologist != null && child.AssignedPsychologistId == psychologist.PsychologistId;
            }
            else if (userRole == "Parent")
            {
                var parent = await _userRepository.GetParentByUserIdAsync(userId);
                return parent != null &&
                       (child.PrimaryParentId == parent.ParentId || child.SecondaryParentId == parent.ParentId);
            }

            return false;
        }
    }
}