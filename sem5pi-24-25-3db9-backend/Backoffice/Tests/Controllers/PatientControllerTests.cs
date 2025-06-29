using System.Diagnostics;
using Azure.Core.Pipeline;
using Backoffice.Controllers;
using Backoffice.Domain.Logs;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Backoffice.Tests
{
    public class PatientControllerTests
    {
        Mock<IPatientRepository> _repo;
        Mock<IUnitOfWork> _unitOfWork;
        Mock<ILogRepository> _logRepo;
        Mock<IExternalApiServices> _mockExternal;
        Mock<AuthService> _mockAuthService;
        PatientService patientService;
        PatientController patientController;
        Mock<IEmailService> emailService;
        Mock<IConfiguration> configuration;

        private void Setup()
        {
            _repo = new Mock<IPatientRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _logRepo = new Mock<ILogRepository>();

            _mockExternal = new Mock<IExternalApiServices>();
            _mockAuthService = new Mock<AuthService>(_mockExternal.Object);

            emailService = new Mock<IEmailService>();
            configuration = new Mock<IConfiguration>();
            patientService = new PatientService(_unitOfWork.Object, _repo.Object, new PatientMapper(), _logRepo.Object, emailService.Object, configuration.Object);

            patientController = new PatientController(patientService, _mockAuthService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer someToken";
            patientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }


        [Fact]
        public async Task GetAllPatients()
        {

            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                       .ReturnsAsync(true);

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Jeremy",
                LastName = "Doku",
                FullName = "Jeremy Doke",
                Gender = "M",
                DateOfBirth = new DateTime(2002, 5, 27),
                Email = "jeremyDoku@gmail.com",
                Phone = "929888777",
                EmergencyContact = "929111222"
            };
            var patientDto2 = new CreatePatientDto
            {
                FirstName = "Savio",
                LastName = "Moreira",
                FullName = "Savio Moreira",
                Gender = "M",
                DateOfBirth = new DateTime(2004, 4, 10),
                Email = "savinho@gmail.com",
                Phone = "999888203",
                EmergencyContact = "919111202"
            };

            var patient1 = new Patient(patientDto1, "202310000001");
            var patient2 = new Patient(patientDto2, "202310000002");

            var list = new List<Patient> { patient1, patient2 };

            _repo.Setup(repo => repo.GetAllWithDetailsAsync()).ReturnsAsync(list);

            var result = await patientController.GetAll();


            var objResult = Assert.IsType<OkObjectResult>(result.Result);
            var listFinal = Assert.IsAssignableFrom<List<PatientDto>>(objResult.Value);

            Assert.Equal(list.Count, listFinal.Count);

        }

        [Fact]
        public async Task GetAllNoPatients()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                       .ReturnsAsync(true);


            var expectedList = new List<Patient> { };
            _repo.Setup(repo => repo.GetAllWithDetailsAsync()).ReturnsAsync(expectedList);

            var result = await patientController.GetAll();

            var noPatients = Assert.IsType<NoContentResult>(result.Result);
        }
        [Fact]
        public async Task GetAllWithoutAuthorization()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await patientController.GetAll();
            var noAuth = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", noAuth.Value);
        }

        [Fact]
        public async Task GetByIdAsync()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Kevin",
                LastName = "DeBruyne",
                FullName = "Kevin DeBruyne",
                Gender = "M",
                DateOfBirth = new DateTime(1991, 6, 28),
                Email = "kevinDeBruyne@gmail.com",
                Phone = "929888771",
                EmergencyContact = "929111211"
            };

            var patient = new Patient(patientDto1, "200001000912");
            _repo.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<PatientId>())).ReturnsAsync(patient);

            var result = await patientController.GetById(patient.Id.AsGuid());

            var okResult = Assert.IsType<ActionResult<PatientDto>>(result);
            var objResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var value = Assert.IsType<PatientDto>(objResult.Value);

            Assert.Equal("Kevin DeBruyne", value.FullName);
            Assert.Equal("M", value.Gender);
            Assert.Equal(new DateTime(1991, 6, 28), value.DateOfBirth);
            Assert.Equal("kevinDeBruyne@gmail.com", value.Email);
            Assert.Equal("929888771", value.Phone);
            Assert.Equal("200001000912", value.MedicalRecordNumber);

        }
        [Fact]
        public async Task GetByIdReturnNotFound()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var patientId = Guid.NewGuid();

            _repo.Setup(repo => repo.GetByIdAsync(It.IsAny<PatientId>())).ReturnsAsync((Patient)null);
            var result = await patientController.GetById(patientId);

            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task GetByIdBadRequest()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            _repo.Setup(repo => repo.GetByIdAsync(It.IsAny<PatientId>())).ReturnsAsync((Patient)null);

            var result = await patientController.GetById(Guid.NewGuid());
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequest.Value);
        }
        [Fact]
        public async Task CreateBadRequestAuth()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                        .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Oscar",
                LastName = "Bobb",
                FullName = "Oscar Bobb",
                Gender = "M",
                DateOfBirth = new DateTime(2003, 7, 12),
                Email = "oscarBobb@gmail.com",
                Phone = "919870011",
                EmergencyContact = "934171011"
            };

            var result = await patientController.Create(patientDto1);
            var badRequestauth = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestauth.Value);
        }
        [Fact]
        public async Task CreateBadRequestInvalidInput()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                        .ReturnsAsync(true);

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Mateo",
                LastName = "Kovacic",
                FullName = "Mateo Kovacic",
                Gender = "M",
                DateOfBirth = new DateTime(1994, 5, 6),
                Email = "mateoKovacic@gmail.com",
                Phone = "919870010",
                EmergencyContact = "932171011"
            };
            _repo.Setup(repo => repo.AddAsync(It.IsAny<Patient>())).ThrowsAsync(new BusinessRuleValidationException("Error: This email is already in use !!!"));

            var result = await patientController.Create(patientDto1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var messageBadRequest = badRequest.Value as dynamic;
            Assert.Equal("Error: This email is already in use !!!", (string)messageBadRequest.Message);
        }
        [Fact]
        public async Task CreateWithValidInput()
        {

            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                          .ReturnsAsync(true);

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Rodri",
                LastName = "Hernandez",
                FullName = "Rodri Hernandez",
                Gender = "M",
                DateOfBirth = new DateTime(1996, 6, 22),
                Email = "rodriBallonDOr@gmail.com",
                Phone = "919871010",
                EmergencyContact = "933171011"
            };


            var patient = new Patient(patientDto1, "202411000001");
            _repo.Setup(repo => repo.AddAsync(It.IsAny<Patient>())).ReturnsAsync(patient);

            _logRepo.Setup(repo => repo.AddAsync(It.IsAny<Log>()))
                    .ReturnsAsync(new Mock<Log>().Object);

            var result = await patientController.Create(patientDto1);
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<PatientDto>(actionResult.Value);

            Assert.NotNull(value);
            Assert.Equal("Rodri Hernandez", value.FullName);
            Assert.Equal("M", value.Gender);
            Assert.Equal(new DateTime(1996, 6, 22), value.DateOfBirth);
            Assert.Equal("rodriBallonDOr@gmail.com", value.Email);
            Assert.Equal("919871010", value.Phone);
        }

        [Fact]
        public async Task UpdateWithValidInput()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                       .ReturnsAsync(true);
            var existingPatient = new Patient(new CreatePatientDto
            {
                FirstName = "Ederson",
                LastName = "Moraes",
                FullName = "Ederson Moraes",
                Gender = "M",
                DateOfBirth = new DateTime(1993, 7, 17),
                Email = "edersonMoraes@gmail.com",
                Phone = "919888771",
                EmergencyContact = "934111222"
            }, "202012000001");

            _repo.Setup(repo => repo.GetByIdAsync(existingPatient.Id)).ReturnsAsync(existingPatient);

            var editPatientDto1 = new EditPatientDto
            {
                FirstName = "Nathan",
                LastName = "Ake",
                FullName = "Nathan Ake",
                Email = "nathanAke@gmail.com",
                Phone = "919191910",
                EmergencyContact = "929292920"
            };

            var result = await patientController.Update(existingPatient.Id.AsGuid(), editPatientDto1);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<PatientDto>(actionResult.Value);

            Assert.NotNull(value);
            Assert.Equal("Nathan Ake", value.FullName);
            Assert.Equal("nathanAke@gmail.com", value.Email);
            Assert.Equal("919191910", value.Phone);
            Assert.Equal(new DateTime(1993, 7, 17), value.DateOfBirth);
            Assert.Equal("202012000001", value.MedicalRecordNumber);

        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPatientDoesNotExist()
        {

            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
            .ReturnsAsync(true);

            var nonExistentPatientId = Guid.NewGuid();

            var editPatientDto = new EditPatientDto
            {
                FirstName = "Nathan",
                LastName = "Ake",
                FullName = "Nathan Ake",
                Email = "nathanAke@gmail.com",
                Phone = "919191910",
                EmergencyContact = "929292920"
            };


            _repo.Setup(repo => repo.GetByIdAsync(It.IsAny<PatientId>())).ReturnsAsync((Patient)null);


            var result = await patientController.Update(nonExistentPatientId, editPatientDto);

            var actionResult = Assert.IsType<NotFoundResult>(result.Result);
        }


        [Fact]
        public async Task Patch_ReturnsOk_WhenDataIsValid()
        {

            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
            .ReturnsAsync(true);


            var existingPatient = new Patient(new CreatePatientDto
            {
                FirstName = "Ederson",
                LastName = "Moraes",
                FullName = "Ederson Moraes",
                Gender = "M",
                DateOfBirth = new DateTime(1993, 7, 17),
                Email = "edersonMoraes@gmail.com",
                Phone = "919888771",
                EmergencyContact = "934111222"
            }, "202012000001");


            _repo.Setup(repo => repo.GetByIdAsync(existingPatient.Id)).ReturnsAsync(existingPatient);

            var editPatientDto = new EditPatientDto
            {
                FirstName = "Nathan",
                LastName = "Ake",
                FullName = "Nathan Ake",
                Email = "nathanAke@gmail.com",
                EmergencyContact = "929292920"
            };


            var result = await patientController.Patch(existingPatient.Id.AsGuid(), editPatientDto);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<PatientDto>(actionResult.Value);

            Assert.NotNull(value);
            Assert.Equal("Nathan Ake", value.FullName);
            Assert.Equal("919888771", value.Phone);
            Assert.Equal("nathanAke@gmail.com", value.Email);

        }
        [Fact]
        public async Task Patch_ReturnsNotFound_WhenPatientDoesNotExist()
        {

            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
            .ReturnsAsync(true);

            var nonExistentPatientId = Guid.NewGuid();

            var editPatientDto = new EditPatientDto
            {
                FirstName = "Nathan",
                LastName = "Ake",
                FullName = "Nathan Ake"

            };


            _repo.Setup(repo => repo.GetByIdAsync(It.IsAny<PatientId>())).ReturnsAsync((Patient)null);


            var result = await patientController.Patch(nonExistentPatientId, editPatientDto);

            var actionResult = Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task Delete_WithoutAuthorization()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await patientController.HardDelete(Guid.NewGuid());
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequest.Value);
        }

        [Fact]
        public async Task Delete_NotFound()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _repo.Setup(repo => repo.GetByIdAsync(It.IsAny<PatientId>())).ReturnsAsync((Patient)null);

            var result = await patientController.HardDelete(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);

        }
        [Fact]
        public async Task DeletePatient()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);
            var patientId = Guid.NewGuid();
            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Matheus",
                LastName = "Nunes",
                FullName = "Matheus Nunes",
                Gender = "M",
                DateOfBirth = new DateTime(1996, 6, 22),
                Email = "matheusNunes@gmail.com",
                Phone = "919271010",
                EmergencyContact = "933121011"
            };
            var patient = new Patient(patientDto1, "200001000001");

            _repo.Setup(repo => repo.GetByIdAsync(patient.Id)).ReturnsAsync(patient);

            var result = await patientController.HardDelete(patient.Id.AsGuid());

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<PatientDto>(actionResult.Value);

            Assert.Equal("Matheus Nunes", value.FullName);
            Assert.Equal("M", value.Gender);
            Assert.Equal("matheusNunes@gmail.com", value.Email);


        }
        [Fact]
        public async Task SearchPatientsByName()
        {
            Setup();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                       .ReturnsAsync(true);

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Joao",
                LastName = "Mario",
                FullName = "Joao Mario",
                Gender = "M",
                DateOfBirth = new DateTime(1993, 1, 19),
                Email = "joaoMarioBenfica@gmail.com",
                Phone = "919800011",
                EmergencyContact = "934111011"
            };
            var patientDto2 = new CreatePatientDto
            {
                FirstName = "Joao",
                LastName = "Mario",
                FullName = "Joao Mario",
                Gender = "M",
                DateOfBirth = new DateTime(2000, 1, 3),
                Email = "joaoMarioPorto@gmail.com",
                Phone = "919800012",
                EmergencyContact = "934111012"
            };
            var patientDto3 = new CreatePatientDto
            {
                FirstName = "Viktor",
                LastName = "Gyokeres",
                FullName = "Viktor Gyokeres",
                Gender = "M",
                DateOfBirth = new DateTime(1998, 6, 4),
                Email = "gyokeresSporting@gmail.com",
                Phone = "919800013",
                EmergencyContact = "934111013"
            };

            var patient1 = new Patient(patientDto1, "199901000001");
            var patient2 = new Patient(patientDto2, "199901000002");
            var patient3 = new Patient(patientDto3, "199901000003");

            var list = new List<Patient> { patient1, patient2, patient3 };
            _repo.Setup(repo => repo.SearchPatientsAsync("Joao", null, null, null)).ReturnsAsync(list.Where(p => p.FirstName == "Joao").ToList());

            var result = await patientController.SearchPatients("Joao", null, null, null);

            var objResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultList = Assert.IsType<List<SearchPatientDto>>(objResult.Value);

            Assert.Equal(2, resultList.Count);
            Assert.Equal("Joao Mario", resultList[0].FullName);
            Assert.Equal("Joao Mario", resultList[1].FullName);
            Assert.NotEqual("Viktor Gyokeres", resultList[0].FullName);
            Assert.NotEqual("Viktor Gyokeres", resultList[1].FullName);
        }

        [Fact]
        public async Task RequestDataDeletion_ReturnsOk_WhenAuthorizedAndSuccessful()
        {

            Setup();

            var dto = new DataDeletionPatientRequestDto
            {
                Email = "patient@example.com"
            };

            var patientDto = new CreatePatientDto
            {
                FirstName = "Viktor",
                LastName = "Gyokeres",
                FullName = "Viktor Gyokeres",
                Gender = "M",
                DateOfBirth = new DateTime(1998, 6, 4),
                Email = "gyokeresSporting@gmail.com",
                Phone = "919800013",
                EmergencyContact = "934111013"
            };
            var patient = new Patient(patientDto, "199901000003");

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            _repo.Setup(repo => repo.GetPatientByEmailAsync(It.IsAny<Email>()))
                     .ReturnsAsync(patient);

            emailService.Setup(emailService => emailService.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(true);

            var result = await patientController.RequestDataDeletion(dto);

            var okResult = Assert.IsType<ActionResult<PatientDto>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.Equal("Data deletion request from patient gyokeresSporting@gmail.com sent to DPO.", objectResult.Value);
        }

        [Fact]
        public async Task RequestDataDeletion_ReturnsException_WithouAuthorization()
        {

            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                                       .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));


            var result = await patientController.RequestDataDeletion(new DataDeletionPatientRequestDto());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task RequestDataDeletion_ReturnsBusinessException_WhenEmailDoesntExist()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            _repo.Setup(repo => repo.GetPatientByEmailAsync(It.IsAny<Email>()))
                     .ReturnsAsync((Patient)null);

            var result = await patientController.RequestDataDeletion(new DataDeletionPatientRequestDto());

            var okResult = Assert.IsType<ActionResult<PatientDto>>(result);
            var objectResult = Assert.IsType<BadRequestObjectResult>(okResult.Result);
            var errorMessage = objectResult.Value.GetType().GetProperty("Message")?.GetValue(objectResult.Value, null);
            Assert.Equal("Error: The email is invalid!", errorMessage);
        }

        [Fact]
        public async Task DownloadMedicalHistoryJson_ReturnsBadRequest_WithoutAuthorization()
        {

            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));


            var result = await patientController.DownloadMedicalHistoryJson("asdas@asdas");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task DownloadMedicalHistoryXml_ReturnsBadRequest_WithoutAuthorization()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));


            var result = await patientController.DownloadMedicalHistoryXml("asdsas@asdas");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }













    }
}