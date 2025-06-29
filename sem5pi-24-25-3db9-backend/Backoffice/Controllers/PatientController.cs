using Backoffice.Domain.Patients;
using Backoffice.Domain.Shared;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Backoffice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [EnableCors("CORSPolicy")]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _service;
        private readonly AuthService _authService;

        public PatientController(PatientService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        //Get: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Patient" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var list = await _service.GetAllAsync();

            if (list == null || !list.Any())
                return NoContent();  // codigo 204 caso nao haja nada


            return Ok(list); // codigo 200 sucesso
        }

        // GET: api/Patient/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetById(Guid id)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); //Erro na validação erro 400 
            }

            var patient = await _service.GetByIdAsync(id);

            if (patient == null)
            {
                return NotFound(); // Erro 404
            }

            return Ok(patient);  // sucesso 200
        }

        //Criar um patient profile
        [HttpPost]
        public async Task<ActionResult<PatientDto>> Create(CreatePatientDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); //Erro na validação erro 400 
            }

            try
            {
                var medicalRecordNumber = await _service.GenerateNextMedicalRecordNumber();

                var patient = await _service.AddAsync(dto, medicalRecordNumber);
                //Codigo 201 -> Created
                return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);

            }
            catch (BusinessRuleValidationException e)
            {
                //Codigo 400 erro de validação 
                return BadRequest(new { Message = e.Message });
            }
        }

        //Dar update PUT: api/Patients
        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDto>> Update(Guid id, EditPatientDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); //Erro na validação erro 400 
            }

            try
            {
                var updatedPatient = await _service.UpdateAsync(id, dto);

                if (updatedPatient == null)
                {
                    return NotFound(); //Erro 404 caso o patient profile nao tenha sido encontrado
                }
                return Ok(updatedPatient); // Sucesso
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message }); // Erro 400 caso haja uma exceção na regra de negocio ou seja na alteração de algum dado
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<PatientDto>> Patch(Guid id, EditPatientDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); //Erro na validação erro 400 
            }

            try
            {
                var updatedPatient = await _service.PatchAsync(id, dto);
                if (updatedPatient == null)
                {
                    return NotFound(); // Erro 404, nao foi encontrado
                }
                return Ok(updatedPatient); // Sucesso 200 e o patient profile updated 

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message }); // Erro 400, erro na validação dos dados
            }
        }

        [HttpPatch("patient/{id}")]
        public async Task<ActionResult<PatientDto>> PatchAsPatient(Guid id, EditPatientAsPatientDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Patient" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); //Erro na validação erro 400 
            }

            try
            {
                var updatedPatient = await _service.PatchAsPatientAsync(id, dto);
                if (updatedPatient == null)
                {
                    return NotFound(); // Erro 404, nao foi encontrado
                }
                return Ok(updatedPatient); // Sucesso 200 e o patient profile updated 

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message }); // Erro 400, erro na validação dos dados
            }
        }

        [HttpGet("SearchByVariousAttributes")]
        public async Task<ActionResult<List<SearchPatientDto>>> SearchPatients(
            //[FromQuery] usado para ir buscar à query o que existe la porque como todos sao opcionais podem existir ou nao 
            [FromQuery] string name = null,
            [FromQuery] string email = null,
            [FromQuery] DateTime? dateOfBirth = null, // ? para permitir que seja null 
            [FromQuery] string medicalRecordNumber = null
        )
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Patient" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var patients = await _service.SearchPatientsAsync(name, email, dateOfBirth, medicalRecordNumber);

            if (patients == null || patients.Count == 0)
                return NotFound("No patients found using the attributes given");
            return Ok(patients);
        }



        //Dar Delete a um patient profile
        [HttpDelete("{id}")]
        public async Task<ActionResult<PatientDto>> HardDelete(Guid id)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); //Erro na validação erro 400 
            }

            try
            {
                var patient = await _service.DeleteAsync(id);

                if (patient == null)
                    return NotFound(); //404 se o patient profile nao foi encontrado

                return Ok(patient); // 202 e o patient profile caso seja um sucesso
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message }); // 400 se houver um erro de validação
            }

        }

        // GET: api/Patient/download-medical-history-json/{email}
        [HttpGet("download-medical-history-json/{email}")]
        public async Task<ActionResult> DownloadMedicalHistoryJson(string email)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Patient" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {

                var json = await _service.createMedicalHistoryJson(email);

                return File(json, "application/json", "medical-history.json");

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }

        }

        // GET: api/Patient/download-medical-history-xml/{email}
        [HttpGet("download-medical-history-xml/{email}")]
        public async Task<ActionResult> DownloadMedicalHistoryXml(string email)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Patient" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {

                var xml = await _service.createMedicalHistoryXml(email);

                return File(xml, "application/xml", "medical-history.xml");

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        //Request patient data deletion
        [HttpPost("request-data-deletion")]
        public async Task<ActionResult<PatientDto>> RequestDataDeletion([FromBody] DataDeletionPatientRequestDto dto)
        {
            try
            {
                await _authService.IsAuthorized(Request, new List<string> { "Admin", "Patient" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {

                var patient = await _service.RequestPatientDataDeletion(dto);

                return Ok("Data deletion request from patient " + patient.Email + " sent to DPO.");

            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}