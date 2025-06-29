using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Backoffice.Domain.Shared;
using Backoffice.Domain.OperationTypes;

namespace Backoffice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OperationTypesController : ControllerBase
    {
        private readonly OperationTypeService _service;
        private readonly AuthService _authService;

        public OperationTypesController(OperationTypeService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OperationTypeDto>>> GetOperationTypes(
            [FromQuery] string name = null,
            [FromQuery] string specialization = null,
            [FromQuery] bool? status = null
        )
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {

                List<OperationTypeDto> opTypes;

                if (Request.Query.ContainsKey("name") || Request.Query.ContainsKey("specialization") || Request.Query.ContainsKey("status"))
                {
                    opTypes = await _service.FilterOperationTypesAsync(name, specialization, status);
                }
                else
                {
                    opTypes = await _service.GetAllAsync();
                }
                if (opTypes == null || opTypes.Count == 0)
                {
                    return NoContent();
                }

                return Ok(opTypes);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        // GET: api/OperationTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperationTypeDto>> GetGetById(Guid id)
        {

            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var opType = await _service.GetByIdAsync(id);

            if (opType == null)
            {
                return NotFound();
            }

            return Ok(opType);
        }

        // POST: api/OperationTypes
        [HttpPost]
        public async Task<ActionResult<OperationTypeDto>> Create(CreatingOperationTypeDto dto)
        {

            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var opType = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetGetById), new { id = opType.Id }, opType);

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<OperationTypeDto>> PatchOperationType(Guid id, [FromBody] EditOperationTypeDto operationTypeDto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var updatedOperationType = await _service.Patch(id, operationTypeDto);

                if (updatedOperationType == null)
                {
                    return NotFound();
                }

                return Ok(updatedOperationType);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationTypeDto>> UpdateOperationType(Guid id, [FromBody] EditOperationTypeDto updateDto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var updatedOperationType = await _service.Put(id, updateDto);

                if (updatedOperationType == null)
                {
                    return NotFound();
                }

                return Ok(updatedOperationType);

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        // Inactivate: api/OperationTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationTypeDto>> SoftDelete(Guid id)
        {

            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var cat = await _service.InactivateAsync(id);

                if (cat == null)
                {
                    return NotFound();
                }

                return Ok(cat);

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("{id}/hard")]
        public async Task<ActionResult<OperationTypeDto>> HardDelete(Guid id)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                await _service.DeleteAsync(id);

                return Ok();
            }
            catch (BusinessRuleValidationException e)
            {
                return NotFound(new { Message = e.Message }); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message }); 
            }
        }
    }
}