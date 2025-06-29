using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Backoffice.Controllers;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Specializations;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;
using Backoffice.Domain.Logs;
using Backoffice.Infraestructure.Specializations;

namespace Backoffice.Tests.Controllers
{
    public class OperationTypesControllerTests
    {
        private readonly Mock<IOperationTypeRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ISpecializationRepository> _mockSpecializationRepo;
        private readonly Mock<ILogRepository> _mockLogRepo;
        private readonly Mock<IExternalApiServices> _mockExternal;
        private readonly Mock<AuthService> _mockAuthService;
        private readonly OperationTypesController _controller;
        private readonly OperationTypeService _service;
        private readonly OperationTypeMapper _mapper;

        public OperationTypesControllerTests()
        {
            _mockRepo = new Mock<IOperationTypeRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockSpecializationRepo = new Mock<ISpecializationRepository>();
            _mockLogRepo = new Mock<ILogRepository>();

            _mockExternal = new Mock<IExternalApiServices>();
            _mapper = new OperationTypeMapper();
            _mockAuthService = new Mock<AuthService>(_mockExternal.Object);

            _service = new OperationTypeService(
                _mockUnitOfWork.Object,
                _mockRepo.Object,
                _mockSpecializationRepo.Object,
                _mockLogRepo.Object,
                _mapper
            );

            _controller = new OperationTypesController(_service, _mockAuthService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer someToken";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetOperationTypes_ReturnsNoContent_WhenNotFiltering_WithAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var emptyList = new List<OperationType>();
            _mockRepo.Setup(repo => repo.GetAllWithDetailsAsync())
                     .ReturnsAsync(emptyList);

            var result = await _controller.GetOperationTypes();

            var okResult = Assert.IsType<ActionResult<List<OperationTypeDto>>>(result);

            Assert.IsType<NoContentResult>(okResult.Result);
        }

        [Fact]
        public async Task GetOperationTypes_ReturnsNotFound_WhenFiltering_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var emptyList = new List<OperationType>();
            _mockRepo.Setup(repo => repo.FilterOperationTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                     .ReturnsAsync(emptyList);

            var query = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "name", "NonExistentName" },
                { "specialization", "NonExistentSpecialization" },
                { "status", "false" }
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(query);

            var result = await _controller.GetOperationTypes();

            var actionResult = Assert.IsType<ActionResult<List<OperationTypeDto>>>(result);
            Assert.IsType<NoContentResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetOperationTypes_ReturnsBadRequestResult_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await _controller.GetOperationTypes();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task GetOperationTypes_ReturnsOkResult_WithOperationTypes_WhenNotFiltering_WithAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);


