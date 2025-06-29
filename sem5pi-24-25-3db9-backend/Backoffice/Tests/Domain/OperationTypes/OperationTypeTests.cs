using System;
using System.Collections.Generic;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Specializations;
using Backoffice.Domain.Shared;
using Moq;
using Xunit;

namespace Backoffice.Tests
{
    public class OperationTypeTests
    {

        [Fact]
        public void TestOperationTypeWithValidInputs()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2),
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Cardio"),"code","description"), 3)
            };

            var operationType = new OperationType(name, duration, requiredStaff);

            Assert.Equal(name, operationType.Name);
            Assert.Equal(duration, operationType.Duration);
            Assert.Equal(requiredStaff, operationType.RequiredStaff);
            Assert.True(operationType.Active);
            Assert.NotNull(operationType.Id);
        }

        [Fact]
        public void TestOperationTypeWithNullName()
        {
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                new OperationType(null, duration, requiredStaff));
            Assert.Equal("Error: The operation type name can't be null.", exception.Message);
        }

        [Fact]
        public void TestOperationTypeWithEmptyName()
        {
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                new OperationType(new OperationTypeName(""), duration, requiredStaff));
            Assert.Equal("Error: The operation type name can't be null, empty or consist in only white spaces.", exception.Message);
        }

        [Fact]
        public void TestOperationTypeWithNullDuration()
        {
            var name = new OperationTypeName("Appendectomy");
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                new OperationType(name, null, requiredStaff));
            Assert.Equal("Error: The operation type duration can't be null.", exception.Message);
        }

        [Fact]
        public void TestOperationTypeWithEmptyRequiredStaff()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);

            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                new OperationType(name, duration, new List<OperationTypeRequiredStaff>()));

            Assert.Equal("Error: The operation type must have at least one required staff.", exception.Message);
        }

        [Fact]
        public void TestOperationTypeWithSingleValidRequiredStaff()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var operationType = new OperationType(name, duration, requiredStaff);

            Assert.Equal(name, operationType.Name);
            Assert.Equal(duration, operationType.Duration);
            Assert.Single(operationType.RequiredStaff);
        }


        //After teoric class

        [Fact]
        public void WhenTryingToMarkAsInativeWhileActive_ThenChangeStatus()
        {
            Mock<OperationTypeName> nameDouble = new Mock<OperationTypeName>();
            Mock<OperationTypeDuration> durationDouble = new Mock<OperationTypeDuration>();
            var opReqStaffMock = new Mock<OperationTypeRequiredStaff>();
            var requiredStaffList = new List<OperationTypeRequiredStaff>
            {
                opReqStaffMock.Object
            };

            OperationType opType = new OperationType(nameDouble.Object, durationDouble.Object, requiredStaffList);

            Assert.True(opType.Active);
            opType.MarkAsInative();
            Assert.False(opType.Active);
        }

        [Fact]
        public void WhenTryingToMarkAsInativeWhileAlreadyInactive_ThenException()
        {
            Mock<OperationTypeName> nameDouble = new Mock<OperationTypeName>();
            Mock<OperationTypeDuration> durationDouble = new Mock<OperationTypeDuration>();
            var opReqStaffMock = new Mock<OperationTypeRequiredStaff>();
            var requiredStaffList = new List<OperationTypeRequiredStaff>
            {
                opReqStaffMock.Object
            };

            OperationType opType = new OperationType(nameDouble.Object, durationDouble.Object, requiredStaffList);
            opType.MarkAsInative();

            var ex = Assert.Throws<BusinessRuleValidationException>(() => opType.MarkAsInative());
            Assert.Equal("Error: The operation type is already inactive.", ex.Message);

        }

        [Fact]
        public void WhenPassingNameDurationAndRequiredStaff_ThenOperationTypeIsInstantiated()
        {

            Mock<OperationTypeName> opNameDouble = new Mock<OperationTypeName>();
            Mock<OperationTypeDuration> opDurationDouble = new Mock<OperationTypeDuration>();
            var opReqStaffMock = new Mock<OperationTypeRequiredStaff>();
            var opReqList = new List<OperationTypeRequiredStaff>
            {
                opReqStaffMock.Object
            };

            new OperationType(opNameDouble.Object, opDurationDouble.Object, opReqList);
        }

        [Fact]
        public void WhenPassingNullAsOperationTypeName_ThenThrowsException()
        {
            Mock<OperationTypeDuration> opDurationDouble = new Mock<OperationTypeDuration>();
            Mock<List<OperationTypeRequiredStaff>> opReqDouble = new Mock<List<OperationTypeRequiredStaff>>();

            var ex = Assert.Throws<BusinessRuleValidationException>(() => new OperationType(null, opDurationDouble.Object, opReqDouble.Object));
            Assert.Equal("Error: The operation type name can't be null.", ex.Message);
        }

        [Fact]
        public void WhenPassingNullAsOperationTypeDuration_ThenThrowsException()
        {
            Mock<OperationTypeName> opNameDouble = new Mock<OperationTypeName>();
            Mock<List<OperationTypeRequiredStaff>> opReqDouble = new Mock<List<OperationTypeRequiredStaff>>();

            var ex = Assert.Throws<BusinessRuleValidationException>(() => new OperationType(opNameDouble.Object, null, opReqDouble.Object));
            Assert.Equal("Error: The operation type duration can't be null.", ex.Message);
        }

        [Fact]
        public void WhenPassingNullAsOperationTypeRequiredStaff_ThenThrowsException()
        {
            Mock<OperationTypeName> opNameDouble = new Mock<OperationTypeName>();
            Mock<OperationTypeDuration> opDurationDouble = new Mock<OperationTypeDuration>();

            var ex = Assert.Throws<BusinessRuleValidationException>(() => new OperationType(opNameDouble.Object, opDurationDouble.Object, null));
            Assert.Equal("Error: The operation type can't have null required staff.", ex.Message);
        }

        [Fact]
        public void WhenPassingEmptyAsOperationTypeRequiredStaff_ThenThrowsException()
        {
            Mock<OperationTypeName> opNameDouble = new Mock<OperationTypeName>();
            Mock<OperationTypeDuration> opDurationDouble = new Mock<OperationTypeDuration>();
            var opReqList = new List<OperationTypeRequiredStaff>();

            var ex = Assert.Throws<BusinessRuleValidationException>(() => new OperationType(opNameDouble.Object, opDurationDouble.Object, opReqList));
            Assert.Equal("Error: The operation type must have at least one required staff.", ex.Message);
        }

        [Fact]
        public void WhenRequestingOperationTypeName_ThenReturnOperationTypeName()
        {
            string expectedName = "OpTypeA";

            var opMock = new Mock<OperationTypeName>();

            opMock.Setup(s => s.Name).Returns(expectedName);

            string actualName = opMock.Object.Name;

            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void WhenRequestingOperationTypeDuration_ThenReturnOperationTypeDuration()
        {
            TimeSpan phase1 = TimeSpan.FromMinutes(20);
            TimeSpan phase2 = TimeSpan.FromMinutes(60);
            TimeSpan phase3 = TimeSpan.FromMinutes(30);

            var opMock = new Mock<OperationTypeDuration>();

            opMock.Setup(s => s.AnesthesiaPatientPreparationInMinutes).Returns(phase1);
            opMock.Setup(s => s.SurgeryInMinutes).Returns(phase2);
            opMock.Setup(s => s.CleaningInMinutes).Returns(phase3);

            TimeSpan actualPhase1 = opMock.Object.AnesthesiaPatientPreparationInMinutes;
            TimeSpan actualPhase2 = opMock.Object.SurgeryInMinutes;
            TimeSpan actualPhase3 = opMock.Object.CleaningInMinutes;


            Assert.Equal(phase1, actualPhase1);
            Assert.Equal(phase2, actualPhase2);
            Assert.Equal(phase3, actualPhase3);
        }

        [Fact]
        public void WhenRequestingActiveStatus_ThenReturnTrue()
        {
            var opMock = new Mock<OperationType>();

            opMock.Setup(s => s.Active).Returns(true);

            bool status = opMock.Object.Active;

            Assert.True(status);
        }

        [Fact]
        public void WhenRequestingActiveStatus_AfterMarkAsInativeWhileActive_ThenReturnFalse()
        {
            var opMock = new Mock<OperationType>();
            opMock.Setup(s => s.Active).Returns(true);
            opMock.Object.MarkAsInative();
            opMock.Setup(s => s.Active).Returns(false);
            bool status = opMock.Object.Active;

            Assert.False(status);
        }

        [Fact]
        public void WhenRequestingActiveStatus_AfterMarkAsInativeWhileInactive_ThenReturnThrow()
        {
            var opMock = new Mock<OperationType>();
            opMock.Setup(s => s.Active).Returns(false);

            bool status = opMock.Object.Active;

            var ex = Assert.Throws<BusinessRuleValidationException>(() => opMock.Object.MarkAsInative());
            Assert.Equal("Error: The operation type is already inactive.", ex.Message);
        }

        [Fact]
        public void ChangeName_ShouldUpdateName_WhenOperationTypeIsActive()
        {
            var operationType = new OperationType();
            var newName = new OperationTypeName("New Operation");

            operationType.ChangeName(newName);

            Assert.Equal(newName, operationType.Name);
        }

        [Fact]
        public void ChangeName_ShouldThrowException_WhenOperationTypeIsInactive()
        {
            var operationType = new OperationType();
            operationType.MarkAsInative();
            var newName = new OperationTypeName("New Operation");

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeName(newName));
            Assert.Equal("Error: Can't update an inactive operation type.", ex.Message);
        }

        [Fact]
        public void ChangeAnesthesiaPatientPreparationDuration_ShouldUpdateDuration_WhenValid()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var operationType = new OperationType(name, duration, requiredStaff);

            operationType.ChangeAnesthesiaPatientPreparationDuration(100);

            Assert.Equal(TimeSpan.FromMinutes(100), operationType.Duration.AnesthesiaPatientPreparationInMinutes);
        }

        [Fact]
        public void ChangeAnesthesiaPatientPreparationDuration_ShouldThrowException_WhenOperationTypeIsInactive()
        {
            var operationType = new OperationType();
            operationType.MarkAsInative();
            int validDuration = 30;

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeAnesthesiaPatientPreparationDuration(validDuration));
            Assert.Equal("Error: Can't update an inactive operation type.", ex.Message);
        }

        [Fact]
        public void ChangeAnesthesiaPatientPreparationDuration_ShouldThrowException_WhenDurationIsInvalid()
        {
            var operationType = new OperationType();
            int invalidDuration = 0;

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeAnesthesiaPatientPreparationDuration(invalidDuration));
            Assert.Equal("Error: The anesthesia/preparation duration must be more than 0 minutes.", ex.Message);
        }

        [Fact]
        public void ChangeSurgeryDuration_ShouldUpdateDuration_WhenValid()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var operationType = new OperationType(name, duration, requiredStaff);

            operationType.ChangeSurgeryDuration(100);

            Assert.Equal(TimeSpan.FromMinutes(100), operationType.Duration.SurgeryInMinutes);
        }

        [Fact]
        public void ChangeSurgeryDuration_ShouldThrowException_WhenOperationTypeIsInactive()
        {
            var operationType = new OperationType();
            operationType.MarkAsInative();
            int validDuration = 60;

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeSurgeryDuration(validDuration));
            Assert.Equal("Error: Can't update an inactive operation type.", ex.Message);
        }

        [Fact]
        public void ChangeSurgeryDuration_ShouldThrowException_WhenDurationIsInvalid()
        {
            var operationType = new OperationType();
            int invalidDuration = 0;

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeSurgeryDuration(invalidDuration));
            Assert.Equal("Error: The surgery duration must be more than 0 minutes.", ex.Message);
        }

        [Fact]
        public void ChangeCleaningDuration_ShouldUpdateDuration_WhenValid()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var operationType = new OperationType(name, duration, requiredStaff);

            operationType.ChangeCleaningDuration(100);

            Assert.Equal(TimeSpan.FromMinutes(100), operationType.Duration.CleaningInMinutes);
        }

        [Fact]
        public void ChangeCleaningDuration_ShouldThrowException_WhenOperationTypeIsInactive()
        {
            var operationType = new OperationType();
            operationType.MarkAsInative();
            int validDuration = 15;

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeCleaningDuration(validDuration));
            Assert.Equal("Error: Can't update an inactive operation type.", ex.Message);
        }

        [Fact]
        public void ChangeCleaningDuration_ShouldThrowException_WhenDurationIsInvalid()
        {
            var operationType = new OperationType();
            int invalidDuration = 0;

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeCleaningDuration(invalidDuration));
            Assert.Equal("Error: The cleaning duration must be more than 0 minutes.", ex.Message);
        }

        [Fact]
        public void ChangeRequiredStaff_ShouldUpdateStaff_WhenValid()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var staffList = new List<OperationTypeRequiredStaff>();
            staffList.Add(new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Brain"),"code","description"), 4));
            staffList.Add(new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Leg"),"code","description"), 10));

            var operationType = new OperationType(name, duration, requiredStaff);

            operationType.ChangeRequiredStaff(staffList);

            Assert.Equal(staffList, operationType.RequiredStaff);
        }

        [Fact]
        public void ChangeRequiredStaff_ShouldThrowException_WhenOperationTypeIsInactive()
        {
            var operationType = new OperationType();
            operationType.MarkAsInative();
            var staffList = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeRequiredStaff(staffList));
            Assert.Equal("Error: Can't update an inactive operation type.", ex.Message);
        }

        [Fact]
        public void ChangeRequiredStaff_ShouldThrowException_WhenStaffListIsEmpty()
        {
            var operationType = new OperationType();
            var emptyStaffList = new List<OperationTypeRequiredStaff>();

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeRequiredStaff(emptyStaffList));
            Assert.Equal("Error: Required staff can't be empty.", ex.Message);
        }

        [Fact]
        public void ChangeAll_ShouldUpdateAllFields_WhenValid()
        {
            var name = new OperationTypeName("Appendectomy");
            var duration = new OperationTypeDuration(10, 60, 20);
            var requiredStaff = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };

            var operationType = new OperationType(name, duration, requiredStaff);

            
            var staffList = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };
            var newName = new OperationTypeName("Updated Operation");
            int anesthesiaDuration = 30;
            int surgeryDuration = 60;
            int cleaningDuration = 15;

            operationType.ChangeAll(newName, anesthesiaDuration, surgeryDuration, cleaningDuration, staffList);

            Assert.Equal(newName, operationType.Name);
            Assert.Equal(TimeSpan.FromMinutes(anesthesiaDuration), operationType.Duration.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(TimeSpan.FromMinutes(surgeryDuration), operationType.Duration.SurgeryInMinutes);
            Assert.Equal(TimeSpan.FromMinutes(cleaningDuration), operationType.Duration.CleaningInMinutes);
            Assert.Equal(staffList, operationType.RequiredStaff);
        }

        [Fact]
        public void ChangeAll_ShouldThrowException_WhenOperationTypeIsInactive()
        {
            var operationType = new OperationType();
            operationType.MarkAsInative();
            var staffList = new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            };
            var newName = new OperationTypeName("Updated Operation");

            var ex = Assert.Throws<BusinessRuleValidationException>(() => operationType.ChangeAll(newName, 30, 60, 15, staffList));
            Assert.Equal("Error: Can't update an inactive operation type.", ex.Message);
        }






    }
}
