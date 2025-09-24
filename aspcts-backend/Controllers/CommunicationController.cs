// Controllers/CommunicationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Models.DTOs.Communication;
using aspcts_backend.Helpers;

namespace aspcts_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommunicationController : ControllerBase
{
    private readonly ICommunicationService _communicationService;
    
    public CommunicationController(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }
    
    [HttpPost("send")]
    public async Task<ActionResult<MessageResponse>> SendMessage([FromBody] MessageRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var message = await _communicationService.SendMessageAsync(request, userId);
            return Ok(message);
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
    
    [HttpGet("child/{childId}")]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessagesByChild(Guid childId)
    {
        try
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();
            
            var messages = await _communicationService.GetMessagesByChildIdAsync(childId, userId, userRole);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("conversation/{otherUserId}/child/{childId}")]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetConversation(Guid otherUserId, Guid childId)
    {
        try
        {
            var userId = User.GetUserId();
            var messages = await _communicationService.GetConversationAsync(otherUserId, childId, userId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("unread")]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetUnreadMessages()
    {
        try
        {
            var userId = User.GetUserId();
            var messages = await _communicationService.GetUnreadMessagesAsync(userId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPatch("{messageId}/read")]
    public async Task<ActionResult> MarkAsRead(Guid messageId)
    {
        try
        {
            var userId = User.GetUserId();
            var success = await _communicationService.MarkAsReadAsync(messageId, userId);
            
            if (!success)
                return NotFound(new { message = "Mensagem não encontrada ou acesso negado" });
            
            return Ok(new { message = "Mensagem marcada como lida" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        try
        {
            var userId = User.GetUserId();
            var count = await _communicationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
