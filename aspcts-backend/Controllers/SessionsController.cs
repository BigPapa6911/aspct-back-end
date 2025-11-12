using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Models.DTOs.Session;
using aspcts_backend.Helpers;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IUserRepository _userRepository;

        public SessionsController(ISessionService sessionService, IUserRepository userRepository)
        {
            _sessionService = sessionService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Buscar sessão por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionResponse>> GetSession(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var session = await _sessionService.GetSessionByIdAsync(id, userId, userRole);
                if (session == null)
                    return NotFound(new { message = "Sessão não encontrada ou acesso negado" });

                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Buscar sessões de uma criança
        /// </summary>
        [HttpGet("child/{childId}")]
        public async Task<ActionResult<IEnumerable<SessionResponse>>> GetSessionsByChild(Guid childId)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var sessions = await _sessionService.GetSessionsByChildIdAsync(childId, userId, userRole);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Criar nova sessão
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<SessionResponse>> CreateSession([FromBody] SessionCreateRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var session = await _sessionService.CreateSessionAsync(request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetSession), new { id = session.SessionId }, session);
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

        /// <summary>
        /// Atualizar sessão
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<SessionResponse>> UpdateSession(Guid id, [FromBody] SessionUpdateRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var session = await _sessionService.UpdateSessionAsync(id, request, psychologist.PsychologistId);
                if (session == null)
                    return NotFound(new { message = "Sessão não encontrada ou acesso negado" });

                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletar sessão
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult> DeleteSession(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var success = await _sessionService.DeleteSessionAsync(id, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Sessão não encontrada ou acesso negado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Compartilhar/descompartilhar sessão com pais
        /// </summary>
        [HttpPatch("{id}/share")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult> ShareWithParent(Guid id, [FromBody] bool share)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var success = await _sessionService.ShareWithParentAsync(id, share, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Sessão não encontrada ou acesso negado" });

                return Ok(new { message = share ? "Sessão compartilhada com os pais" : "Compartilhamento removido" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}