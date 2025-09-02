using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Child;
using aspcts_backend.Helpers;

namespace aspcts_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChildrenController : Controller
    {
        private readonly IChildService _childService;
        private readonly IUserRepository _userRepository;

        public ChildrenController(IChildService childService, IUserRepository userRepository)
        {
            _childService = childService;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChildResponse>>> GetChildren()
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var children = await _childService.GetChildrenAsync(userId, userRole);
                return Ok(children);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChildResponse>> GetChild(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var child = await _childService.GetChildByIdAsync(id, userId, userRole);
                if (child == null)
                    return NotFound(new { message = "Criança não encontrada ou acesso negado" });

                return Ok(child);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult<ChildResponse>> CreateChild([FromBody] ChildCreateRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var child = await _childService.CreateChildAsync(request, psychologist.PsychologistId);
                return CreatedAtAction(nameof(GetChild), new { id = child.ChildId }, child);
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
        public async Task<ActionResult<ChildResponse>> UpdateChild(Guid id, [FromBody] ChildUpdateRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var child = await _childService.UpdateChildAsync(id, request, psychologist.PsychologistId);
                if (child == null)
                    return NotFound(new { message = "Criança não encontrada ou acesso negado" });

                return Ok(child);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Psychologist")]
        public async Task<ActionResult> DeleteChild(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);

                if (psychologist == null)
                    return BadRequest(new { message = "Psicólogo não encontrado" });

                var success = await _childService.DeleteChildAsync(id, psychologist.PsychologistId);
                if (!success)
                    return NotFound(new { message = "Criança não encontrada ou acesso negado" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/can-access")]
        public async Task<ActionResult<bool>> CanAccessChild(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                var canAccess = await _childService.CanAccessChildAsync(id, userId, userRole);
                return Ok(new { canAccess });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}