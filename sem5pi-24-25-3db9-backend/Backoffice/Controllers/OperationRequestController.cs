using Microsoft.AspNetCore.Mvc;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;

namespace Backoffice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OperationRequestController : ControllerBase
    {
        private readonly OperationRequestService _service;
        private readonly AuthService _authService;
        public OperationRequestController(OperationRequestService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAll()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var opReqList = await _service.GetAllAsync();

            if (opReqList == null || opReqList.Count == 0)
            {
                return NoContent();
            }

            return Ok(opReqList);
        }

        [HttpGet("doctor/{doctorEmail}")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAllByDoctorEmail(string doctorEmail)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var OperationRequests = await _service.GetAllByDoctorEmailAsync(doctorEmail);

            if (OperationRequests == null || !OperationRequests.Any())
            {
                return NotFound();
            }

            return Ok(OperationRequests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationRequestDto>> GetById(Guid id)
        {
            var OperationRequest = await _service.GetByIdAsync(id);

            if (OperationRequest == null)
            {
                return NotFound();
            }

            return OperationRequest;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetByFilter(
            [FromQuery] Guid doctorId,
            [FromQuery] Guid patientId, 
            [FromQuery] string operationTypeName,
            [FromQuery] string priority,
            [FromQuery] string status)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            List<OperationRequestDto> OperationRequests = new List<OperationRequestDto>();

            try
            {/*
                switch (parameter)
                {
                    case "patient":
                        OperationRequests = await _service.GetAllByPatientIdAsDoctorAsync(doctorId, value);
                        break;
                    case "priority":
                        OperationRequests = await _service.GetAllByPriorityAsDoctorAsync(doctorId, value);
                        break;
                    case "operation type":
                        OperationRequests = await _service.GetAllByOpTypeIdAsDoctorAsync(doctorId, value);
                        break;
                    case "status":
                        OperationRequests = await _service.GetAllByStatusAsDoctorAsync(doctorId, value);
                        break;
                    case "":
                        OperationRequests = await _service.GetAllByDoctorIdAsync(doctorId);
                        break;
                    default:
                        return BadRequest(new { Message = "Invalid parameter!" });
                }*/

                if (Request.Query.ContainsKey("doctorId") || Request.Query.ContainsKey("patientId") || Request.Query.ContainsKey("operationTypeName") || Request.Query.ContainsKey("priority") || Request.Query.ContainsKey("status"))
                {
                    OperationRequests = await _service.FilterOperationRequestsAsync(doctorId, patientId, operationTypeName, priority, status);
                }
                else
                {
                    OperationRequests = await _service.GetAllAsync();
                }

                if (OperationRequests == null || !OperationRequests.Any())
                {
                    return NotFound();
                }

                return Ok(OperationRequests);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("pick/{id}")]
        public async Task<ActionResult<OperationRequestDto>> Pick(Guid id, 
        [FromQuery] List<string> selectedStaffIds)
        {
            if(selectedStaffIds == null || selectedStaffIds.Count == 0)
            {
                return BadRequest(new { Message = "Operation must have selected staff." });
            }

            var OperationRequest = await _service.PickByIdAsync(id, selectedStaffIds);

            if (OperationRequest == null)
            {
                return NotFound();
            }

            return OperationRequest;
        }

        [HttpPost]
        public async Task<ActionResult<OperationRequestDto>> Create(CreateOperationRequestDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Doctor", "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var opRequest = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = opRequest.Id }, opRequest);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("picked")]
        public async Task<ActionResult<OperationRequestDto>> CreatePicked(CreatePickedOperationRequestDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Doctor", "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var opRequest = await _service.AddPickedAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = opRequest.Id }, opRequest);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<OperationRequestDto>> Update(Guid id, [FromBody] EditOperationRequestDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var opRequest = await _service.PatchAsync(id, dto );

                if (opRequest == null)
                {
                    return NotFound();
                }

                return Ok(opRequest);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationRequestDto>> Delete(Guid id)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var opRequest = await _service.DeleteAsync(id);

            if (opRequest == null)
            {
                return NotFound();
            }

            return Ok(opRequest);
        }

        /*

        [HttpGet("doctorGetByPatientEmail/{doctorEmail}/{patientEmail}")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAllByPatientEmailAsDoctor(string doctorEmail, string patientEmail)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            try
            {
                var OperationRequests = await _service.GetAllByPatientEmailAsDoctorAsync(doctorEmail, patientEmail);

                if (OperationRequests == null || !OperationRequests.Any())
                {
                    return NotFound();
                }

                return Ok(OperationRequests);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("doctorGetByPriority/{doctorEmail}/{priority}")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAllByPriorityAsDoctor(string doctorEmail, string priority)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var OperationRequests = await _service.GetAllByPriorityAsDoctorAsync(doctorEmail, priority);

                if (OperationRequests == null || !OperationRequests.Any())
                {
                    return NotFound();
                }

                return Ok(OperationRequests);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("doctorGetByOperationTypeName/{doctorEmail}/{opTypeName}")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAllByOpTypeNameAsDoctor(string doctorEmail, string opTypeName)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            try
            {
                var OperationRequests = await _service.GetAllByOpTypeNameAsDoctorAsync(doctorEmail, opTypeName);

                if (OperationRequests == null || !OperationRequests.Any())
                {
                    return NotFound();
                }

                return Ok(OperationRequests);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("doctorGetByStatus/{doctorEmail}/{status}")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAllByStatusAsDoctor(string doctorEmail, string status)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            try
            {
                var OperationRequests = await _service.GetAllByStatusAsDoctorAsync(doctorEmail, status);

                if (OperationRequests == null || !OperationRequests.Any())
                {
                    return NotFound();
                }

                return Ok(OperationRequests);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }*/
    }
}