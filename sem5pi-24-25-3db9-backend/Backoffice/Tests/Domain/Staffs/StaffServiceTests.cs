using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Specializations;
using Xunit;
using Backoffice.Infraestructure.Staffs;
using Backoffice.Infraestructure;
using System.Runtime.CompilerServices;
using Backoffice.Domain.Logs;


namespace Backoffice.Tests
{
    public class StaffServiceTests
    {
        Mock<IStaffRepository> staffRepository;

        Mock<IUnitOfWork> unitOfWork;

        Mock<ISpecializationRepository> specRepository;

        Mock<ILogRepository> logRepository;

        Mock<IConfiguration> config;

        Mock<IEmailService> emailService;

        Mock<StaffService> mockService;

        private void Setup(List<Staff> staffDb, List<Specialization> specializationsDb)
        {
            staffRepository = new Mock<IStaffRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            specRepository = new Mock<ISpecializationRepository>();
            logRepository = new Mock<ILogRepository>();
            config = new Mock<IConfiguration>();
            emailService = new Mock<IEmailService>();


            staffRepository.Setup(repo => repo.GetAllWithDetailsAsync(1, 5))
            .ReturnsAsync(staffDb);

            staffRepository.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<StaffId>()))
            .ReturnsAsync((StaffId id) => staffDb.SingleOrDefault(s => s.Id.Equals(id)));

            staffRepository.Setup(repo => repo.AddAsync(It.IsAny<Staff>()))
            .Callback<Staff>(s => staffDb.Add(s));

            staffRepository.Setup(repo => repo.GetLastMechanographicNumAsync())
            .ReturnsAsync(staffDb.DefaultIfEmpty().Max(s => s != null ? s.MecNumSequence : 0));

            staffRepository.Setup(repo => repo.GetStaffByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync((Email email) => staffDb.FirstOrDefault(s => s.Email == email));

            specRepository.Setup(repo => repo.GetBySpecializationName(It.IsAny<string>()))
            .ReturnsAsync(new Specialization(new SpecializationName("skin"),"code","description"));

            logRepository.Setup(repo => repo.AddAsync(It.IsAny<Log>()))
            .ReturnsAsync(new Mock<Log>().Object);

            unitOfWork.Setup(uow => uow.CommitAsync())
            .ReturnsAsync(0);

            config.Setup(c => c["EmailSmtp:DefaultStaffConfirmationEmail"])
            .Returns("1201313@isep.ipp.pt");

            mockService = new Mock<StaffService>(unitOfWork.Object, staffRepository.Object, new StaffMapper(), specRepository.Object, logRepository.Object, emailService.Object, config.Object);

        }

        [Fact]
        public async Task GetAllAsync()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            staffDb.Add(staff1);
            staffDb.Add(staff2);

            var result = await service.GetAllAsync(1, 5);

            staffRepository.Verify(repo => repo.GetAllWithDetailsAsync(1, 5), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("ana costa", result[0].Name);
            Assert.Equal(1, result[0].LicenseNumber);
            Assert.Equal("999999999", result[0].Phone);
            Assert.Equal("skin", result[0].Specialization);
            Assert.Equal(Role.Nurse, result[0].Role);

            Assert.Equal("maria silva", result[1].Name);
            Assert.Equal(2, result[1].LicenseNumber);
            Assert.Equal("999999989", result[1].Phone);
            Assert.Equal("skin", result[1].Specialization);
            Assert.Equal(Role.Doctor, result[1].Role);
        }

        [Fact]
        public async Task GetAllAsyncWithPageSize()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            List<Staff> list = new List<Staff>();
            list.Add(staff1);
            list.Add(staff2);
            list.Add(staff3);

            staffRepository.Setup(repo => repo.GetAllWithDetailsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(list);

            staffDb.Add(staff1);
            staffDb.Add(staff2);
            staffDb.Add(staff3);
            staffDb.Add(staff4);
            staffDb.Add(staff5);

            var result = await service.GetAllAsync(1, 3);

            staffRepository.Verify(repo => repo.GetAllWithDetailsAsync(1, 3), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);

            Assert.Equal("ana costa", result[0].Name);
            Assert.Equal(1, result[0].LicenseNumber);
            Assert.Equal("999999999", result[0].Phone);
            Assert.Equal("skin", result[0].Specialization);
            Assert.Equal(Role.Nurse, result[0].Role);

            Assert.Equal("maria silva", result[1].Name);
            Assert.Equal(2, result[1].LicenseNumber);
            Assert.Equal("999999989", result[1].Phone);
            Assert.Equal("skin", result[1].Specialization);
            Assert.Equal(Role.Doctor, result[1].Role);

            Assert.Equal("maria silva", result[2].Name);
            Assert.Equal(3, result[2].LicenseNumber);
            Assert.Equal("999999979", result[2].Phone);
            Assert.Equal("skin", result[2].Specialization);
            Assert.Equal(Role.Doctor, result[2].Role);
        }

