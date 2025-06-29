using System;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Specializations;
using Moq;
using Xunit;

namespace Backoffice.Tests;
public class OperationTypeRequiredStaffTests
{
    [Fact]
    public void TestOperationTypeRequiredStaffWithValidInputs()
    {
        var specializationName = new SpecializationName("Cardiology");
        var specialization = new Specialization(specializationName,"code","description");

        var operationTypeRequiredStaff = new OperationTypeRequiredStaff(specialization, 3);

        Assert.Equal(specializationName.Name, operationTypeRequiredStaff.Specialization.Name.Name);
        Assert.Equal(3, operationTypeRequiredStaff.Total);
    }

    [Fact]
    public void TestOperationTypeRequiredStaffWithNullSpecialization()
    {

        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeRequiredStaff(null, 3));

        Assert.Equal("Error: The specialization can't be null.", exception.Message);
    }

    [Fact]
    public void TestOperationTypeRequiredStaffWithTotalEqualTo0()
    {
        var specializationName = new SpecializationName("Cardiology");
        var specialization = new Specialization(specializationName,"code","description");
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeRequiredStaff(specialization, 0));

        Assert.Equal("Error: The total number of required staff of a specialization can't be lower or equal to 0.", exception.Message);
    }

    //After teoric class

    [Fact]
    public void WhenPassingASpecializationAndTotal_ThenOperationTypeRequiredStaffIsInstantiated()
    {

        Mock<Specialization> specDouble = new Mock<Specialization>();
        int totalDouble = 5;

        new OperationTypeRequiredStaff(specDouble.Object, totalDouble);
    }

    [Fact]
    public void WhenPassingNullAsSpecialization_ThenThrowsException()
    {
        var ex = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeRequiredStaff(null, 10));
        Assert.Equal("Error: The specialization can't be null.", ex.Message);

    }

    [Fact]
    public void WhenPassingNullAsTotal_ThenThrowsException()
    {
        Mock<Specialization> specDouble = new Mock<Specialization>();
        var ex = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeRequiredStaff(specDouble.Object, 0));
        Assert.Equal("Error: The total number of required staff of a specialization can't be lower or equal to 0.", ex.Message);
    }

    [Fact]
    public void WhenRequestingSpecializationName_ThenReturnSpecializationName()
    {
        string expectedName = "SpecializationA";

        var specMock = new Mock<Specialization>();

        var specializationName = new SpecializationName(expectedName);
        specMock.Setup(s => s.Name).Returns(specializationName);

        var reqStaff = new OperationTypeRequiredStaff(specMock.Object, 1);
        string actualName = reqStaff.Specialization.Name.Name;

        Assert.Equal(expectedName, actualName);
    }

  

}
