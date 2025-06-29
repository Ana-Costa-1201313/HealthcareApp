using Backoffice.Controllers;
using Backoffice.Domain.Appointments;
using Backoffice.Domain.Shared;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Patients;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.Logs;
using Backoffice.Domain.Specializations;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.SurgeryRooms.ValueObjects;

namespace Backoffice.Tests.Controllers
{
    public class AppointmentControllerTests
    {
        private Mock<IAppointmentRepository> _repo;
        private Mock<IOperationRequestRepository> _opRepo;
        private Mock<IStaffRepository> _staffRepo;
        private Mock<ISurgeryRoomRepository> _surgeryRoomRepo;
        private Mock<IPatientRepository> _patientRepo;
        private Mock<ILogRepository> _logRepo;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IExternalApiServices> _mockExternal;
        private Mock<AuthService> _mockAuthService;
        private AppointmentService _service;
        private AppointmentController _controller;
        private Mock<IConfiguration> _config;
        private Mock<IEmailService> _emailService;
        private Mock<IOperationTypeRepository> _opTypeRepo;

        private void Setup()
        {
            _repo = new Mock<IAppointmentRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockExternal = new Mock<IExternalApiServices>();
            _config = new Mock<IConfiguration>();
            _emailService = new Mock<IEmailService>();
            _opRepo = new Mock<IOperationRequestRepository>();
            _staffRepo = new Mock<IStaffRepository>();
            _surgeryRoomRepo = new Mock<ISurgeryRoomRepository>();
            _patientRepo = new Mock<IPatientRepository>();
            _logRepo = new Mock<ILogRepository>();
            _opTypeRepo = new Mock<IOperationTypeRepository>();


            _mockAuthService = new Mock<AuthService>(_mockExternal.Object);
            _service = new AppointmentService(_unitOfWork.Object, _repo.Object, _opRepo.Object,
                _staffRepo.Object, _surgeryRoomRepo.Object, _patientRepo.Object, _logRepo.Object, _opTypeRepo.Object);

            _controller = new AppointmentController(_service, _mockAuthService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer someToken";

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private List<Appointment> createAppointmentList()
        {
            List<Appointment> appointments = new List<Appointment>();

            var patientDto = new CreatePatientDto
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

            Patient patient = new Patient(patientDto, "202310000001");

            List<string> slots = new List<string>();
            slots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            slots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            List<Staff> staffList = createStaffList();

            OperationType optype = new OperationType(
                new OperationTypeName("Carpal tunnel syndrome"),
                new OperationTypeDuration(15, 10, 15),
                new List<OperationTypeRequiredStaff>{
                    new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Cardiology"), "1" , "description"), 1),
                });

            OperationRequest opReq1 = new OperationRequest(optype, new DateTime(2024, 10, 11), Priority.Elective, patient, staffList.First(), "The patient needs a knee replacement surgery", new List<string> { staffList.First().Id.ToString() });
            OperationRequest opReq2 = new OperationRequest(optype, new DateTime(2024, 10, 12), Priority.Urgent, patient, staffList.First(), "The patient needs a knee replacement surgery", new List<string> { staffList.First().Id.ToString() });
            OperationRequest opReq3 = new OperationRequest(optype, new DateTime(2024, 10, 13), Priority.Emergency, patient, staffList.First(), "The patient needs a shoulder replacement surgery", new List<string> { staffList.First().Id.ToString() });

            opReq1.ChangeStatus(new List<string> { staffList.First().Email.ToString() });
            opReq2.ChangeStatus(new List<string> { staffList.First().Email.ToString(), staffList.ElementAt(1).Email.ToString() });
            opReq3.ChangeStatus(new List<string> { staffList.First().Email.ToString(), staffList.ElementAt(1).Email.ToString(), staffList.ElementAt(2).Email.ToString() });

            List<SurgeryRoom> surgeryRooms = createSurgeryRoomList();


            CreateAppointmentDto createAppointmentDto1 = new CreateAppointmentDto();//opReq1.Id.AsGuid(), surgeryRoom1.RoomNumber, new DateTime(2024, 10, 11, 12, 0, 0))
            createAppointmentDto1.OpRequestId = opReq1.Id.AsString();
            createAppointmentDto1.SurgeryRoomNumber = surgeryRooms.ElementAt(0).RoomNumber.ToString();
            createAppointmentDto1.DateTime = new DateTime(2024, 10, 11, 12, 0, 0).ToString();

            Appointment appointment1 = new Appointment(opReq1, surgeryRooms.ElementAt(0), new DateTime(2024, 10, 11, 12, 0, 0));
            Appointment appointment2 = new Appointment(opReq2, surgeryRooms.ElementAt(1), new DateTime(2024, 10, 12, 12, 0, 0));
            Appointment appointment3 = new Appointment(opReq3, surgeryRooms.ElementAt(2), new DateTime(2024, 10, 13, 12, 0, 0));

            appointments.Add(appointment1);
            appointments.Add(appointment2);
            appointments.Add(appointment3);

            return appointments;
        }

        private CreateAppointmentDto makeCreateAppointmentDto()
        {
            var a = createAppointmentList().ElementAt(1);

            CreateAppointmentDto createAppointmentDto = new CreateAppointmentDto();
            createAppointmentDto.OpRequestId = a.OperationRequestId.AsString();
            createAppointmentDto.SurgeryRoomNumber = a.SurgeryRoom.RoomNumber.ToString();
            createAppointmentDto.DateTime = new DateTime(2024, 10, 12, 12, 0, 0).ToString();

            return createAppointmentDto;
        }

        private List<Staff> createStaffList()
        {
            List<Staff> staffList = new List<Staff>();

            var staffDto = new CreateStaffDto
            {
                Name = "Mário Ferreira",
                LicenseNumber = 39324,
                Phone = "929244299",
                Specialization = "Cardiology",
                AvailabilitySlots = new List<string>(),
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            var staff1Dto = new CreateStaffDto
            {
                Name = "Maria Silva",
                LicenseNumber = 39324,
                Phone = "929244299",
                Specialization = "Cardiology",
                AvailabilitySlots = new List<string>(),
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            var staff2Dto = new CreateStaffDto
            {
                Name = "António Bantunes",
                LicenseNumber = 39324,
                Phone = "929244299",
                Specialization = "Cardiology",
                AvailabilitySlots = new List<string>(),
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Staff staff = new Staff(staffDto, new Specialization(new SpecializationName("Cardiology"), "1", "description"), 1, "healthcareapp.com");
            Staff staff1 = new Staff(staff1Dto, new Specialization(new SpecializationName("Cardiology"), "1", "description"), 2, "healthcareapp.com");
            Staff staff2 = new Staff(staff2Dto, new Specialization(new SpecializationName("Cardiology"), "1", "description"), 3, "healthcareapp.com");

            staffList.Add(staff);
            staffList.Add(staff1);
            staffList.Add(staff2);

            return staffList;
        }

        private List<SurgeryRoom> createSurgeryRoomList()
        {
            List<SurgeryRoom> surgeryRooms = new List<SurgeryRoom>();

            SurgeryRoom surgeryRoom1 = new SurgeryRoom(1, new SurgeryRoomType("Operating Room", true, "description1"), 10, new List<TimeSlot>());
            SurgeryRoom surgeryRoom2 = new SurgeryRoom(2, new SurgeryRoomType("Operating Room", true, "description2"), 10, new List<TimeSlot>());
            SurgeryRoom surgeryRoom3 = new SurgeryRoom(3, new SurgeryRoomType("Operating Room", true, "description3"), 10, new List<TimeSlot>());

            surgeryRooms.Add(surgeryRoom1);
            surgeryRooms.Add(surgeryRoom2);
            surgeryRooms.Add(surgeryRoom3);

            return surgeryRooms;
        }

        [Fact]
        public async Task GetAllAppointments_SUCCESSFUL()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            List<Appointment> expectedList = createAppointmentList();

            _repo.Setup(repo => repo.GetAllWithDetailsAsync()).ReturnsAsync(expectedList);

            var result = await _controller.GetAll();

            var objectResult = Assert.IsType<OkObjectResult>(result.Result);

            var actualList = Assert.IsAssignableFrom<List<AppointmentDto>>(objectResult.Value);

            Assert.Equal(expectedList.Count, actualList.Count);
        }

        [Fact]
        public async Task GetAllAppointments_FAILURE_FailedAuthorization()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ThrowsAsync(new UnauthorizedAccessException("Error: User not authenticated"));

            var result = await _controller.GetAll();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error: User not authenticated", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAppointmentById_SUCCESSFUL()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            var appointment = createAppointmentList().FirstOrDefault();

            _repo.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<AppointmentId>())).ReturnsAsync(appointment);

            var result = await _controller.GetById(appointment.Id.AsGuid());

            var okResult = Assert.IsType<ActionResult<AppointmentDto>>(result);

            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);

            var actualAppointment = Assert.IsType<AppointmentDto>(objectResult.Value);

            Assert.Equal(appointment.Id.AsGuid(), actualAppointment.AppointmentId);
        }

