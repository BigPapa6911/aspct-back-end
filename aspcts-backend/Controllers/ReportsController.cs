using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Models.DTOs.Report;
using aspcts_backend.Helpers;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IUserRepository _userRepository;

        public ReportsController(IReportService reportService, IUserRepository userRepository)
        {
            _reportService = reportService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Buscar relatório por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportResponse>> GetReport(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var report = await _reportService.GetReportByIdAsync(id, userId, userRole);
                if (report == null)
                    return NotFound(new { message = "Relatório não encontrado ou acesso negado" });

                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Buscar relatórios de uma criança
        /// </summary>
        [HttpGet("child/{childId}")]
        public async Task<ActionResult<IEnumerable<ReportResponse>>> GetReportsByChild(Guid childId)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var reports = await _reportService.GetReportsByChildIdAsync(childId, userId, userRole);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Criar novo relatório
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<ReportResponse>> CreateReport([FromBody] ReportRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var report = await _reportService.CreateReportAsync(request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetReport), new { id = report.ReportId }, report);
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
        /// Compartilhar/descompartilhar relatório com pais
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

                var success = await _reportService.ShareWithParentAsync(id, share, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Relatório não encontrado ou acesso negado" });

                return Ok(new { message = share ? "Relatório compartilhado com os pais" : "Compartilhamento removido" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Adicionar sessões a um relatório existente
        /// </summary>
        [HttpPost("{id}/sessions")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult> AddSessionsToReport(Guid id, [FromBody] List<Guid> sessionIds)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                if (sessionIds == null || !sessionIds.Any())
                    return BadRequest(new { message = "Lista de sessões não pode ser vazia" });

                var success = await _reportService.AddSessionsToReportAsync(id, sessionIds, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Relatório não encontrado ou acesso negado" });

                return Ok(new { message = "Sessões adicionadas ao relatório com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remover sessão de um relatório
        /// </summary>
        [HttpDelete("{id}/sessions/{sessionId}")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult> RemoveSessionFromReport(Guid id, Guid sessionId)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var success = await _reportService.RemoveSessionFromReportAsync(id, sessionId, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Sessão não encontrada no relatório ou acesso negado" });

                return Ok(new { message = "Sessão removida do relatório com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Baixar relatório em PDF
        /// </summary>
        [HttpGet("{id}/pdf")]
        public async Task<ActionResult> DownloadPdf(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var pdfBytes = await _reportService.GeneratePdfReportAsync(id, userId, userRole);

                return File(pdfBytes, "application/pdf", $"relatorio_{id}.pdf");
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