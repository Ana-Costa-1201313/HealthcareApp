using Microsoft.AspNetCore.Mvc;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Shared;
using Microsoft.AspNetCore.Cors;

namespace Backoffice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly StaffService _service;

        private readonly AuthService _authService;

        public StaffController(StaffService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StaffDto>>> GetAll(
            [FromQuery] string name,
            [FromQuery] string email,
            [FromQuery] string specialization,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 5
        )
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor", "Nurse", "Technician" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            List<StaffDto> staffList;

            if (name != null || email != null || specialization != null)
            {
                staffList = await _service.FilterStaffAsync(name, email, specialization, pageNum, pageSize);
            }
            else
            {
                staffList = await _service.GetAllAsync(pageNum, pageSize);
            }

            if (staffList == null || staffList.Count == 0)
            {
                return NoContent();
            }

            return Ok(staffList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetById(Guid id)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            StaffDto staff = await _service.GetByIdAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        [HttpPost]
        public async Task<ActionResult<StaffDto>> Create(CreateStaffDto dto)
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
                StaffDto staff = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = staff.Id }, staff);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<StaffDto>> Deactivate(Guid id)
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
                StaffDto staff = await _service.Deactivate(id);

                if (staff == null)
                {
                    return NotFound(new { Message = "There's no Staff with that ID!" });
                }

                return Ok(staff);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("{id}/hard")]
        public async Task<ActionResult<StaffDto>> Delete(Guid id)
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
                StaffDto staff = await _service.Delete(id);

                if (staff == null)
                {
                    return NotFound(new { Message = "There's no Staff with that ID!" });
                }

                return Ok(staff);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StaffDto>> Update(Guid id, EditStaffDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (id != dto.Id)
            {
                return BadRequest(new { Message = "The staff Id does not match the header!" });
            }

            try
            {
                StaffDto staff = await _service.UpdateAsync(dto, false);

                if (staff == null)
                {
                    return NotFound(new { Message = "There's no Staff with that ID!" });
                }

                return Ok(staff);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<StaffDto>> PartialUpdate(Guid id, EditStaffDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (id != dto.Id)
            {
                return BadRequest(new { Message = "The staff Id does not match the header!" });
            }

            try
            {
                StaffDto staff = await _service.UpdateAsync(dto, true);

                if (staff == null)
                {
                    return NotFound(new { Message = "There's no Staff with that ID!" });
                }

                return Ok(staff);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("totalRecords")]
        public async Task<ActionResult<int>> GetTotalRecords()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            int totalRecords;

            totalRecords = await _service.GetTotalRecords();

            return Ok(totalRecords);
        }
    }
}