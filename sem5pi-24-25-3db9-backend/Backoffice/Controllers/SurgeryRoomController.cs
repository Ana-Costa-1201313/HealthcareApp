using Microsoft.AspNetCore.Mvc;
using Backoffice.Domain.Shared;
using Backoffice.Domain.SurgeryRooms;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backoffice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class SurgeryRoomController : ControllerBase
    {
        private readonly SurgeryRoomService _service;
        private readonly AuthService _authService;
        public SurgeryRoomController(SurgeryRoomService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurgeryRoomDto>>> GetAll()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var SurgeryRooms = await _service.GetAllAsync();

            if (SurgeryRooms == null || SurgeryRooms.Count == 0)
            {
                return NoContent();
            }

            return Ok(SurgeryRooms);
        }

        [HttpGet("{number}")]
        public async Task<ActionResult<SurgeryRoomDto>> GetByNumber(int number)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var SurgeryRoom = await _service.GetByNumberAsync(number);

            if (SurgeryRoom == null)
            {
                return NotFound();
            }

            return Ok(SurgeryRoom);
        }/*

        [HttpPost]
        public async Task<ActionResult<SurgeryRoomDto>> Create(CreateSurgeryRoomDto SurgeryRoomDto)
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
                var SurgeryRoom = await _service.CreateAsync(SurgeryRoomDto);
                return Ok(SurgeryRoom);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            //return null;
        }*/

        // [HttpPost("createRoomType")]
        // public async Task<IActionResult> createRoomType([FromQuery] string tipo, [FromQuery] bool? fitForSurgery, [FromQuery] string description)
        // {
        //     try
        //     {
        //         await _authService.IsAuthorized(Request, new List<string> { "Admin" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        //     if(tipo == null)  return BadRequest("Room Type cannot be null or empty!");

        //     try{
        //         var result = await _service.addRoomType(tipo,fitForSurgery,description);
        //         return Ok(result);
        //     } catch(Exception e){
        //         return BadRequest(e.Message);
        //     }
        // }

        [HttpPost("createRoomType")]
        public async Task<IActionResult> CreateRoomType([FromBody] SurgeryRoomTypeDto roomTypeDto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
    
            if (string.IsNullOrEmpty(roomTypeDto.surgeryRoomType))
                return BadRequest("Room Type cannot be null or empty!");
    
            try
            {
                var result = await _service.addRoomType(roomTypeDto.surgeryRoomType, roomTypeDto.fitForSurgery, roomTypeDto.description);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getAllRoomType")]
        public async Task<ActionResult<List<String>>> getAllRoomType()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var types = await _service.GetAllRoomTypeAsync();

            if (types == null || types.Count == 0)
            {
                return NoContent();
            }

            return Ok(types);
        }

        [HttpGet("getAllRoomTypeDto")]
        public async Task<ActionResult<List<String>>> getAllRoomTypeDto()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var types = await _service.GetAllRoomTypeDtoAsync();

            if (types == null || types.Count == 0)
            {
                return NoContent();
            }

            return Ok(types);
        }
    }
}