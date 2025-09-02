using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Models.DTOs.InterventionPlan;
using aspcts_backend.Helpers;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InterventionPlansController : Controller
    {
        private readonly IInterventionPlanService _interventionPlanService;
        private readonly IUserRepository _userRepository;

        public InterventionPlansController(IInterventionPlanService interventionPlanService, IUserRepository userRepository)
        {
            _interventionPlanService = interventionPlanService;
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InterventionPlanResponse>> GetInterventionPlan(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var plan = await _interventionPlanService.GetInterventionPlanByIdAsync(id, userId, userRole);
                if (plan == null)
                    return NotFound(new { message = "Plano de intervenção não encontrado ou acesso negado" });

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("child/{childId}")]
        public async Task<ActionResult<IEnumerable<InterventionPlanResponse>>> GetInterventionPlansByChild(Guid childId)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var plans = await _interventionPlanService.GetInterventionPlansByChildIdAsync(childId, userId, userRole);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("child/{childId}/active")]
        public async Task<ActionResult<InterventionPlanResponse>> GetActiveInterventionPlan(Guid childId)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var plan = await _interventionPlanService.GetActiveInterventionPlanAsync(childId, userId, userRole);
                if (plan == null)
                    return NotFound(new { message = "Nenhum plano de intervenção ativo encontrado" });

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<InterventionPlanResponse>> CreateInterventionPlan([FromBody] InterventionPlanRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var plan = await _interventionPlanService.CreateInterventionPlanAsync(request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetInterventionPlan), new { id = plan.PlanId }, plan);
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

        [HttpPut("{id}")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<InterventionPlanResponse>> UpdateInterventionPlan(Guid id, [FromBody] InterventionPlanRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var plan = await _interventionPlanService.UpdateInterventionPlanAsync(id, request, psychologist.PsychologistId);
                if (plan == null)
                    return NotFound(new { message = "Plano de intervenção não encontrado ou acesso negado" });

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/archive")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult> ArchiveInterventionPlan(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var success = await _interventionPlanService.ArchiveInterventionPlanAsync(id, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Plano de intervenção não encontrado ou acesso negado" });

                return Ok(new { message = "Plano de intervenção arquivado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/goals")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<InterventionGoalResponse>> AddGoal(Guid id, [FromBody] InterventionGoalRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var goal = await _interventionPlanService.AddGoalAsync(id, request, psychologist.PsychologistId);
                return Ok(goal);
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

        [HttpPut("goals/{goalId}")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<InterventionGoalResponse>> UpdateGoal(Guid goalId, [FromBody] InterventionGoalRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var goal = await _interventionPlanService.UpdateGoalAsync(goalId, request, psychologist.PsychologistId);
                if (goal == null)
                    return NotFound(new { message = "Meta não encontrada ou acesso negado" });

                return Ok(goal);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}