            var requiredStaff2 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 2),
                    ("Cardio", 3)
                };
            var operationType2 = _mapper.ToDomainForTests("Embolectomy", 30, 60, 15, requiredStaff2);


            var listOp = new List<OperationType> { operationType1, operationType2 };

            _mockRepo.Setup(repo => repo.GetAllWithDetailsAsync())
                     .ReturnsAsync(listOp);

            var result = await _controller.GetOperationTypes();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var returnValue = Assert.IsType<List<OperationTypeDto>>(okResult.Value);

            Assert.Equal(2, returnValue.Count);

            Assert.Equal("Surgery", returnValue[0].Name);
            Assert.Equal(30, returnValue[0].AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, returnValue[0].SurgeryInMinutes);
            Assert.Equal(15, returnValue[0].CleaningInMinutes);
            Assert.Single(returnValue[0].RequiredStaff);


            var surgeryStaff = returnValue[0].RequiredStaff.First();
            Assert.Equal("Surgeon", surgeryStaff.Specialization);
            Assert.Equal(5, surgeryStaff.Total);

            Assert.Equal("Embolectomy", returnValue[1].Name);
            Assert.Equal(30, returnValue[1].AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, returnValue[1].SurgeryInMinutes);
            Assert.Equal(15, returnValue[1].CleaningInMinutes);
            Assert.Equal(2, returnValue[1].RequiredStaff.Count);

            var embolectomyStaff1 = returnValue[1].RequiredStaff[0];
            Assert.Equal("Surgeon", embolectomyStaff1.Specialization);
            Assert.Equal(2, embolectomyStaff1.Total);

            var embolectomyStaff2 = returnValue[1].RequiredStaff[1];
            Assert.Equal("Cardio", embolectomyStaff2.Specialization);
            Assert.Equal(3, embolectomyStaff2.Total);
        }

        [Fact]
        public async Task GetOperationTypes_ReturnsOkResult_WithOperationTypes_WhenFilteringBySpecialization_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
            {
                ("Surgeon", 5)
            };

            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            var listOp = new List<OperationType>
            {
                operationType1
            };

            _mockRepo.Setup(repo => repo.FilterOperationTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
                     .ReturnsAsync(listOp);

            var query = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "specialization", "Surgeon" }
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _controller.ControllerContext.HttpContext.Request.Query = new QueryCollection(query);

            var result = await _controller.GetOperationTypes();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var returnValue = Assert.IsType<List<OperationTypeDto>>(okResult.Value);

            Assert.Single(returnValue);

            Assert.Equal("Surgery", returnValue[0].Name);
            Assert.Equal(30, returnValue[0].AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, returnValue[0].SurgeryInMinutes);
            Assert.Equal(15, returnValue[0].CleaningInMinutes);
            Assert.Single(returnValue[0].RequiredStaff);

            var surgeryStaff = returnValue[0].RequiredStaff.First();
            Assert.Equal("Surgeon", surgeryStaff.Specialization);
            Assert.Equal(5, surgeryStaff.Total);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithOperationType_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var requiredStaff = new List<(string SpecializationName, int Total)>
                        {
                            ("Surgeon", 5)
                        };

            var operationType = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff);


            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(operationType);

            var result = await _controller.GetGetById(operationType.Id.AsGuid());


            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<OperationTypeDto>(objectResult.Value);

            Assert.Equal("Surgery", returnValue.Name);
            Assert.Equal(30, returnValue.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, returnValue.SurgeryInMinutes);
            Assert.Equal(15, returnValue.CleaningInMinutes);
            Assert.Single(returnValue.RequiredStaff);
            Assert.Equal("Surgeon", returnValue.RequiredStaff[0].Specialization);
            Assert.Equal(5, returnValue.RequiredStaff[0].Total);
        }



        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOperationTypeDoesNotExist_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var operationTypeId = Guid.NewGuid();

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync((OperationType)null);

            var result = await _controller.GetGetById(operationTypeId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WithoutAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync((OperationType)null);

            var result = await _controller.GetGetById(Guid.NewGuid());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                    .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var creatingDto = new CreatingOperationTypeDto
            (
                "Surgery",
                30,
                60,
                15,
                new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 }
                }
            );

            var result = await _controller.Create(creatingDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidInput_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var creatingDto = new CreatingOperationTypeDto
            (
                "Op",
                30,
                60,
                15,
                new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Spec", Total = 2 }
                }
            );

            var createdDto = new OperationTypeDto
            {
                Id = Guid.NewGuid(),
                Name = creatingDto.Name,
                AnesthesiaPatientPreparationInMinutes = creatingDto.AnesthesiaPatientPreparationInMinutes,
                SurgeryInMinutes = creatingDto.SurgeryInMinutes,
                CleaningInMinutes = creatingDto.CleaningInMinutes,
                RequiredStaff = creatingDto.RequiredStaff
            };

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<OperationType>()))
                        .ReturnsAsync(_mapper.ToDomain(createdDto));

            _mockSpecializationRepo.Setup(repo => repo.SpecializationNameExists(It.IsAny<string>()))
                           .ReturnsAsync(true);

            var specialization = new Specialization(new SpecializationName("Spec"), "code", "description");

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Spec"))
                                   .ReturnsAsync(specialization);


            _mockLogRepo.Setup(repo => repo.AddAsync(It.IsAny<Log>()))
                        .ReturnsAsync(new Mock<Log>().Object);



            var result = await _controller.Create(creatingDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<OperationTypeDto>(createdAtActionResult.Value);
            Assert.Equal("Op", returnValue.Name);
            Assert.Equal(30, returnValue.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, returnValue.SurgeryInMinutes);
            Assert.Equal(15, returnValue.CleaningInMinutes);
            Assert.Equal("Spec", returnValue.RequiredStaff[0].Specialization);
            Assert.Equal(2, returnValue.RequiredStaff[0].Total);
        }

        [Theory]
        [InlineData("   ", 10, 10, 10, "SpecName", 10, "Error: The operation type name can't be null, empty or consist in only white spaces.")]
        [InlineData("", 10, 10, 10, "SpecName", 10, "Error: The operation type name can't be null, empty or consist in only white spaces.")]
        [InlineData(null, 10, 10, 10, "SpecName", 10, "Error: The operation type name can't be null, empty or consist in only white spaces.")]
        [InlineData("OpName", -1, 10, 10, "SpecName", 10, "Error: The anesthesia/preparation duration must be more than 0 minutes.")]
        [InlineData("OpName", 0, 10, 10, "SpecName", 10, "Error: The anesthesia/preparation duration must be more than 0 minutes.")]
        [InlineData("OpName", 10, -1, 10, "SpecName", 10, "Error: The surgery duration must be more than 0 minutes.")]
        [InlineData("OpName", 10, 0, 10, "SpecName", 10, "Error: The surgery duration must be more than 0 minutes.")]
        [InlineData("OpName", 10, 10, -1, "SpecName", 10, "Error: The cleaning duration must be more than 0 minutes.")]
        [InlineData("OpName", 10, 10, 0, "SpecName", 10, "Error: The cleaning duration must be more than 0 minutes.")]
        [InlineData("OpName", 10, 10, 10, "SpecName", -10, "Error: The total number of required staff of a specialization can't be lower or equal to 0.")]
        [InlineData("OpName", 10, 10, 10, "SpecName", 0, "Error: The total number of required staff of a specialization can't be lower or equal to 0.")]
        [InlineData("OpName", 10, 10, 10, "     ", 10, "Error: The specialization can't be null.")]
        [InlineData("OpName", 10, 10, 10, "", 10, "Error: The specialization can't be null.")]
        [InlineData("OpName", 10, 10, 10, null, 10, "Error: The specialization can't be null.")]
        public async Task Create_ReturnsException_WithInvalidInput_WithAuthorization(string opName, int opPhase1Duration, int opPhase2Duration, int opPhase3Duration, string specName, int total, string error)
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(true);

            var creatingDto = new CreatingOperationTypeDto
            (
                opName,
                opPhase1Duration,
                opPhase2Duration,
                opPhase3Duration,
                new List<RequiredStaffDto>
                {
            new RequiredStaffDto { Specialization = specName, Total = total }
                }
            );

            _mockSpecializationRepo.Setup(repo => repo.SpecializationNameExists(It.IsAny<string>()))
                           .ReturnsAsync(true);

            var specialization = new Specialization(new SpecializationName("SpecName"), "code", "description");

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("SpecName"))
                                   .ReturnsAsync(specialization);

            var result = await _controller.Create(creatingDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);
            Assert.Equal(error, errorMessage);
        }

        [Fact]
        public async Task SoftDelete_ReturnsBadRequest_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));


            var result = await _controller.SoftDelete(Guid.NewGuid());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task SoftDelete_ReturnsNotFound_WithAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                        .ReturnsAsync(true);

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                        .ReturnsAsync((OperationType)null);

            var result = await _controller.SoftDelete(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task SoftDelete_ReturnsOk_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var expectedDto = new OperationTypeDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Operation",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 60,
                CleaningInMinutes = 15,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 }
                },
                Active = true
            };

            var opType = _mapper.ToDomain(expectedDto);

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(opType);

            var result = await _controller.SoftDelete(expectedDto.Id);

            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var returnValue = Assert.IsType<OperationTypeDto>(objectResult.Value);

            Assert.False(returnValue.Active);
        }

        [Fact]
        public async Task Patch_ReturnsBadRequest_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };


            var result = await _controller.PatchOperationType(Guid.NewGuid(), editOperationTypeDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };


            var result = await _controller.UpdateOperationType(Guid.NewGuid(), editOperationTypeDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task Patch_ReturnsNotFound_WithAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                        .ReturnsAsync(true);

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                        .ReturnsAsync((OperationType)null);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };

            var result = await _controller.PatchOperationType(Guid.NewGuid(), editOperationTypeDto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WithAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                        .ReturnsAsync(true);

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                        .ReturnsAsync((OperationType)null);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };

            var result = await _controller.UpdateOperationType(Guid.NewGuid(), editOperationTypeDto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Put_ReturnsOk_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(operationType1);

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Surgeon"))
                  .ReturnsAsync(new Specialization(new SpecializationName("Surgeon"), "code", "description"));

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Anesthesiologist"))
                                   .ReturnsAsync(new Specialization(new SpecializationName("Anesthesiologist"), "code", "description"));

            var updateDto = new EditOperationTypeDto
            {
                Name = "Updated Operation",
                AnesthesiaPatientPreparationInMinutes = 40,
                SurgeryInMinutes = 70,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 3 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };

            var result = await _controller.UpdateOperationType(operationType1.Id.AsGuid(), updateDto);

            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<OperationTypeDto>(objectResult.Value);

            Assert.Equal("Updated Operation", returnValue.Name);
            Assert.Equal(40, returnValue.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(70, returnValue.SurgeryInMinutes);
            Assert.Equal(20, returnValue.CleaningInMinutes);
            Assert.Equal(3, returnValue.RequiredStaff[0].Total);
        }

        [Fact]
        public async Task Patch_ReturnsOk_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(operationType1);

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Surgeon"))
                  .ReturnsAsync(new Specialization(new SpecializationName("Surgeon"), "code", "description"));

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Anesthesiologist"))
                                   .ReturnsAsync(new Specialization(new SpecializationName("Anesthesiologist"), "code", "description"));

            var updateDto = new EditOperationTypeDto
            {
                Name = "Updated Operation",
                AnesthesiaPatientPreparationInMinutes = 40,
                SurgeryInMinutes = 70,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 3 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };

            var result = await _controller.PatchOperationType(operationType1.Id.AsGuid(), updateDto);

            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<OperationTypeDto>(objectResult.Value);

            Assert.Equal("Updated Operation", returnValue.Name);
            Assert.Equal(40, returnValue.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(70, returnValue.SurgeryInMinutes);
            Assert.Equal(20, returnValue.CleaningInMinutes);
            Assert.Equal(3, returnValue.RequiredStaff[0].Total);
        }

        [Fact]
        public async Task Patch_ReturnsBusinessException_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(operationType1);

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Surgeon"))
                  .ReturnsAsync(new Specialization(new SpecializationName("Surgeon"), "code", "description"));

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Anesthesiologist"))
                                   .ReturnsAsync(new Specialization(new SpecializationName("Anesthesiologist"), "code", "description"));

            var updateDto = new EditOperationTypeDto
            {
                Name = "Name",
                AnesthesiaPatientPreparationInMinutes = 40,
                SurgeryInMinutes = 70,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "ASDAS", Total = 3 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };

            var result = await _controller.PatchOperationType(operationType1.Id.AsGuid(), updateDto);

            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<BadRequestObjectResult>(okResult.Result);
        }


        [Fact]
        public async Task Put_ReturnsBusinessException_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(operationType1);

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Surgeon"))
                  .ReturnsAsync((Specialization)null);

            _mockSpecializationRepo.Setup(repo => repo.GetBySpecializationName("Anesthesiologist"))
                                   .ReturnsAsync(new Specialization(new SpecializationName("Anesthesiologist"), "code", "description"));

            var updateDto = new EditOperationTypeDto
            {
                Name = "Updated Operation",
                AnesthesiaPatientPreparationInMinutes = 40,
                SurgeryInMinutes = 70,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 3 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 }
                }
            };

            var result = await _controller.UpdateOperationType(operationType1.Id.AsGuid(), updateDto);

            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<BadRequestObjectResult>(okResult.Result);
        }

        [Fact]
        public async Task SoftDelete_ReturnsBusinessException_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var expectedDto = new OperationTypeDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Operation",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 60,
                CleaningInMinutes = 15,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 }
                },
                Active = true
            };

            var opType = _mapper.ToDomain(expectedDto);

            opType.MarkAsInative();

            _mockRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(opType);

            var result = await _controller.SoftDelete(expectedDto.Id);

            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<BadRequestObjectResult>(okResult.Result);
        }


        [Fact]
        public async Task HardDelete_ReturnsBadRequest_WhenAuthorizationFails()
        {

            var id = Guid.NewGuid();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new Exception("Unauthorized"));

            var result = await _controller.HardDelete(id);

            var badRequestResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
            Assert.Equal("Unauthorized", objectResult.Value);
        }

        [Fact]
        public async Task HardDelete_ReturnsNotFound_WhenBusinessRuleValidationExceptionIsThrown()
        {
            var id = Guid.NewGuid();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            _mockRepo.Setup(service => service.GetByIdAsync(It.IsAny<OperationTypeId>()))
                        .ThrowsAsync(new BusinessRuleValidationException("Business rule violated"));

            var result = await _controller.HardDelete(id);

            var notFoundResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<NotFoundObjectResult>(notFoundResult.Result);
            Assert.Equal("Business rule violated", ((dynamic)objectResult.Value).Message);
        }

        [Fact]
        public async Task HardDelete_ReturnsBadRequest_WhenGenericExceptionIsThrown()
        {
            var id = Guid.NewGuid();
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            _mockRepo.Setup(service => service.GetByIdAsync(It.IsAny<OperationTypeId>()))
                        .ThrowsAsync(new Exception("An unexpected error occurred"));

            var result = await _controller.HardDelete(id);

            var badRequestResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            var objectResult = Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
            Assert.Equal("An unexpected error occurred", ((dynamic)objectResult.Value).Message);
        }

        [Fact]
        public async Task HardDelete_ReturnsOk_WhenAuthorizedAndSuccessful()
        {
            var id = Guid.NewGuid();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var operationType = new OperationType();

            _mockRepo.Setup(service => service.GetByIdAsync(It.IsAny<OperationTypeId>()))
                     .ReturnsAsync(operationType);

            var result = await _controller.HardDelete(id);

            var okResult = Assert.IsType<ActionResult<OperationTypeDto>>(result);
            Assert.IsType<OkResult>(okResult.Result);
        }
    }
}
