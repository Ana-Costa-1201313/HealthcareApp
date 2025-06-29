using Backoffice.Domain.Logs;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Backoffice.Tests
{
    public class PatientServiceTests
    {
        Mock<IPatientRepository> patientRepository;
        Mock<IUnitOfWork> unitOfWork;
        Mock<ILogRepository> logRepository;
        Mock<PatientService> mockService;
        Mock<IEmailService> emailService;
        Mock<IConfiguration> configuration;

        private void Setup(List<Patient> patientsDataBase)
        {
            patientRepository = new Mock<IPatientRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            logRepository = new Mock<ILogRepository>();
            emailService = new Mock<IEmailService>();
            configuration = new Mock<IConfiguration>();

            //Mock obter todos os patients
            patientRepository.Setup(repo => repo.GetAllWithDetailsAsync()).ReturnsAsync(patientsDataBase);

            //Mock obter patient por id 
            patientRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<PatientId>()))
            .ReturnsAsync((PatientId id) => patientsDataBase.SingleOrDefault(p => p.Id.Equals(id)));

            patientRepository.Setup(repo => repo.GetWithDetailsAsync(It.IsAny<PatientId>()))
           .ReturnsAsync((PatientId id) => patientsDataBase.SingleOrDefault(p => p.Id.Equals(id)));

            //Mock para adicionar um patient
            patientRepository.Setup(repo => repo.AddAsync(It.IsAny<Patient>()))
            .Callback<Patient>(p => patientsDataBase.Add(p));

            //Mock para obter o ultimo patient pelo mês
            patientRepository.Setup(repo => repo.GetLatestPatientByMonthAsync())
            .ReturnsAsync(() => patientsDataBase.OrderByDescending(p => p.MedicalRecordNumber).FirstOrDefault());

            //Mock para remover o patient
            patientRepository.Setup(repo => repo.Remove(It.IsAny<Patient>()))
            .Callback<Patient>(p => patientsDataBase.Remove(p));

            //Mock para o searchPatients
            patientRepository.Setup(repo => repo.SearchPatientsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<string>()))
            .ReturnsAsync((string name, string email, DateTime? dateOfBirth, string medicalRecordNumber) =>
            patientsDataBase.Where(p =>
            (string.IsNullOrEmpty(name) || p.FullName.Contains(name)) &&
            (string.IsNullOrEmpty(email) || p.Email._Email == email) &&
            (!dateOfBirth.HasValue || p.DateOfBirth.Date == dateOfBirth.Value.Date) &&
            (string.IsNullOrEmpty(medicalRecordNumber) || p.MedicalRecordNumber.Contains(medicalRecordNumber))
        ).ToList());

            logRepository.Setup(repo => repo.AddAsync(It.IsAny<Log>()))
            .ReturnsAsync(new Mock<Log>().Object);

            unitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(0);

            mockService = new Mock<PatientService>(unitOfWork.Object, patientRepository.Object, new PatientMapper(), logRepository.Object, emailService.Object, configuration.Object);
        }
        [Fact]
        public async Task GetAllAsync()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "John",
                LastName = "Stones",
                FullName = "John Stones",
                Gender = "M",
                DateOfBirth = new DateTime(1994, 5, 28),
                Email = "johnStones@gmail.com",
                Phone = "999888777",
                EmergencyContact = "939111222"
            };
            var patientDto2 = new CreatePatientDto
            {
                FirstName = "Kyle",
                LastName = "Walker",
                FullName = "Kyle Walker",
                Gender = "M",
                DateOfBirth = new DateTime(1990, 5, 28),
                Email = "kyleWalker@gmail.com",
                Phone = "999888213",
                EmergencyContact = "919111222"
            };

            var patient1 = new Patient(patientDto1, "202310000001");
            var patient2 = new Patient(patientDto2, "202310000002");

            patientsDataBase.Add(patient1);
            patientsDataBase.Add(patient2);

            var result = await service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("John Stones", result[0].FullName);
            Assert.Equal("M", result[0].Gender);
            Assert.Equal(new DateTime(1994, 5, 28), result[0].DateOfBirth);
            Assert.Equal("johnStones@gmail.com", result[0].Email);
            Assert.Equal("999888777", result[0].Phone);
            Assert.Equal("202310000001", result[0].MedicalRecordNumber);

            Assert.Equal("Kyle Walker", result[1].FullName);
            Assert.Equal("M", result[1].Gender);
            Assert.Equal(new DateTime(1990, 5, 28), result[1].DateOfBirth);
            Assert.Equal("kyleWalker@gmail.com", result[1].Email);
            Assert.Equal("999888213", result[1].Phone);
            Assert.Equal("202310000002", result[1].MedicalRecordNumber);

        }

        [Fact]

        public async Task GetByIdAsync()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "John",
                LastName = "Stones",
                FullName = "John Stones",
                Gender = "M",
                DateOfBirth = new DateTime(1994, 5, 28),
                Email = "johnStones@gmail.com",
                Phone = "999888777",
                EmergencyContact = "939111222"
            };
            var patientDto2 = new CreatePatientDto
            {
                FirstName = "Kyle",
                LastName = "Walker",
                FullName = "Kyle Walker",
                Gender = "M",
                DateOfBirth = new DateTime(1990, 5, 28),
                Email = "kyleWalker@gmail.com",
                Phone = "999888213",
                EmergencyContact = "919111222"
            };

            var patient1 = new Patient(patientDto1, "202310000001");
            var patient2 = new Patient(patientDto2, "202310000002");

            patientsDataBase.Add(patient1);
            patientsDataBase.Add(patient2);

            var result = await service.GetByIdAsync(patient1.Id.AsGuid());

            Assert.NotNull(result);
            Assert.Equal("John Stones", result.FullName);
            Assert.Equal("M", result.Gender);
            Assert.Equal(new DateTime(1994, 5, 28), result.DateOfBirth);
            Assert.Equal("johnStones@gmail.com", result.Email);
            Assert.Equal("999888777", result.Phone);
            Assert.Equal("202310000001", result.MedicalRecordNumber);

        }
        [Fact]
        public async Task GetByIdThatDoesNotExist()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "John",
                LastName = "Stones",
                FullName = "John Stones",
                Gender = "M",
                DateOfBirth = new DateTime(1994, 5, 28),
                Email = "johnStones@gmail.com",
                Phone = "999888777",
                EmergencyContact = "939111222"
            };
            var patientDto2 = new CreatePatientDto
            {
                FirstName = "Kyle",
                LastName = "Walker",
                FullName = "Kyle Walker",
                Gender = "M",
                DateOfBirth = new DateTime(1990, 5, 28),
                Email = "kyleWalker@gmail.com",
                Phone = "999888213",
                EmergencyContact = "919111222"
            };

            var patient1 = new Patient(patientDto1, "202310000001");
            var patient2 = new Patient(patientDto2, "202310000002");

            patientsDataBase.Add(patient1);

            var result = await service.GetByIdAsync(patient2.Id.AsGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Ederson",
                LastName = "Moraes",
                FullName = "Ederson Moraes",
                Gender = "M",
                DateOfBirth = new DateTime(1993, 7, 17),
                Email = "edersonMoraes@gmail.com",
                Phone = "919888771",
                EmergencyContact = "934111222"
            };

            var result = await service.AddAsync(patientDto1, "202310000001");


            Assert.NotNull(result);
            Assert.Equal("Ederson Moraes", result.FullName);
            Assert.Equal("M", result.Gender);
            Assert.Equal(new DateTime(1993, 7, 17), result.DateOfBirth);
            Assert.Equal("edersonMoraes@gmail.com", result.Email);
            Assert.Equal("919888771", result.Phone);
            Assert.Equal("202310000001", result.MedicalRecordNumber);
        }
        [Fact]
        public async Task UpdateAsync()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Ederson",
                LastName = "Moraes",
                FullName = "Ederson Moraes",
                Gender = "M",
                DateOfBirth = new DateTime(1993, 7, 17),
                Email = "edersonMoraes@gmail.com",
                Phone = "919888771",
                EmergencyContact = "934111222"
            };
            var patient1 = new Patient(patientDto1, "202012000001");

            patientsDataBase.Add(patient1);
            var editPatientDto1 = new EditPatientDto
            {
                FirstName = "Nathan",
                LastName = "Ake",
                FullName = "Nathan Ake",
                Email = "nathanAke@gmail.com",
                Phone = "919191910",
                EmergencyContact = "929292920"
            };
            var result = await service.UpdateAsync(patient1.Id.AsGuid(), editPatientDto1);
            Assert.NotNull(result);
            Assert.Equal("Nathan Ake", result.FullName);
            Assert.Equal("nathanAke@gmail.com", result.Email);
            Assert.Equal("919191910", result.Phone);
            Assert.Equal(new DateTime(1993, 7, 17), result.DateOfBirth);
            Assert.Equal("202012000001", result.MedicalRecordNumber);
            Assert.NotEqual("Ederson", result.FirstName);

        }
        [Fact]
        public async Task PatchAsync()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Ederson",
                LastName = "Moraes",
                FullName = "Ederson Moraes",
                Gender = "M",
                DateOfBirth = new DateTime(1993, 7, 17),
                Email = "edersonMoraes@gmail.com",
                Phone = "919888771",
                EmergencyContact = "934111222"
            };

            var patient1 = new Patient(patientDto1, "202012000001");

            patientsDataBase.Add(patient1);
            var editPatientDto1 = new EditPatientDto
            {
                FirstName = "Nathan",
                LastName = "Ake",
                FullName = "Nathan Ake",
                EmergencyContact = "929292920"
            };
            var result = await service.PatchAsync(patient1.Id.AsGuid(), editPatientDto1);
            Assert.NotNull(result);
            Assert.Equal("Nathan Ake", result.FullName);
            Assert.Equal("edersonMoraes@gmail.com", result.Email);
            Assert.Equal("929292920", result.EmergencyContact);

            Assert.Equal("919888771", result.Phone);
            Assert.Equal(new DateTime(1993, 7, 17), result.DateOfBirth);
            Assert.Equal("202012000001", result.MedicalRecordNumber);
            Assert.NotEqual("Ederson", result.FirstName);

            //logRepository.Verify(logRepo => logRepo.AddAsync(It.IsAny<Log>()), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;


            var patientDto = new CreatePatientDto
            {
                FirstName = "Rico",
                LastName = "Lewis",
                FullName = "Rico Lewis",
                Gender = "M",
                DateOfBirth = new DateTime(2004, 11, 21),
                Email = "ricoLewis@gmail.com",
                Phone = "919800000",
                EmergencyContact = "934111000"
            };

            var patient = new Patient(patientDto, "201801000001");
            patientsDataBase.Add(patient);

            var result = await service.DeleteAsync(patient.Id.AsGuid());

            Assert.NotNull(result);
            Assert.Equal("Rico Lewis", result.FullName);
            Assert.Equal("ricoLewis@gmail.com", result.Email);
            Assert.Equal("919800000", result.Phone);

        }
        [Fact]
        public async Task DeleteAsyncPatientNotFound()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

            var id = Guid.NewGuid();

            var result = await service.DeleteAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task SearhPatientsAsyncByOnlyName()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

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

            patientsDataBase.Add(patient1);
            patientsDataBase.Add(patient2);
            patientsDataBase.Add(patient3);

            var searchPatientDto = new SearchPatientDto
            {
                FullName = "Viktor Gyokeres",
                Email = "gyokeresSporting@gmail.com",
                DateOfBirth = new DateTime(1998, 6, 4)
            };


            var result = await service.SearchPatientsAsync("Joao", null, null, null);

            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(searchPatientDto, result);
            Assert.Equal("Joao Mario", result[0].FullName);
            Assert.Equal("Joao Mario", result[1].FullName);
            Assert.Equal(new DateTime(1993, 1, 19), result[0].DateOfBirth);
            Assert.Equal(new DateTime(2000, 1, 3), result[1].DateOfBirth);
        }

        [Fact]
        public async Task SearhPatientsAsyncByOnlyNameAndDateOfBirth()
        {
            var patientsDataBase = new List<Patient>();

            Setup(patientsDataBase);
            mockService.CallBase = true;

            PatientService service = mockService.Object;

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

            patientsDataBase.Add(patient1);
            patientsDataBase.Add(patient2);
            patientsDataBase.Add(patient3);

            var searchPatientDto = new SearchPatientDto
            {
                FullName = "Viktor Gyokeres",
                Email = "gyokeresSporting@gmail.com",
                DateOfBirth = new DateTime(1998, 6, 4)
            };


            var result = await service.SearchPatientsAsync("Joao", null, new DateTime(1993, 1, 19), null);

            Assert.Single(result);


            Assert.Equal("Joao Mario", result[0].FullName);
            Assert.Equal(new DateTime(1993, 1, 19), result[0].DateOfBirth);
            Assert.NotEqual(new DateTime(2000, 1, 3), result[0].DateOfBirth);
        }
    }
}