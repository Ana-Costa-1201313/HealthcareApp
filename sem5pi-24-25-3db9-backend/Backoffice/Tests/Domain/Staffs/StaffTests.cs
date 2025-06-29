using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Specializations;
using Xunit;

namespace Backoffice.Tests
{
    public class StaffTests
    {
        [Fact]
        public void ValidStaff()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            string sName = "ana costa";
            int sLicenseNumber = 1;
            PhoneNumber sPhone = new PhoneNumber("999999999");
            string sSpecialization = "skin";
            Role sRole = Role.Nurse;
            int MecNumSequence = 1;
            MechanographicNumber MechanographicNum = new MechanographicNumber(sRole.ToString(), dto.RecruitmentYear, MecNumSequence);
            Email Email = new Email(MechanographicNum + "@healthcareapp.com");

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var staff = new Staff(dto, spec, MecNumSequence, "healthcareapp.com");


            Assert.NotNull(staff.Id);

            Assert.Equal(sName, staff.Name);
            Assert.Equal(sLicenseNumber, staff.LicenseNumber.LicenseNum);
            Assert.Equal(sPhone.PhoneNum, staff.Phone.PhoneNum);
            Assert.Equal(sSpecialization, staff.Specialization.Name.Name);
            Assert.Equal(sRole, staff.Role);
            Assert.Equal(MecNumSequence, staff.MecNumSequence);
            Assert.Equal(MechanographicNum.MechanographicNum, staff.MechanographicNum.MechanographicNum);
            Assert.Equal(Email._Email, staff.Email._Email);
        }
       

        [Fact]
        public void NullName()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
               Name = null,
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "spec",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            int MecNumSequence = 1;

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: The staff must have a name!", exception.Message);
        }

        [Fact]
        public void EmptyName()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
              Name = "",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "spec",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };
            
            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            int MecNumSequence = 1;

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: The staff must have a name!", exception.Message);
        }

        [Fact]
        public void SmallPhone()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "99999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            int MecNumSequence = 1;

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: The phone number is invalid!", exception.Message);
        }

        [Fact]
        public void InvalidPhone()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "199999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };


            int MecNumSequence = 1;

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: The phone number is invalid!", exception.Message);
        }

        [Fact]
        public void WrongRole()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "spec",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Patient,
                RecruitmentYear = 2024
            };

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            int MecNumSequence = 1;

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: The staff role must be one of the following: Doctor, Nurse or Tech!", exception.Message);
        }

        [Fact]
        public void InvalidSlotFormat()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 - 2024 - 10 - 11T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };


            int MecNumSequence = 1;

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: Invalid Availability slot format!", exception.Message);
        }

        [Fact]
        public void InvalidMecNumSeq()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };


            int MecNumSequence = -1;

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: The mechanographic number must be bigger than zero!", exception.Message);
        }

        [Fact]
        public void InvalidRecYear()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "skin",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = -2024
            };


            int MecNumSequence = 1;

            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Staff(dto, spec, MecNumSequence, "healthcareapp.com"));
            Assert.Equal("Error: The year must be bigger than zero!", exception.Message);
        }

        [Fact]
        public void EditStaff()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "spec",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };


            Role sRole = Role.Nurse;
            int MecNumSequence = 1;
            MechanographicNumber MechanographicNum = new MechanographicNumber(sRole.ToString(), dto.RecruitmentYear, MecNumSequence);
            Email Email = new Email(MechanographicNum + "@healthcareapp.com");
            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");


            var staff = new Staff(dto, spec, MecNumSequence, "healthcareapp.com");

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

            staff.Edit(editDto, spec);

            Assert.NotNull(staff.Id);
           
            Assert.Equal("ana costa", staff.Name);
            Assert.Equal(1, staff.LicenseNumber.LicenseNum);
            Assert.Equal("999999991", staff.Phone.PhoneNum);
            Assert.Equal("skin", staff.Specialization.Name.Name);
            Assert.Equal(sRole, staff.Role);
            Assert.Equal(MecNumSequence, staff.MecNumSequence);
            Assert.Equal(MechanographicNum.MechanographicNum, staff.MechanographicNum.MechanographicNum);
            Assert.Equal(Email._Email, staff.Email._Email);
        }

        [Fact]
        public void NullPhoneEditStaff()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "spec",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };


            Role sRole = Role.Nurse;
            int MecNumSequence = 1;
            MechanographicNumber MechanographicNum = new MechanographicNumber(sRole.ToString(), dto.RecruitmentYear, MecNumSequence);
            Email Email = new Email(MechanographicNum + "@healthcareapp.com");
            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");


            var staff = new Staff(dto, spec, MecNumSequence, "healthcareapp.com");

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

            var exception = Assert.Throws<BusinessRuleValidationException>(() => staff.Edit(editDto, spec));

            Assert.Equal("Error: The staff must have a phone number!", exception.Message);
        }

         [Fact]
        public void PartialEditStaff()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
                Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "spec",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };


            Role sRole = Role.Nurse;
            int MecNumSequence = 1;
            MechanographicNumber MechanographicNum = new MechanographicNumber(sRole.ToString(), dto.RecruitmentYear, MecNumSequence);
            Email Email = new Email(MechanographicNum + "@healthcareapp.com");
            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");


            var staff = new Staff(dto, spec, MecNumSequence, "healthcareapp.com");

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

            staff.PartialEdit(editDto, spec);

            Assert.NotNull(staff.Id);
           
            Assert.Equal("ana costa", staff.Name);
            Assert.Equal(1, staff.LicenseNumber.LicenseNum);
            Assert.Equal("999999991", staff.Phone.PhoneNum);
            Assert.Equal("skin", staff.Specialization.Name.Name);
            Assert.Equal(sRole, staff.Role);
            Assert.Equal(MecNumSequence, staff.MecNumSequence);
            Assert.Equal(MechanographicNum.MechanographicNum, staff.MechanographicNum.MechanographicNum);
            Assert.Equal(Email._Email, staff.Email._Email);
        }

        [Fact]
        public void NullPhonePartialEditStaff()
        {
            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto = new CreateStaffDto
            {
               Name = "ana costa",
                LicenseNumber = 1,
                Phone = "999999999",
                Specialization = "spec",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };


            Role sRole = Role.Nurse;
            int MecNumSequence = 1;
            MechanographicNumber MechanographicNum = new MechanographicNumber(sRole.ToString(), dto.RecruitmentYear, MecNumSequence);
            Email Email = new Email(MechanographicNum + "@healthcareapp.com");
            Specialization spec = new Specialization(new SpecializationName("skin"),"code","description");


            var staff = new Staff(dto, spec, MecNumSequence, "healthcareapp.com");

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

            staff.PartialEdit(editDto, spec);

            Assert.NotNull(staff.Id);
            Assert.Equal("ana costa", staff.Name);
            Assert.Equal(1, staff.LicenseNumber.LicenseNum);
            Assert.Equal("999999999", staff.Phone.PhoneNum);
            Assert.Equal("skin", staff.Specialization.Name.Name);
            Assert.Equal(sRole, staff.Role);
            Assert.Equal(MecNumSequence, staff.MecNumSequence);
            Assert.Equal(MechanographicNum.MechanographicNum, staff.MechanographicNum.MechanographicNum);
            Assert.Equal(Email._Email, staff.Email._Email);
        }
    }
}