        [Fact]
        public async Task GetAppointmentById_FAILURE_Not_Found()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            var appointment = createAppointmentList().FirstOrDefault();

            _repo.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<AppointmentId>())).ReturnsAsync((Appointment)null);

            var result = await _controller.GetById(new Guid("11111111-1111-1111-1111-111111111111"));

            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
            //Assert.Equal("Error: DateTime can't be in the past!", notFoundResult.Value);
        }

        [Fact]
        public async Task GetByDate_SUCCESSFUL()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            var appointment = createAppointmentList().ElementAt(2);

            List<Appointment> appointments = new List<Appointment> { appointment };

            string date = appointment.DateTime.ToString("yyyy-MM-dd");

            _repo.Setup(repo => repo.GetByDateWithDetailsAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Appointment> { appointment });
            _opRepo.Setup(repo => repo.GetOpRequestByIdWithDetailsAsync(It.IsAny<OperationRequestId>())).ReturnsAsync(appointment.OperationRequest);

            var result = await _controller.GetByDate(date);

            var objectResult = Assert.IsType<OkObjectResult>(result.Result);

            var actualList = Assert.IsAssignableFrom<List<AppointmentWithDetailsDto>>(objectResult.Value);

            Assert.Equal(appointments.Count, actualList.Count);
        }

        [Fact]
        public async Task CreateAppointment_SUCCESSFUL()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            var createAppointmentDto = makeCreateAppointmentDto();
            var appointment = createAppointmentList().ElementAt(1);

            _repo.Setup(repo => repo.AddAsync(It.IsAny<Appointment>())).ReturnsAsync(appointment);
            _opRepo.Setup(repo => repo.GetOpRequestByIdWithDetailsAsync(It.IsAny<OperationRequestId>())).ReturnsAsync(appointment.OperationRequest);
            _surgeryRoomRepo.Setup(repo => repo.GetSurgeryRoomByRoomNumberAsync(It.IsAny<int>())).ReturnsAsync(appointment.SurgeryRoom);
            _patientRepo.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<PatientId>())).ReturnsAsync(appointment.OperationRequest.Patient);

            var result = await _controller.Create(createAppointmentDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<AppointmentWithDetailsDto>(createdAtActionResult.Value);

            Assert.Equal(201, createdAtActionResult.StatusCode);
            Assert.Equal(appointment.OperationRequestId.AsGuid(), returnValue.OperationRequest.Id);
            Assert.Equal(appointment.SurgeryRoom.RoomNumber, returnValue.SurgeryRoomNumber);
            Assert.Equal(appointment.DateTime, DateTime.Parse(returnValue.DateTime));
        }

        [Fact]
        public async Task UpdateAppointment_SUCCESSFUL()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(true);

            var appointment = createAppointmentList().ElementAt(1);
            var staffList = createStaffList();
            staffList.Insert(0, appointment.OperationRequest.Doctor);

            //var id = appointment.Id.AsGuid();

            var dto = new UpdateAppointmentDto
            {
                //AppointmentId = appointment.Id.AsGuid(),
                SurgeryRoomNumber = "1",
                DateTime = new DateTime(2026, 10, 14, 14, 0, 0).ToString(),
                AppointmentStatus = "Completed",
                StaffList = new List<string> { staffList.First().Email.ToString(), staffList.ElementAt(1).Email.ToString(), staffList.ElementAt(2).Email.ToString() }
            };

            _repo.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<AppointmentId>())).ReturnsAsync(appointment);
            _surgeryRoomRepo.Setup(repo => repo.GetSurgeryRoomByRoomNumberAsync(It.IsAny<int>())).ReturnsAsync(createSurgeryRoomList().ElementAt(0));
            _opRepo.Setup(repo => repo.GetOpRequestByIdWithDetailsAsync(It.IsAny<OperationRequestId>())).ReturnsAsync(appointment.OperationRequest);

            var callIndex = 0;

            _staffRepo.Setup(repo => repo.GetStaffByEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(() => staffList[callIndex++ % staffList.Count]);

            var result = await _controller.Update(appointment.Id.AsGuid(), dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AppointmentWithDetailsDto>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(Int32.Parse(dto.SurgeryRoomNumber), returnValue.SurgeryRoomNumber);
            Assert.Equal(appointment.DateTime, DateTime.Parse(returnValue.DateTime));
            Assert.Equal(dto.AppointmentStatus, returnValue.Status);
            Assert.Equal(dto.StaffList.Count, returnValue.OperationRequest.SelectedStaff.Count);
        }

        [Fact]
        public async Task UpdateAppointment_FAILURE_New_DateTime_In_The_Past()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var appointment = createAppointmentList().First();

            var dto = new UpdateAppointmentDto
            {
                //AppointmentId = appointment.Id.AsGuid(),
                SurgeryRoomNumber = "1",
                DateTime = new DateTime(2024, 10, 14, 14, 0, 0).ToString(),
                AppointmentStatus = "Completed"

            };

            _repo.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<AppointmentId>())).ReturnsAsync(appointment);
            _surgeryRoomRepo.Setup(repo => repo.GetSurgeryRoomByRoomNumberAsync(It.IsAny<int>())).ReturnsAsync(createSurgeryRoomList().ElementAt(0));
            _staffRepo.Setup(repo => repo.GetStaffByEmailAsync(It.IsAny<Email>())).ReturnsAsync(appointment.OperationRequest.Doctor);

            var result = await _controller.Update(appointment.Id.AsGuid(), dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Error: DateTime can't be in the past!", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAppointment_FAILURE_SurgeryRoom_Not_Found()
        {
            Setup();

            _mockAuthService.Setup(auth => auth.IsAuthorized(It.IsAny<HttpRequest>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(true);

            var appointment = createAppointmentList().First();

            var dto = new UpdateAppointmentDto
            {
                //AppointmentId = appointment.Id.AsGuid(),
                SurgeryRoomNumber = "4",
                DateTime = new DateTime(2026, 10, 14, 14, 0, 0).ToString(),
                AppointmentStatus = "Completed"
            };

            _repo.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<AppointmentId>())).ReturnsAsync(appointment);
            _surgeryRoomRepo.Setup(repo => repo.GetSurgeryRoomByRoomNumberAsync(It.IsAny<int>())).ReturnsAsync((SurgeryRoom)null);

            var result = await _controller.Update(appointment.Id.AsGuid(), dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Error: No Surgery Room found with the given number!", badRequestResult.Value);
        }
    }
}