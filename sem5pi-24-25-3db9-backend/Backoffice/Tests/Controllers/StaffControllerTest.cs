using Backoffice.Controllers;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Specializations;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Backoffice.Domain.Logs;


namespace Backoffice.Tests
{
    public class StaffControllerTest
    {
        Mock<IStaffRepository> _repo;
        Mock<ISpecializationRepository> _specRepo;
        Mock<IUnitOfWork> _unitOfWork;
        Mock<ILogRepository> _logRepo;
        Mock<IExternalApiServices> _mockExternal;
        Mock<AuthService> _mockAuthService;
        Mock<StaffService> mockService;
        Mock<StaffController> mockController;
        Mock<IConfiguration> _config;
        Mock<IEmailService> _emailService;


        private void Setup()
        {
            _repo = new Mock<IStaffRepository>();
            _specRepo = new Mock<ISpecializationRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _logRepo = new Mock<ILogRepository>();
            _mockExternal = new Mock<IExternalApiServices>();
            _config = new Mock<IConfiguration>();
            _emailService = new Mock<IEmailService>();
            _mockAuthService = new Mock<AuthService>(_mockExternal.Object);
            mockService = new Mock<StaffService>(_unitOfWork.Object, _repo.Object, new StaffMapper(), _specRepo.Object, _logRepo.Object, _emailService.Object, _config.Object);


            mockController = new Mock<StaffController>(mockService.Object, _mockAuthService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer someToken";

            mockController.Object.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetAllStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto2 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 2,
                Phone = "999999989",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");
            var staff2 = new Staff(dto2, spec, 2, "healthcareapp.com");

            var expectedList = new List<Staff> { staff1, staff2 };

            _repo.Setup(repo => repo.GetAllWithDetailsAsync(1, 5)).ReturnsAsync(expectedList);

            var result = await mockController.Object.GetAll(null, null, null, 1, 5);

            var okResult = Assert.IsType<ActionResult<List<StaffDto>>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualList = Assert.IsAssignableFrom<List<StaffDto>>(objectResult.Value);

            Assert.Equal(expectedList.Count, actualList.Count());
        }

        [Fact]
        public async Task GetAllStaffWithPageSize()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto2 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 2,
                Phone = "999999989",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto3 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 3,
                Phone = "999999979",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto4 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 4,
                Phone = "999999969",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto5 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 5,
                Phone = "999999959",
                Specialization = "heart",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");
            var staff2 = new Staff(dto2, spec, 2, "healthcareapp.com");
            var staff3 = new Staff(dto3, spec, 3, "healthcareapp.com");
            var staff4 = new Staff(dto4, spec, 4, "healthcareapp.com");
            var staff5 = new Staff(dto5, spec, 5, "healthcareapp.com");

            var expectedList = new List<Staff> { staff1, staff2, staff3 };

            _repo.Setup(repo => repo.GetAllWithDetailsAsync(1, 3)).ReturnsAsync(expectedList);

            var result = await mockController.Object.GetAll(null, null, null, 1, 3);

            var okResult = Assert.IsType<ActionResult<List<StaffDto>>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualList = Assert.IsAssignableFrom<List<StaffDto>>(objectResult.Value);

            Assert.Equal(expectedList.Count, actualList.Count());
        }

        [Fact]
        public async Task GetAllAsyncWithPageNumAndPageSize()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto2 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 2,
                Phone = "999999989",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto3 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 3,
                Phone = "999999979",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto4 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 4,
                Phone = "999999969",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto5 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 5,
                Phone = "999999959",
                Specialization = "heart",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");
            var staff2 = new Staff(dto2, spec, 2, "healthcareapp.com");
            var staff3 = new Staff(dto3, spec, 3, "healthcareapp.com");
            var staff4 = new Staff(dto4, spec, 4, "healthcareapp.com");
            var staff5 = new Staff(dto5, spec, 5, "healthcareapp.com");

            var expectedList = new List<Staff> { staff4, staff5 };

            _repo.Setup(repo => repo.GetAllWithDetailsAsync(2, 3)).ReturnsAsync(expectedList);

