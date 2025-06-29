using Backoffice.Domain.Appointments;
using Backoffice.Domain.Shared;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Backoffice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [EnableCors("CORSPolicy")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _service;
        private readonly AuthService _authService;
        public AppointmentController(AppointmentService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var appointments = await _service.GetAllAsync();

            if (appointments == null || appointments.Count == 0)
            {
                return NoContent();
            }

            return Ok(appointments);
        }

        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<AppointmentWithDetailsDto>>> GetAllWithDetails()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var appointments = await _service.GetAllWithDetailsAsync();

            if (appointments == null || appointments.Count == 0)
            {
                return NoContent();
            }

            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorEmail}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAllAsDoctor(string doctorEmail)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Doctor" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var appointments = await _service.GetAllAsDoctorAsync(doctorEmail);

            if (appointments == null || appointments.Count == 0)
            {
                return NoContent();
            }

            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
        {/*
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/

            var appointment = await _service.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<AppointmentDto>> GetByIdWithDetails(Guid id)
        {/*
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/

            var appointment = await _service.GetByIdWithDetailsAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentWithDetailsDto>> Create(CreateAppointmentDto appointmentDto)
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
                var appointment = await _service.CreateAsync(appointmentDto);
                
                return CreatedAtAction(nameof(GetById), new { id = appointment.AppointmentId }, appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            //return null;
        }

        [HttpPost("planning")]
        public async Task<ActionResult<List<AppointmentDto>>> /* Task<ActionResult<string>> */ CreatePlanning(PlanningDto appointmentsDto)
        {/*
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/

            try
            {
                var dtos = await _service.CreatePlanningAsync(appointmentsDto);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            //return null;
            //return BadRequest();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<AppointmentWithDetailsDto>> Update(Guid id, UpdateAppointmentDto appointmentDto)
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
                var appointment = await _service.UpdateAsync(id, appointmentDto);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            //return null
        }

        [HttpGet("bydate/{date}")]
        public async Task<ActionResult<IEnumerable<AppointmentWithDetailsDto>>> GetByDate(string date)
        {/*
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/
            try
            {
                var appointment = await _service.GetByDateAsync(date);

                if (appointment == null)
                {
                    return NotFound();
                }

                return Ok(appointment);

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new
                {
                    Message = e.Message
                });
            }
        }
    }
}