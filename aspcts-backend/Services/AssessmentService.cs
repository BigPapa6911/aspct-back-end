using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Assessment;
using aspcts_backend.Models.Entities;
using System.Text.Json;

namespace aspcts_backend.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AssessmentService(
            IAssessmentRepository assessmentRepository,
            IChildRepository childRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _assessmentRepository = assessmentRepository;
            _childRepository = childRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<AssessmentResponse> CreateMilestonesAssessmentAsync(MilestonesAssessmentRequest request, Guid psychologistId)
        {
            // Validate child exists and belongs to psychologist
            var child = await _childRepository.GetByIdAsync(request.ChildId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                throw new ArgumentException("Criança não encontrada ou acesso negado");

            var assessment = new MilestonesAssessment
            {
                ChildId = request.ChildId,
                PsychologistId = psychologistId,
                AssessmentDate = request.AssessmentDate,
                Level1Score = request.Level1Score,
                Level2Score = request.Level2Score,
                Level3Score = request.Level3Score,
                MilestoneScores = JsonSerializer.Serialize(request.MilestoneScores),
                OverallScore = request.MilestoneScores.Values.Sum()
            };

            await _assessmentRepository.AddAsync(assessment);
            await _assessmentRepository.SaveChangesAsync();

            // Fetch with related data
            var assessments = await _assessmentRepository.GetByChildIdAsync(request.ChildId);
            var createdAssessment = assessments.FirstOrDefault(a => a.AssessmentId == assessment.AssessmentId);

            return _mapper.Map<AssessmentResponse>(createdAssessment);
        }

        public async Task<AssessmentResponse> CreateBarriersAssessmentAsync(BarriersAssessmentRequest request, Guid psychologistId)
        {
            var child = await _childRepository.GetByIdAsync(request.ChildId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                throw new ArgumentException("Criança não encontrada ou acesso negado");

            var assessment = new BarriersAssessment
            {
                ChildId = request.ChildId,
                PsychologistId = psychologistId,
                AssessmentDate = request.AssessmentDate,
                BarrierScores = JsonSerializer.Serialize(request.BarrierScores),
                QualitativeNotes = request.QualitativeNotes,
                OverallScore = request.BarrierScores.Values.Sum()
            };

            await _assessmentRepository.AddAsync(assessment);
            await _assessmentRepository.SaveChangesAsync();

            var assessments = await _assessmentRepository.GetByChildIdAsync(request.ChildId);
            var createdAssessment = assessments.FirstOrDefault(a => a.AssessmentId == assessment.AssessmentId);

            return _mapper.Map<AssessmentResponse>(createdAssessment);
        }

        public async Task<AssessmentResponse> CreateTransitionAssessmentAsync(TransitionAssessmentRequest request, Guid psychologistId)
        {
            var child = await _childRepository.GetByIdAsync(request.ChildId);
            if (child == null || child.AssignedPsychologistId != psychologistId)
                throw new ArgumentException("Criança não encontrada ou acesso negado");

            var assessment = new TransitionAssessment
            {
                ChildId = request.ChildId,
                PsychologistId = psychologistId,
                AssessmentDate = request.AssessmentDate,
                TransitionScores = JsonSerializer.Serialize(request.TransitionScores),
                ReadinessNotes = request.ReadinessNotes,
                OverallScore = request.TransitionScores.Values.Sum()
            };

            await _assessmentRepository.AddAsync(assessment);
            await _assessmentRepository.SaveChangesAsync();

            var assessments = await _assessmentRepository.GetByChildIdAsync(request.ChildId);
            var createdAssessment = assessments.FirstOrDefault(a => a.AssessmentId == assessment.AssessmentId);

            return _mapper.Map<AssessmentResponse>(createdAssessment);
        }

        public async Task<IEnumerable<AssessmentResponse>> GetAssessmentsByChildIdAsync(Guid childId, Guid userId, string userRole)
        {
            // Check access permissions
            var canAccess = await CanAccessChild(childId, userId, userRole);
            if (!canAccess)
                return new List<AssessmentResponse>();

            var assessments = await _assessmentRepository.GetByChildIdAsync(childId);
            return assessments.Select(a => _mapper.Map<AssessmentResponse>(a)).ToList();
        }

        public async Task<AssessmentResponse?> GetAssessmentByIdAsync(Guid assessmentId, Guid userId, string userRole)
        {
            var assessments = await _assessmentRepository.GetAllAsync();
            var assessment = assessments.FirstOrDefault(a => a.AssessmentId == assessmentId);

            if (assessment == null)
                return null;

            var canAccess = await CanAccessChild(assessment.ChildId, userId, userRole);
            if (!canAccess)
                return null;

            var assessmentWithDetails = await _assessmentRepository.GetByChildIdAsync(assessment.ChildId);
            var assessmentDetail = assessmentWithDetails.FirstOrDefault(a => a.AssessmentId == assessmentId);

            return _mapper.Map<AssessmentResponse>(assessmentDetail);
        }

        public async Task<Dictionary<string, object>> GetProgressDataAsync(Guid childId, Guid userId, string userRole)
        {
            var canAccess = await CanAccessChild(childId, userId, userRole);
            if (!canAccess)
                return new Dictionary<string, object>();

            var assessments = await _assessmentRepository.GetByChildIdAsync(childId);
            var milestones = assessments.OfType<MilestonesAssessment>().OrderBy(a => a.AssessmentDate);
            var barriers = assessments.OfType<BarriersAssessment>().OrderBy(a => a.AssessmentDate);
            var transitions = assessments.OfType<TransitionAssessment>().OrderBy(a => a.AssessmentDate);

            return new Dictionary<string, object>
            {
                ["milestonesProgress"] = milestones.Select(m => new
                {
                    date = m.AssessmentDate,
                    score = m.OverallScore,
                    level1 = m.Level1Score,
                    level2 = m.Level2Score,
                    level3 = m.Level3Score
                }),
                ["barriersProgress"] = barriers.Select(b => new
                {
                    date = b.AssessmentDate,
                    score = b.OverallScore
                }),
                ["transitionProgress"] = transitions.Select(t => new
                {
                    date = t.AssessmentDate,
                    score = t.OverallScore
                }),
                ["totalAssessments"] = assessments.Count(),
                ["lastAssessmentDate"] = assessments.OrderByDescending(a => a.AssessmentDate).FirstOrDefault()?.AssessmentDate ?? DateTime.MinValue
            };
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