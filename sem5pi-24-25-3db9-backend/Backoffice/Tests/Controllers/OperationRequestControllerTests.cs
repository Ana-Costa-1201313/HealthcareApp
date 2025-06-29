using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Backoffice.Controllers;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Logs;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Specializations;

namespace Backoffice.Tests.Controllers
{
    public class OperationRequestControllerTests
    {
        private readonly Mock<IOperationRequestRepository> _mockRepo;
        private readonly Mock<IOperationTypeRepository> _mockOpTypeRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<ILogRepository> _mockLogRepo;
        private readonly Mock<IStaffRepository> _mockStaffRepo;
        private readonly Mock<IExternalApiServices> _mockExternal;
        private readonly Mock<AuthService> _mockAuthService;
        private readonly OperationRequestController _controller;
        private readonly OperationRequestService _service;
        private readonly OperationTypeMapper _opTypeMapper;

        public OperationRequestControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IOperationRequestRepository>();
            _mockOpTypeRepo = new Mock<IOperationTypeRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockStaffRepo = new Mock<IStaffRepository>();
            _mockLogRepo = new Mock<ILogRepository>();
            _opTypeMapper = new OperationTypeMapper();

            _mockExternal = new Mock<IExternalApiServices>();

            _mockAuthService = new Mock<AuthService>(_mockExternal.Object);

            _service = new OperationRequestService(
                _mockUnitOfWork.Object,
                _mockRepo.Object,
                _mockOpTypeRepo.Object,
                _mockPatientRepo.Object,
                _mockStaffRepo.Object,  
                _mockLogRepo.Object,
                _opTypeMapper);

            _controller = new OperationRequestController(_service, _mockAuthService.Object);
        }

        private OperationType CreateMockOperationType()
        {
            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            OperationTypeMapper mapper = new OperationTypeMapper();
            return mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);
        }

        private Patient CreateMockPatient()
        {
            var patientDto1 = new CreatePatientDto
            {
                 FirstName = "Kevin",
                LastName = "DeBruyne",
                FullName = "Kevin DeBruyne",
                Gender = "M",
                DateOfBirth = new DateTime(1991,6,28),
                Email = "kevinDeBruyne@gmail.com",
                Phone = "929888771",
                EmergencyContact = "929111211"
            };

            return new Patient(patientDto1,"200001000912");
        }

        private Staff CreateMockDoctor()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
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

            return new Staff(dto, spec, 1, "healthcareapp.com");
        }

        [Fact]
        public async Task GetOperationRequests_ReturnsBadRequestResult_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await _controller.GetAll();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }
        
        [Fact]
        public async Task GetOperationRequests_ReturnsNoContent_WhenNotFiltering_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var emptyList = new List<OperationRequest>();
            _mockRepo.Setup(repo => repo.GetAllAsync())
                     .ReturnsAsync(emptyList);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<ActionResult<IEnumerable<OperationRequestDto>>>(result);

            Assert.IsType<NoContentResult>(okResult.Result);
        }

        /*[Fact]
        public async Task GetOperationRequest_ReturnsNotFound_WhenFilteringByDoctorId_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(true);

            var emptyList = new List<OperationRequest>();
            _mockRepo.Setup(repo => repo.GetOpRequestsByDoctorIdAsync(It.IsAny<StaffId>()))
                 .ReturnsAsync(emptyList);

            var result = await _controller.GetAllByDoctorId("4953e4a7-a171-4724-ba09-913ae8c9b2d5");

            var actionResult = Assert.IsType<ActionResult<IEnumerable<OperationRequestDto>>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }*/


        [Fact]
        public async Task Patch_ReturnsOk_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(true);

            var mockOperationType = CreateMockOperationType();
            var mockPatient = CreateMockPatient();
            var mockDoctor = CreateMockDoctor();

            var operationRequest = OperationRequestMapper.ToDomainTests(
                mockOperationType,
                DateTime.Parse("2024-10-20T15:30:00"),
                Priority.Elective,
                mockPatient,
                mockDoctor,
                "Original Description"
            );

            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationRequestId>()))
                 .ReturnsAsync(operationRequest);
            _mockOpTypeRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                 .ReturnsAsync(mockOperationType);
            _mockPatientRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<PatientId>()))
                .ReturnsAsync(mockPatient);
            _mockStaffRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(mockDoctor);

            var updateDto = new EditOperationRequestDto
            {
                DeadlineDate = "2024-11-20T15:30:00",
                Priority = "Emergency",
                Description= "Updated description"
            };

            var result = await _controller.Update(operationRequest.Id.AsGuid(), updateDto);

            var okResult = Assert.IsType<ActionResult<OperationRequestDto>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<OperationRequestDto>(objectResult.Value);

            Assert.Equal("2024-11-20T15:30:00", returnValue.DeadlineDate);
            Assert.Equal("Emergency", returnValue.Priority);
            Assert.Equal("Updated description", returnValue.Description);
        }
    }
}