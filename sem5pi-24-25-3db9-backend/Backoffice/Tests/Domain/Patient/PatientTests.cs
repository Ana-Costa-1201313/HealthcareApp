using Backoffice.Domain.Patients;
using Backoffice.Domain.Shared;
using Microsoft.AspNetCore.Routing.Constraints;
using Xunit;

namespace Backoffice.Tests
{
    public class PatientTests
    {
        [Fact]
        public void ValidPatientCreation()
        {
            var dto = new CreatePatientDto
            {
                FirstName = "Bernardo",
                LastName = "Silva",
                FullName = "Bernardo Silva",
                Gender = "M",
                DateOfBirth = new DateTime(1994,8,10),
                Email = "bernardoSilva@gmail.com",
                Phone = "919100000",
                EmergencyContact = "929200000"
            };
                                       
            var patient = new Patient(dto,"202410000001");

            Assert.NotNull(patient.Id);
            Assert.Equal(dto.FirstName,patient.FirstName);
            Assert.Equal(dto.LastName,patient.LastName);
            Assert.Equal(dto.FullName,patient.FullName);
            Assert.Equal(dto.Gender,patient.Gender);
            Assert.Equal(dto.Email,patient.Email._Email);
            Assert.Equal(dto.Phone,patient.Phone.PhoneNum);
            Assert.Equal(dto.EmergencyContact,patient.EmergencyContact.PhoneNum);
            Assert.Equal("202410000001",patient.MedicalRecordNumber);
        }

        [Fact]
        public void InvalidEmail()
        {
             var dto = new CreatePatientDto
            {
                FirstName = "Erling",
                LastName = "Haaland",
                FullName = "Erling Haaland",
                Gender = "M",
                DateOfBirth = new DateTime(2000,7,21),
                Email = "haaland",
                Phone = "929100000",
                EmergencyContact = "939200000"
            };

           var ex = Assert.Throws<BusinessRuleValidationException>(() => new Patient(dto, "202410000001"));
            Assert.Equal("Error: The email is invalid!",ex.Message);
        }
        [Fact]
        public void InvalidPhone()
        {
             var dto = new CreatePatientDto
            {
                FirstName = "Ruben",
                LastName = "Dias",
                FullName = "Ruben Dias",
                Gender = "M",
                DateOfBirth = new DateTime(1997,5,14),
                Email = "rubenDias@gmail.com",
                Phone = "9291",
                EmergencyContact = "939400000"
            };
            var ex = Assert.Throws<BusinessRuleValidationException>(() => new Patient(dto, "202410000001"));
            Assert.Equal("Error: The phone number is invalid!",ex.Message);
        }
        [Fact]
        public void InvalidEmergencyContact()
        {
             var dto = new CreatePatientDto
            {
                FirstName = "Phil",
                LastName = "Foden",
                FullName = "Phil Foden",
                Gender = "M",
                DateOfBirth = new DateTime(2000,5,28),
                Email = "philFoden@gmail.com",
                Phone = "999888777",
                EmergencyContact = "139111222"
            };
            var ex = Assert.Throws<BusinessRuleValidationException>(() => new Patient(dto, "202410000001"));
            Assert.Equal("Error: The phone number is invalid!",ex.Message);
        }

    }
}