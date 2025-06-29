using System;
using Backoffice.Domain.Specializations;
using Backoffice.Domain.Shared;
using Xunit;

namespace Backoffice.Tests;

public class SpecializationNameTests
{
    [Fact]
    public void TestSpecializationNameWithValidInputs()
    {
        var validName = "Cardiology";

        var specializationName = new SpecializationName(validName);

        Assert.Equal(validName, specializationName.Name);
    }

    [Fact]
    public void TestSpecializationNameWithNullName()
    {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new SpecializationName(null));

        Assert.Equal("Error: The specialization name can't be null, empty or consist in only white spaces.", exception.Message);
    }

    [Fact]
    public void TestSpecializationNameWithEmptyName()
    {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new SpecializationName(""));

        Assert.Equal("Error: The specialization name can't be null, empty or consist in only white spaces.", exception.Message);
    }

    [Fact]
    public void TestSpecializationNameWithWhiteSpacesName()
    {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new SpecializationName("   "));

        Assert.Equal("Error: The specialization name can't be null, empty or consist in only white spaces.", exception.Message);
    }

    //After teoric class

    [Theory]
    [InlineData("Cardiology")]
    [InlineData("Cardio - logy")]
    [InlineData("asdasda1223dsg")]
    public void WhenPassingCorrectData_ThenSpecializationNameIsInstantiated(string name)
    {
        new SpecializationName(name);
    }

    [Theory]
    [InlineData("       ")]
    [InlineData("")]
    [InlineData(null)]
    public void WhenPassingInvalidAnesthesiaPatientPreparationInMinutes_ThenThrowsException(string name)
    {
        var ex = Assert.Throws<BusinessRuleValidationException>(() =>

            new SpecializationName(name)
        );
        Assert.Equal("Error: The specialization name can't be null, empty or consist in only white spaces.", ex.Message);
    }


}
