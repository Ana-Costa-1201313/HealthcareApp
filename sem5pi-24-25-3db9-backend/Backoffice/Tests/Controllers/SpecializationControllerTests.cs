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

namespace Backoffice.Tests.Controllers
{
    public class SpecializationControllerTests
    {
        private readonly Mock<ISpecializationRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SpecializationsController _controller;
        private readonly SpecializationService _service;
        private readonly Mock<IExternalApiServices> _mockExternal;
        private readonly Mock<AuthService> _mockAuthService;
        private readonly SpecializationMapper _mapper;
        private readonly Mock<ICD11Service> _icdService;


        public SpecializationControllerTests()
        {
            _icdService = new Mock<ICD11Service>();
            _mockRepo = new Mock<ISpecializationRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mapper = new SpecializationMapper();
            _service = new SpecializationService(
                _mockUnitOfWork.Object,
                _mockRepo.Object,
                _mapper,
                _icdService.Object
            );
            _mockExternal = new Mock<IExternalApiServices>();

            _mockAuthService = new Mock<AuthService>(_mockExternal.Object);

            _controller = new SpecializationsController(_service, _mockAuthService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer someToken";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }



        [Fact]
        public async Task GetById_ReturnsOkResult_WithSpecialization()
        {

            var spec = _mapper.ToDomainForTests("Surgeon");


            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<SpecializationId>()))
                     .ReturnsAsync(spec);

            var result = await _controller.GetGetById(spec.Id.AsGuid());

            var okResult = Assert.IsType<ActionResult<SpecializationDto>>(result);
            var returnValue = Assert.IsType<SpecializationDto>(okResult.Value);

            Assert.Equal("Surgeon", returnValue.Name);
        }


        [Fact]
        public async Task GetAll_ReturnsOkResult_WithSpecializations()
        {
            var spec = _mapper.ToDomainForTests("Surgeon");
            var spec2 = _mapper.ToDomainForTests("Cardio");

            var listSpec = new List<Specialization> { spec, spec2 };

            _mockRepo.Setup(repo => repo.GetAllAsync())
                     .ReturnsAsync(listSpec);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<ActionResult<IEnumerable<SpecializationDto>>>(result);
           
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var returnValue = Assert.IsAssignableFrom<List<SpecializationDto>>(objectResult.Value);

            Assert.Equal(2, returnValue.Count);
            Assert.Equal("Surgeon", returnValue[0].Name);
            Assert.Equal("Cardio", returnValue[1].Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenSpecializationNotExist()
        {
            var specId = Guid.NewGuid();

            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<SpecializationId>()))
                     .ReturnsAsync((Specialization)null);

            var result = await _controller.GetGetById(specId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAll_ReturnsNoContent_WithEmptyList_WhenNoSpecializationExist()
        {
            var emptyList = new List<Specialization>();
            _mockRepo.Setup(repo => repo.GetAllAsync())
                     .ReturnsAsync(emptyList);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<ActionResult<IEnumerable<SpecializationDto>>>(result);
            var objectResult = Assert.IsType<NoContentResult>(okResult.Result);
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
        public async Task SoftDelete_ReturnsOk_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var expectedDto = new SpecializationDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Spec",
                Code = "Code",
                Description = "Description"
            };

            var spec = _mapper.ToDomain(expectedDto);

            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<SpecializationId>()))
                     .ReturnsAsync(spec);

            var result = await _controller.SoftDelete(expectedDto.Id);

            var okResult = Assert.IsType<ActionResult<SpecializationDto>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var returnValue = Assert.IsType<SpecializationDto>(objectResult.Value);
        }

        [Theory]
        [InlineData("", "desc", "Error: This specialization name is already being used.")]
        [InlineData(" ", "", "Error: This specialization name is already being used.")]
        [InlineData(null, null, "Error: This specialization name is already being used.")]
        public async Task Create_ReturnsException_WithInvalidInputWithDuplicateName_WithAuthorization(string name, string description, string error)
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(true);

            var creatingDto = new CreatingSpecializationDto
            (
                name,
                description
            );

            _mockRepo.Setup(repo => repo.SpecializationNameExists(It.IsAny<string>()))
                           .ReturnsAsync(true);


            var result = await _controller.Create(creatingDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);
            Assert.Equal(error, errorMessage);
        }

        [Theory]
        [InlineData("", "desc", "Error: The specialization name can't be null, empty or consist in only white spaces.")]
        [InlineData(" ", "", "Error: The specialization name can't be null, empty or consist in only white spaces.")]
        [InlineData(null, null, "Error: The specialization name can't be null, empty or consist in only white spaces.")]
        public async Task Create_ReturnsException_WithInvalidInput_WithAuthorization(string name, string description, string error)
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(true);

            var creatingDto = new CreatingSpecializationDto
            (
                name,
                description
            );

            _mockRepo.Setup(repo => repo.SpecializationNameExists(It.IsAny<string>()))
                           .ReturnsAsync(false);


            var result = await _controller.Create(creatingDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMessage = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);
            Assert.Equal(error, errorMessage);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidInput_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(true);

            var creatingDto = new CreatingSpecializationDto
            (
                "name",
                "description"
            );

            _mockRepo.Setup(repo => repo.SpecializationNameExists(It.IsAny<string>()))
                           .ReturnsAsync(false);


            var result = await _controller.Create(creatingDto);

            var badRequestResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequestResult_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await _controller.GetAll();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequestResult_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await _controller.GetGetById(Guid.NewGuid());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequestResult_WithoutAuthorization()
        {

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await _controller.Create(new CreatingSpecializationDto("name", "desc"));

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task SoftDelete_ReturnsNotFound_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);


            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<SpecializationId>()))
                     .ReturnsAsync((Specialization)null);

            var result = await _controller.SoftDelete(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task SoftDelete_ReturnsBusinessException_WithAuthorization()
        {
            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var expectedDto = new SpecializationDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Spec",
                Code = "Code",
                Description = "Description"
            };

            var spec = _mapper.ToDomain(expectedDto);

            spec.MarkAsInative();

            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<SpecializationId>()))
                     .ReturnsAsync(spec);

            var result = await _controller.SoftDelete(expectedDto.Id);

            var okResult = Assert.IsType<ActionResult<SpecializationDto>>(result);
            var objectResult = Assert.IsType<BadRequestObjectResult>(okResult.Result);
        }


    }
}
