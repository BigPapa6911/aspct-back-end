using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Child;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Services
{
    public class ChildService : IChildService
    {
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ChildService(IChildRepository childRepository, IUserRepository userRepository, IMapper mapper)
        {
            _childRepository = childRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ChildResponse> CreateChildAsync(ChildCreateRequest request, Guid psychologistId)
        {
            // Validate that parents exist
            var primaryParent = await _userRepository.GetParentByUserIdAsync(request.PrimaryParentId);
            if (primaryParent == null)
                throw new ArgumentException("Responsável principal não encontrado");

            Parent? secondaryParent = null;
            if (request.SecondaryParentId.HasValue)
            {
                secondaryParent = await _userRepository.GetParentByUserIdAsync(request.SecondaryParentId.Value);
                if (secondaryParent == null)
                    throw new ArgumentException("Responsável secundário não encontrado");
            }

            var child = new Child
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Diagnosis = request.Diagnosis,
                AssignedPsychologistId = psychologistId,
                PrimaryParentId = primaryParent.ParentId,
                SecondaryParentId = secondaryParent?.ParentId,
                MedicalHistory = request.MedicalHistory
            };

            await _childRepository.AddAsync(child);
            await _childRepository.SaveChangesAsync();

            var createdChild = await _childRepository.GetWithDetailsAsync(child.ChildId);
            return MapToChildResponse(createdChild!);
        }

        public async Task<ChildResponse?> GetChildByIdAsync(Guid childId, Guid userId, string userRole)
        {
            if (!await CanAccessChildAsync(childId, userId, userRole))
                return null;

            var child = await _childRepository.GetWithDetailsAsync(childId);
            return child != null ? MapToChildResponse(child) : null;
        }

        public async Task<IEnumerable<ChildResponse>> GetChildrenAsync(Guid userId, string userRole)
        {
            IEnumerable<Child> children;

            if (userRole == "Psychologist")
            {
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);
                if (psychologist == null)
                    throw new ArgumentException("Psicólogo não encontrado");

                children = await _childRepository.GetByPsychologistIdAsync(psychologist.PsychologistId);
            }
            else if (userRole == "Parent")
            {
                var parent = await _userRepository.GetParentByUserIdAsync(userId);
                if (parent == null)
                    throw new ArgumentException("Responsável não encontrado");

                children = await _childRepository.GetByParentIdAsync(parent.ParentId);
            }
            else
            {
                throw new UnauthorizedAccessException("Acesso não autorizado");
            }

            return children.Select(MapToChildResponse).ToList();
        }

        public async Task<ChildResponse?> UpdateChildAsync(Guid childId, ChildUpdateRequest request, Guid psychologistId)
        {
            var child = await _childRepository.GetWithDetailsAsync(childId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                return null;

            // Update only provided fields
            if (!string.IsNullOrEmpty(request.FirstName))
                child.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName))
                child.LastName = request.LastName;

            if (request.DateOfBirth.HasValue)
                child.DateOfBirth = request.DateOfBirth.Value;

            if (!string.IsNullOrEmpty(request.Gender))
                child.Gender = request.Gender;

            if (request.Diagnosis != null)
                child.Diagnosis = request.Diagnosis;

            if (request.MedicalHistory != null)
                child.MedicalHistory = request.MedicalHistory;

            if (request.PrimaryParentId.HasValue)
            {
                var primaryParent = await _userRepository.GetParentByUserIdAsync(request.PrimaryParentId.Value);
                if (primaryParent != null)
                    child.PrimaryParentId = primaryParent.ParentId;
            }

            if (request.SecondaryParentId.HasValue)
            {
                var secondaryParent = await _userRepository.GetParentByUserIdAsync(request.SecondaryParentId.Value);
                if (secondaryParent != null)
                    child.SecondaryParentId = secondaryParent.ParentId;
            }

            if (request.IsActive.HasValue)
                child.IsActive = request.IsActive.Value;

            _childRepository.Update(child);
            await _childRepository.SaveChangesAsync();

            var updatedChild = await _childRepository.GetWithDetailsAsync(childId);
            return MapToChildResponse(updatedChild!);
        }

        public async Task<bool> DeleteChildAsync(Guid childId, Guid psychologistId)
        {
            var child = await _childRepository.GetByIdAsync(childId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                return false;

            // Soft delete - just mark as inactive
            child.IsActive = false;
            _childRepository.Update(child);
            await _childRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CanAccessChildAsync(Guid childId, Guid userId, string userRole)
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

        private ChildResponse MapToChildResponse(Child child)
        {
            return new ChildResponse
            {
                ChildId = child.ChildId,
                FirstName = child.FirstName,
                LastName = child.LastName,
                DateOfBirth = child.DateOfBirth,
                Gender = child.Gender,
                Diagnosis = child.Diagnosis,
                OnboardingDate = child.OnboardingDate,
                IsActive = child.IsActive,
                AssignedPsychologistId = child.AssignedPsychologistId,
                PsychologistName = $"{child.AssignedPsychologist.User.FirstName} {child.AssignedPsychologist.User.LastName}",
                PrimaryParent = new ParentInfo
                {
                    ParentId = child.PrimaryParent.ParentId,
                    FirstName = child.PrimaryParent.User.FirstName,
                    LastName = child.PrimaryParent.User.LastName,
                    Email = child.PrimaryParent.User.Email,
                    ContactNumber = child.PrimaryParent.User.ContactNumber,
                    ChildRelationship = child.PrimaryParent.ChildRelationship
                },
                SecondaryParent = child.SecondaryParent != null ? new ParentInfo
                {
                    ParentId = child.SecondaryParent.ParentId,
                    FirstName = child.SecondaryParent.User.FirstName,
                    LastName = child.SecondaryParent.User.LastName,
                    Email = child.SecondaryParent.User.Email,
                    ContactNumber = child.SecondaryParent.User.ContactNumber,
                    ChildRelationship = child.SecondaryParent.ChildRelationship
                } : null,
                MedicalHistory = child.MedicalHistory,
                TotalSessions = child.Sessions?.Count ?? 0,
                TotalAssessments = child.Assessments?.Count ?? 0,
                LastSessionDate = child.Sessions?.OrderByDescending(s => s.SessionDate).FirstOrDefault()?.SessionDate,
                LastAssessmentDate = child.Assessments?.OrderByDescending(a => a.AssessmentDate).FirstOrDefault()?.AssessmentDate
            };
        }
    }
}