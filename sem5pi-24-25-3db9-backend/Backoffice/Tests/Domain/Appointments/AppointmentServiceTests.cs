using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Backoffice.Domain.Appointments;
using Backoffice.Domain.Shared;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Logs;
using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Specializations;

public class AppointmentServiceTests
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
    private Mock<IConfiguration> _config;
    private Mock<IEmailService> _emailService;
    private Mock<IOperationTypeRepository> _opTypeRepository;

    private void Setup()
    {
        _repo = new Mock<IAppointmentRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _opRepo = new Mock<IOperationRequestRepository>();
        _staffRepo = new Mock<IStaffRepository>();
        _surgeryRoomRepo = new Mock<ISurgeryRoomRepository>();
        _patientRepo = new Mock<IPatientRepository>();
        _logRepo = new Mock<ILogRepository>();
        _opTypeRepository = new Mock<IOperationTypeRepository>();

        _service = new AppointmentService(_unitOfWork.Object, _repo.Object, _opRepo.Object,
            _staffRepo.Object, _surgeryRoomRepo.Object, _patientRepo.Object, _logRepo.Object, _opTypeRepository.Object);
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

            var staffDto = new CreateStaffDto
            {
                Name = "Mário Ferreira",
                LicenseNumber = 39324,
                Phone = "929244299",
                Specialization = "Cardiology",
                AvailabilitySlots = slots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            Staff staff = new Staff(staffDto, new Specialization(new SpecializationName("Cardiology"), "1", "description"), 1, "healthcareapp.com");

            OperationType optype = new OperationType(
                new OperationTypeName("Carpal tunnel syndrome"),
                new OperationTypeDuration(15, 10, 15),
                new List<OperationTypeRequiredStaff>{
                    new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Cardiology"), "1" , "description"), 1),
                });

            OperationRequest opReq1 = new OperationRequest(optype, new DateTime(2024, 10, 11), Priority.Elective, patient, staff, "The patient needs a knee replacement surgery", new List<string> { staff.Id.ToString() });
            OperationRequest opReq2 = new OperationRequest(optype, new DateTime(2024, 10, 12), Priority.Urgent, patient, staff, "The patient needs a knee replacement surgery", new List<string> { staff.Id.ToString() });
            OperationRequest opReq3 = new OperationRequest(optype, new DateTime(2024, 10, 13), Priority.Emergency, patient, staff, "The patient needs a shoulder replacement surgery", new List<string> { staff.Id.ToString() });

            /* SurgeryRoom surgeryRoom1 = new SurgeryRoom(1, new SurgeryRoomType("Operating Room", true, "description"), 10, new List<TimeSlot>());
            SurgeryRoom surgeryRoom2 = new SurgeryRoom(2, new SurgeryRoomType("Operating Room", true, "description"), 10, new List<TimeSlot>());
            SurgeryRoom surgeryRoom3 = new SurgeryRoom(3, new SurgeryRoomType("Operating Room", true, "description"), 10, new List<TimeSlot>()); */

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

        //[Fact]
        //public async Task 
}