        [Fact]
        public async Task GetAllAsyncWithPageNumAndPageSize()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            List<Staff> list = new List<Staff>();
            list.Add(staff4);
            list.Add(staff5);

            staffRepository.Setup(repo => repo.GetAllWithDetailsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(list);

            staffDb.Add(staff1);
            staffDb.Add(staff2);
            staffDb.Add(staff3);
            staffDb.Add(staff4);
            staffDb.Add(staff5);

            var result = await service.GetAllAsync(2, 3);

            staffRepository.Verify(repo => repo.GetAllWithDetailsAsync(2, 3), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("maria silva", result[0].Name);
            Assert.Equal(4, result[0].LicenseNumber);
            Assert.Equal("999999969", result[0].Phone);
            Assert.Equal("skin", result[0].Specialization);
            Assert.Equal(Role.Doctor, result[0].Role);

            Assert.Equal("maria silva", result[1].Name);
            Assert.Equal(5, result[1].LicenseNumber);
            Assert.Equal("999999959", result[1].Phone);
            Assert.Equal("skin", result[1].Specialization);
            Assert.Equal(Role.Doctor, result[1].Role);
        }

        [Fact]
        public async Task GetByIdAsync()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            staffDb.Add(staff1);
            staffDb.Add(staff2);

            var result = await service.GetByIdAsync(staff1.Id.AsGuid());

            staffRepository.Verify(repo => repo.GetByIdWithDetailsAsync(staff1.Id), Times.Once);

            Assert.NotNull(result);

            Assert.Equal("ana costa", result.Name);
            Assert.Equal(1, result.LicenseNumber);
            Assert.Equal("999999999", result.Phone);
            Assert.Equal("skin", result.Specialization);
            Assert.Equal(Role.Nurse, result.Role);
        }

        [Fact]
        public async Task InvalidGetByIdAsync()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            staffDb.Add(staff1);

            var result = await service.GetByIdAsync(staff2.Id.AsGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            var result = await service.AddAsync(dto1);

            staffRepository.Verify(repo => repo.GetLastMechanographicNumAsync(), Times.Once);

            Assert.NotNull(result);
            Assert.Equal("ana costa", result.Name);
            Assert.Equal(1, result.LicenseNumber);
            Assert.Equal("999999999", result.Phone);
            Assert.Equal("skin", result.Specialization);
            Assert.Equal(Role.Nurse, result.Role);
        }

        [Fact]
        public async Task Deactivate()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");

            staffDb.Add(staff1);

            var result = await service.Deactivate(staff1.Id.AsGuid());

            staffRepository.Verify(repo => repo.GetByIdWithDetailsAsync(staff1.Id), Times.Once);


            Assert.NotNull(result);

