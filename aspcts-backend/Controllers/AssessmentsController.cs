using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Models.DTOs.Assessment;
using aspcts_backend.Helpers;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssessmentsController : Controller
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IUserRepository _userRepository;

        public AssessmentsController(IAssessmentService assessmentService, IUserRepository userRepository)
        {
            _assessmentService = assessmentService;
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentResponse>> GetAssessment(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var assessment = await _assessmentService.GetAssessmentByIdAsync(id, userId, userRole);
                if (assessment == null)
                    return NotFound(new { message = "Avaliação não encontrada ou acesso negado" });

                return Ok(assessment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("child/{childId}")]
        public async Task<ActionResult<IEnumerable<AssessmentResponse>>> GetAssessmentsByChild(Guid childId)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var assessments = await _assessmentService.GetAssessmentsByChildIdAsync(childId, userId, userRole);
                return Ok(assessments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("child/{childId}/progress")]
        public async Task<ActionResult<Dictionary<string, object>>> GetProgressData(Guid childId)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var progressData = await _assessmentService.GetProgressDataAsync(childId, userId, userRole);
                return Ok(progressData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("milestones")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<AssessmentResponse>> CreateMilestonesAssessment([FromBody] MilestonesAssessmentRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var assessment = await _assessmentService.CreateMilestonesAssessmentAsync(request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, assessment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("barriers")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<AssessmentResponse>> CreateBarriersAssessment([FromBody] BarriersAssessmentRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var assessment = await _assessmentService.CreateBarriersAssessmentAsync(request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, assessment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("transition")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<AssessmentResponse>> CreateTransitionAssessment([FromBody] TransitionAssessmentRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var assessment = await _assessmentService.CreateTransitionAssessmentAsync(request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, assessment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}