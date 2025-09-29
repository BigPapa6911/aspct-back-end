// Controllers/ParentsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Helpers;

namespace aspcts_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ParentsController : Controller
{
    private readonly IParentService _parentService;
    private readonly ILogger<ParentsController> _logger;
    
    public ParentsController(IParentService parentService, ILogger<ParentsController> logger)
    {
        _parentService = parentService;
        _logger = logger;
    }
    
    [HttpGet("get-id-by-email")]
    [Authorize(Roles = "Psychologist")]
    public async Task<ActionResult> GetParentIdByEmail([FromQuery] string email)
    {
        try
        {
            // Validação de entrada
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email é obrigatório" });
            }
            
            if (!IsValidEmail(email))
            {
                return BadRequest(new { message = "Formato de email inválido" });
            }
            
            // Buscar responsável usando o service
            var parent = await _parentService.FindParentByEmailAsync(email);
            
            if (parent == null)
            {
                _logger.LogWarning(
                    "Parent search failed for email {Email} by psychologist {PsychologistId}",
                    email, 
                    User.GetUserId()
                );
                
                return NotFound(new { message = "Responsável não encontrado" });
            }
            
            // Log para auditoria (sem dados sensíveis)
            _logger.LogInformation(
                "Psychologist {PsychologistId} successfully found parent {ParentId} by email search",
                User.GetUserId(),
                parent.ParentId
            );
            
            // Retornar dados necessários para o cadastro
            return Ok(new 
            { 
                parentId = parent.ParentId,
                firstName = parent.FirstName,
                lastName = parent.LastName,
                email = parent.Email,
                relationship = parent.ChildRelationship,
                fullName = parent.FullName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error searching parent by email {Email} for psychologist {PsychologistId}",
                email,
                User.GetUserId()
            );
            
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
    
    [HttpGet("search")]
    [Authorize(Roles = "Psychologist")]
    public async Task<ActionResult> SearchParents([FromQuery] string? query = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                return BadRequest(new { message = "Termo de busca deve ter pelo menos 3 caracteres" });
            }
            
            var parents = await _parentService.SearchParentsAsync(query);
            
            _logger.LogInformation(
                "Psychologist {PsychologistId} searched parents with query '{Query}', found {ResultCount} results",
                User.GetUserId(),
                query,
                parents.Count()
            );
            
            return Ok(new { parents });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error searching parents with query '{Query}' for psychologist {PsychologistId}",
                query,
                User.GetUserId()
            );
            
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
    
    [HttpGet("{parentId}/info")]
    [Authorize(Roles = "Psychologist")]
    public async Task<ActionResult> GetParentInfo(Guid parentId)
    {
        try
        {
            var parentInfo = await _parentService.GetParentInfoAsync(parentId);
            
            if (parentInfo == null)
            {
                return NotFound(new { message = "Responsável não encontrado" });
            }
            
            return Ok(parentInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting parent info for parentId {ParentId} by psychologist {PsychologistId}",
                parentId,
                User.GetUserId()
            );
            
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
    
    private static bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }
}