            Assert.Equal("Deactivated Staff", result.Name);
            Assert.Null(result.Phone);
            Assert.Null(result.Specialization);
            Assert.False(result.Active);
        }

        [Fact]
        public async Task Update()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;


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

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");

            staffDb.Add(staff1);

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff1.Id.AsGuid(),
                Phone = "999999991",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };
            var result = await service.UpdateAsync(editDto, false);

            staffRepository.Verify(repo => repo.GetByIdWithDetailsAsync(staff1.Id), Times.Once);

            Assert.NotNull(result);

            Assert.Equal("ana costa", result.Name);
            Assert.Equal("999999991", result.Phone);
            Assert.Equal("skin", result.Specialization);
        }

        [Fact]
        public async Task InactiveUpdate()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");

            staffDb.Add(staff1);

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff1.Id.AsGuid(),
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            await service.Deactivate(staff1.Id.AsGuid());

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () =>
                 await service.UpdateAsync(editDto, false));

            Assert.Equal("Error: Can't update an inactive staff!", exception.Message);

        }

        [Fact]
        public async Task NullPhoneUpdate()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");

            staffDb.Add(staff1);

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff1.Id.AsGuid(),
                Phone = null,
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () =>
                await service.UpdateAsync(editDto, false));

            Assert.Equal("Error: The staff must have a phone number!", exception.Message);
        }

        [Fact]
        public async Task PartialUpdate()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");

            staffDb.Add(staff1);

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff1.Id.AsGuid(),
                Phone = "999999991",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };
            var result = await service.UpdateAsync(editDto, true);

            staffRepository.Verify(repo => repo.GetByIdWithDetailsAsync(staff1.Id), Times.Once);

            Assert.NotNull(result);


            Assert.Equal("ana costa", result.Name);
            Assert.Equal("999999991", result.Phone);
            Assert.Equal("skin", result.Specialization);
        }

        [Fact]
        public async Task InactivePartialUpdate()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");

            staffDb.Add(staff1);

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff1.Id.AsGuid(),
                Phone = "999999991",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            await service.Deactivate(staff1.Id.AsGuid());

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () =>
                 await service.UpdateAsync(editDto, true));

            Assert.Equal("Error: Can't update an inactive staff!", exception.Message);

        }

        [Fact]
        public async Task NullPhonePartialUpdate()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            var staff1 = new Staff(dto1, spec, 1, "healthcareapp.com");

            staffDb.Add(staff1);

            List<string> AvailabilitySlots2 = new List<string>();
            AvailabilitySlots2.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots2.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            EditStaffDto editDto = new EditStaffDto
            {
                Id = staff1.Id.AsGuid(),
                Phone = null,
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots2
            };

            var result = await service.UpdateAsync(editDto, true);

            Assert.NotNull(result);

            Assert.Equal("ana costa", result.Name);
            Assert.Equal("999999999", result.Phone);
            Assert.Equal("skin", result.Specialization);
        }

        [Fact]
        public async Task FilterStaffByNameAsync()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            List<Staff> list = new List<Staff>();
            list.Add(staff1);

            staffRepository.Setup(repo => repo.FilterStaffAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(list);

            staffDb.Add(staff1);
            staffDb.Add(staff2);

            var result = await service.FilterStaffAsync("ana costa", null, null, 1, 5);

            staffRepository.Verify(repo => repo.FilterStaffAsync("ana costa", null, null, 1, 5), Times.Once);

            Assert.NotNull(result);
            Assert.Single(result);

            Assert.Equal("ana costa", result[0].Name);
            Assert.Equal(1, result[0].LicenseNumber);
            Assert.Equal("999999999", result[0].Phone);
            Assert.Equal("skin", result[0].Specialization);
            Assert.Equal(Role.Nurse, result[0].Role);
        }

        [Fact]
        public async Task FilterStaffByNameAndPageSizeAsync()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            List<Staff> list = new List<Staff>();
            list.Add(staff2);
            list.Add(staff3);
            list.Add(staff4);

            staffRepository.Setup(repo => repo.FilterStaffAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(list);

            staffDb.Add(staff1);
            staffDb.Add(staff2);
            staffDb.Add(staff3);
            staffDb.Add(staff4);
            staffDb.Add(staff5);
            staffDb.Add(staff6);
            staffDb.Add(staff7);

            var result = await service.FilterStaffAsync("maria silva", null, null, 1, 3);

            staffRepository.Verify(repo => repo.FilterStaffAsync("maria silva", null, null, 1, 3), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);

            Assert.Equal("maria silva", result[0].Name);
            Assert.Equal(2, result[0].LicenseNumber);
            Assert.Equal("999999989", result[0].Phone);
            Assert.Equal("skin", result[0].Specialization);
            Assert.Equal(Role.Doctor, result[0].Role);
        }


        [Fact]
        public async Task FilterStaffBySpecAndPageNum()
        {
            var staffDb = new List<Staff>();

            var specializationsDb = new List<Specialization>();

            Setup(staffDb, specializationsDb);

            mockService.CallBase = true;

            mockService.Setup(servico => servico.ReadDNS()).Returns("myhealthcareapp.com");

            StaffService service = mockService.Object;

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

            List<Staff> list = new List<Staff>();
            list.Add(staff4);
            list.Add(staff5);

            staffRepository.Setup(repo => repo.FilterStaffAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(list);

            staffDb.Add(staff1);
            staffDb.Add(staff2);
            staffDb.Add(staff3);
            staffDb.Add(staff4);
            staffDb.Add(staff5);
            staffDb.Add(staff6);
            staffDb.Add(staff7);

            var result = await service.FilterStaffAsync(null, null, "heart", 2, 3);

            staffRepository.Verify(repo => repo.FilterStaffAsync(null, null, "heart", 2, 3), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("maria silva", result[0].Name);
            Assert.Equal(4, result[0].LicenseNumber);
            Assert.Equal("999999969", result[0].Phone);
            Assert.Equal("heart", result[0].Specialization);
            Assert.Equal(Role.Doctor, result[0].Role);
        }

    }
}