            var result = await mockController.Object.GetAll(null, null, null, 2, 3);

            var okResult = Assert.IsType<ActionResult<List<StaffDto>>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualList = Assert.IsAssignableFrom<List<StaffDto>>(objectResult.Value);

            Assert.Equal(expectedList.Count, actualList.Count());
        }

        [Fact]
        public async Task GetAllEmpty()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            var expectedList = new List<Staff>();

            _repo.Setup(repo => repo.GetAllWithDetailsAsync(1, 5))
            .ReturnsAsync(expectedList);

            var result = await mockController.Object.GetAll(null, null, null, 1, 5);

            var noContentResult = Assert.IsType<ActionResult<List<StaffDto>>>(result);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }

        [Fact]
        public async Task GetByIdStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            var result = await mockController.Object.GetById(staff.Id.AsGuid());

            var okResult = Assert.IsType<ActionResult<StaffDto>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualStaff = Assert.IsType<StaffDto>(objectResult.Value);

            Assert.Equal(staff.Name, actualStaff.Name);
            Assert.Equal(staff.LicenseNumber.LicenseNum, actualStaff.LicenseNumber);
            Assert.Equal(staff.Specialization.Name.Name, actualStaff.Specialization);
            Assert.Equal(staff.Role, actualStaff.Role);
        }

        [Fact]
        public async Task GetByIdEmpty()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            var staffId = Guid.NewGuid();

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync((Staff)null);

            var result = await mockController.Object.GetById(staffId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            StaffDto createdStaff = new StaffDto
            {
                Id = Guid.NewGuid(),
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
            };

            var result = await mockController.Object.Create(dto1);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<StaffDto>(createdAtActionResult.Value);

            Assert.Equal(201, createdAtActionResult.StatusCode);

            Assert.Equal(createdStaff.Name, returnValue.Name);
            Assert.Equal(createdStaff.Phone, returnValue.Phone);
            Assert.Equal(createdStaff.LicenseNumber, returnValue.LicenseNumber);
            Assert.Equal(createdStaff.Specialization, returnValue.Specialization);
            Assert.Equal(createdStaff.Role, returnValue.Role);
        }

        [Fact]
        public async Task InvalidPhoneCreateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            var result = await mockController.Object.Create(dto1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: The phone number is invalid!", errorMessage);
        }

        [Fact]
        public async Task InvalidRecYearCreateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = -1
            };

            var result = await mockController.Object.Create(dto1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: The year must be bigger than zero!", errorMessage);
        }

        [Fact]
        public async Task NullNameCreateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = null,
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            var result = await mockController.Object.Create(dto1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: The staff must have a name!", errorMessage);
        }

        [Fact]
        public async Task EmptyNameCreateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            var result = await mockController.Object.Create(dto1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: The staff must have a name!", errorMessage);
        }

        [Fact]
        public async Task InvalidRoleCreateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Patient,
                RecruitmentYear = 2024
            };

            var result = await mockController.Object.Create(dto1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: The staff role must be one of the following: Doctor, Nurse or Tech!", errorMessage);
        }

        [Fact]
        public async Task DeleteStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };
            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            var result = await mockController.Object.Deactivate(staff.Id.AsGuid());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<StaffDto>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal("Deactivated Staff", returnValue.Name);
            Assert.Null(returnValue.Phone);
            Assert.Null(returnValue.Specialization);
            Assert.False(returnValue.Active);
        }

        [Fact]
        public async Task InvalidDeleteStaff()
        {

            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync((Staff)null);

            var staffId = Guid.NewGuid();

            var result = await mockController.Object.Deactivate(staffId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteInactiveStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            await mockController.Object.Deactivate(staff.Id.AsGuid());

            var result2 = await mockController.Object.Deactivate(staff.Id.AsGuid());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result2.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: This Staff profile is already deactivated!", errorMessage);
        }

        [Fact]
        public async Task UpdateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff.Id.AsGuid(),
                Phone = "999999991",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            var result = await mockController.Object.Update(staff.Id.AsGuid(), editDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<StaffDto>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal("ana costa", returnValue.Name);
            Assert.Equal("999999991", returnValue.Phone);
            Assert.Equal("skin", returnValue.Specialization);
        }

        [Fact]
        public async Task NullPhoneUpdateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff.Id.AsGuid(),
                Phone = null,
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            var result = await mockController.Object.Update(staff.Id.AsGuid(), editDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: The staff must have a phone number!", errorMessage);
        }

        [Fact]
        public async Task UpdateInactiveStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");


            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff.Id.AsGuid(),
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            await mockController.Object.Deactivate(staff.Id.AsGuid());

            var result = await mockController.Object.Update(staff.Id.AsGuid(), editDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);

            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Error: Can't update an inactive staff!", errorMessage);
        }

        [Fact]
        public async Task UpdateStaffEmpty()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            var staffId = new Guid("846b1792-6a40-4d97-ba61-300c934dddd3");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staffId,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync((Staff)null);

            var result = await mockController.Object.Update(staffId, editDto);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PartialUpdateStaff()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff.Id.AsGuid(),
                Phone = "999999991",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            var result = await mockController.Object.PartialUpdate(staff.Id.AsGuid(), editDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<StaffDto>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal("ana costa", returnValue.Name);
            Assert.Equal("999999991", returnValue.Phone);
            Assert.Equal("skin", returnValue.Specialization);
        }

        [Fact]
        public async Task PartialUpdateStaffSamePhone()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff.Id.AsGuid(),
                Phone = null,
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            var result = await mockController.Object.PartialUpdate(staff.Id.AsGuid(), editDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<StaffDto>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal("ana costa", returnValue.Name);
            Assert.Equal("999999999", returnValue.Phone);
            Assert.Equal("skin", returnValue.Specialization);
        }

        [Fact]
        public async Task PartialUpdateStaffSameSpec()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));


            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff.Id.AsGuid(),
                Phone = "999999999",
                Specialization = null,
                AvailabilitySlots = AvailabilitySlots2
            };

            _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

            var result = await mockController.Object.PartialUpdate(staff.Id.AsGuid(), editDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<StaffDto>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("ana costa", returnValue.Name);
            Assert.Equal("999999999", returnValue.Phone);
            Assert.Equal("skin", returnValue.Specialization);
        }

        // [Fact]
        // public async Task PartialUpdateInactiveStaff()
        // {
        //     Setup();

        //     _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
        //                    .ReturnsAsync(true);

        //     _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
        //                 .ReturnsAsync(new Specialization(new SpecializationName("skin")));


        //     mockService.CallBase = true;
        //     mockController.CallBase = true;

        //     mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

        //     List<string> AvailabilitySlots = new List<string>();
        //     AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
        //     AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

        //     CreateStaffDto dto1 = new CreateStaffDto
        //     {
        //         Name = "ana costa",
        //         LicenseNumber = 1,
        //         Phone = "999999999",
        //         Specialization = "skin",
        //         AvailabilitySlots = AvailabilitySlots,
        //         Role = Role.Nurse,
        //         RecruitmentYear = 2024
        //     };

        //     Specialization spec = new Specialization(new SpecializationName("skin"));

        //     var staff = new Staff(dto1, spec, 1, "healthcareapp.com");

        //     List<string> AvailabilitySlots2 = new List<string>();
        //     AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
        //     AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

        //     EditStaffDto editDto = new EditStaffDto
        //     {
        //         Id = staff.Id.AsGuid(),
        //         Phone = "999999999",
        //         Specialization = "skin",
        //         AvailabilitySlots = AvailabilitySlots2
        //     };

        //     _repo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);

        //     await mockController.Object.Deactivate(staff.Id.AsGuid());

        //     var result = await mockController.Object.PartialUpdate(staff.Id.AsGuid(), editDto);

        //     var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

        //     Assert.Equal(400, badRequestResult.StatusCode);

        //     var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);

        //     Assert.Equal("Error: Can't update an inactive staff!", errorMessage);
        // }

        [Fact]
        public async Task FilterStaffByNameAsync()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));

            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto2 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 2,
                Phone = "999999989",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");
            var staff2 = new Staff(dto2, spec, 2, "healthcareapp.com");

            var expectedList = new List<Staff> { staff1 };

            _repo.Setup(repo => repo.FilterStaffAsync("ana costa", null, null, 1, 5)).ReturnsAsync(expectedList);

            var result = await mockController.Object.GetAll("ana costa", null, null, 1, 5);

            var okResult = Assert.IsType<ActionResult<List<StaffDto>>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualList = Assert.IsAssignableFrom<List<StaffDto>>(objectResult.Value);

            Assert.Equal(expectedList.Count, actualList.Count());
        }

        [Fact]
        public async Task FilterStaffByNameAndPageSizeAsync()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));

            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto2 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 2,
                Phone = "999999989",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto3 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 3,
                Phone = "999999979",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto4 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 4,
                Phone = "999999969",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto5 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 5,
                Phone = "999999959",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto6 = new CreateStaffDto
            {
                Name = "joana silva",
                LicenseNumber = 6,
                Phone = "999999949",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto7 = new CreateStaffDto
            {
                Name = "joana silva",
                LicenseNumber = 7,
                Phone = "999999939",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");
            var staff2 = new Staff(dto2, spec, 2, "healthcareapp.com");
            var staff3 = new Staff(dto3, spec, 3, "healthcareapp.com");
            var staff4 = new Staff(dto4, spec, 4, "healthcareapp.com");
            var staff5 = new Staff(dto5, spec, 5, "healthcareapp.com");
            var staff6 = new Staff(dto6, spec, 6, "healthcareapp.com");
            var staff7 = new Staff(dto7, spec, 7, "healthcareapp.com");

            var expectedList = new List<Staff> { staff2, staff3, staff4 };

            _repo.Setup(repo => repo.FilterStaffAsync("maria silva", null, null, 1, 3)).ReturnsAsync(expectedList);

            var result = await mockController.Object.GetAll("maria silva", null, null, 1, 3);

            var okResult = Assert.IsType<ActionResult<List<StaffDto>>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualList = Assert.IsAssignableFrom<List<StaffDto>>(objectResult.Value);

            Assert.Equal(expectedList.Count, actualList.Count());
        }

        [Fact]
        public async Task FilterStaffBySpecAndPageNum()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            _specRepo.Setup(specRepo => specRepo.GetBySpecializationName(It.IsAny<string>()))
                        .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));

            mockService.CallBase = true;
            mockController.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto2 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 2,
                Phone = "999999989",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto3 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 3,
                Phone = "999999979",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto4 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 4,
                Phone = "999999969",
                Specialization = "heart",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto5 = new CreateStaffDto
            {
                Name = "maria silva",
                LicenseNumber = 5,
                Phone = "999999959",
                Specialization = "heart",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto6 = new CreateStaffDto
            {
                Name = "joana silva",
                LicenseNumber = 6,
                Phone = "999999949",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto7 = new CreateStaffDto
            {
                Name = "joana silva",
                LicenseNumber = 7,
                Phone = "999999939",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");
            Specialization spec2 = new Specialization(new SpecializationName("heart"),"code","description");

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");
            var staff2 = new Staff(dto2, spec, 2, "healthcareapp.com");
            var staff3 = new Staff(dto3, spec, 3, "healthcareapp.com");
            var staff4 = new Staff(dto4, spec2, 4, "healthcareapp.com");
            var staff5 = new Staff(dto5, spec2, 5, "healthcareapp.com");
            var staff6 = new Staff(dto6, spec, 6, "healthcareapp.com");
            var staff7 = new Staff(dto7, spec, 7, "healthcareapp.com");

            var expectedList = new List<Staff> { staff4, staff5 };

            _repo.Setup(repo => repo.FilterStaffAsync(null, null, "heart", 2, 3)).ReturnsAsync(expectedList);

            var result = await mockController.Object.GetAll(null, null, "heart", 2, 3);

            var okResult = Assert.IsType<ActionResult<List<StaffDto>>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualList = Assert.IsAssignableFrom<List<StaffDto>>(objectResult.Value);

            Assert.Equal(expectedList.Count, actualList.Count());
        }
    }
}