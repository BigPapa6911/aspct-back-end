using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Models.DTOs.Session;
using aspcts_backend.Helpers;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Controllers
{
    /// <summary>
    /// Controller para gerenciar dados do protocolo ABA de sessões
    /// </summary>
    [ApiController]
    [Route("api/sessions/{sessionId}/protocol")]
    [Authorize]
    public class SessionProtocolController : ControllerBase
    {
        private readonly ISessionProtocolDataService _protocolService;
        private readonly IUserRepository _userRepository;

        public SessionProtocolController(
            ISessionProtocolDataService protocolService,
            IUserRepository userRepository)
        {
            _protocolService = protocolService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Buscar dados do protocolo de uma sessão
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<SessionProtocolDataResponse>> GetProtocolData(Guid sessionId)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var protocolData = await _protocolService.GetBySessionIdAsync(sessionId, userId, userRole);
                if (protocolData == null)
                    return NotFound(new { message = "Dados do protocolo não encontrados" });

                return Ok(protocolData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Criar dados do protocolo para uma sessão
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<SessionProtocolDataResponse>> CreateProtocolData(
            Guid sessionId,
            [FromBody] SessionProtocolDataRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var protocolData = await _protocolService.CreateAsync(sessionId, request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetProtocolData), new { sessionId }, protocolData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualizar dados do protocolo de uma sessão
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<SessionProtocolDataResponse>> UpdateProtocolData(
            Guid sessionId,
            [FromBody] SessionProtocolDataRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var protocolData = await _protocolService.UpdateAsync(sessionId, request, psychologist.PsychologistId);
                if (protocolData == null)
                    return NotFound(new { message = "Dados do protocolo não encontrados ou acesso negado" });

                return Ok(protocolData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletar dados do protocolo de uma sessão
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult> DeleteProtocolData(Guid sessionId)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var success = await _protocolService.DeleteAsync(sessionId, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Dados do protocolo não encontrados ou acesso negado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
