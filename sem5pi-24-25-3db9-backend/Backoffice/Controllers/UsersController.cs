using Microsoft.AspNetCore.Mvc;
using Backoffice.Domain.Users;
using Backoffice.Domain.Shared;
using System.Configuration;
using System.ComponentModel;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Net.Mail;
using Backoffice.Domain.Patients;
using System.Runtime.CompilerServices;

namespace Backoffice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;

        private readonly ExternalApiServices _externalApiService;

        public UsersController(UserService service, ExternalApiServices externalApiService)
        {
            _service = service;
            _externalApiService = externalApiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
        {

            List<String> roles = new List<String> { "Admin" };

            // verificar header -> enviar header token para auth -> confirmar validade -> continuar
            if (!Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                return BadRequest("Authorization header is missing");
            }
            try
            {
                //if(! await checkHeader(roles, tokenHeader)) return BadRequest("User does not autenticated");
                if (!await _externalApiService.checkHeader(roles, tokenHeader)) return BadRequest("User not autenticated");
                //_externalApiService.checkHeader(roles, tokenHeader);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            try
            {
                var user = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
            catch (ConfigurationErrorsException e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost("createPatient")]
        public async Task<ActionResult<UserDto>> CreatePatientUser(CreateUserDto dto)
        {

            List<String> roles = new List<String> { "Patient" };

            try
            {
                var user = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
            catch (ConfigurationErrorsException e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<UserDto>> UpdatePassword(Guid id, [FromQuery] string password)
        {
            try
            {
                var user = await _service.UpdatePassword(id, password);

                if (user == null)
                {
                    return NotFound();
                }

                _service.sendConfirmationEmail(user);

                return Ok(user);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }


        [HttpPost("reset-password")]
        public async Task<ActionResult> RequestPasswordReset(ResetPasswordUserDto dto)
        {
            //Auth not needed
            try
            {
                var user = await _service.SendPasswordResetLink(dto.Email);

                return Ok("Password reset link sent to " + user.Email);

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPatch("new-password")]
        public async Task<ActionResult> ResetPassword([FromQuery] string token, [FromBody] NewPasswordUserDto dto)
        {
            //Auth not needed
            try
            {
                var user = await _service.NewPassword(token, dto.Password);

                return Ok("Password from " + user.Email + " was changed successfully.");

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPut("createPatient")]
        public async Task<ActionResult<UserDto>> CreatePatient([FromBody]CreatePatientRequestDto createPatientRequestDto)
        {
            UserDto user = null;
            try
            {
                user = await _service.createPatient(createPatientRequestDto);

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch(BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message }); //400
            }
        }

        [HttpGet("deletePatientRequest")]
        public async Task<ActionResult<UserDto>> deletePatientRequest([FromBody] UserDto userDto)
        {
            await _service.askConsentDeletePatientUserAsync(userDto);

            return userDto;
        }

        [HttpDelete("deletePatientRequest2")]
        public async Task<ActionResult> deletePatientRequest2([FromQuery] String email)
        {
            UserDto userDto = new UserDto();
            userDto.Email = email;
            await _service.askConsentDeletePatientUserAsync(userDto);

            return NoContent();
        }

        [HttpDelete("deletePatient")]
        public async Task<ActionResult<UserDto>> deletePatient([FromQuery] String email)
        {
            if (await _service.deletePatientUserAsync(email)) return Ok(null);

            else return BadRequest();
        }

    }
}