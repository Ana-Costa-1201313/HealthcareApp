using System;
using Backoffice.Domain.Specializations;
using Backoffice.Domain.Shared;
using Moq;
using Xunit;

namespace Backoffice.Tests;

public class SpecializationTests
{

    [Fact]
    public void TestSpecializationWithValidSpecializationName()
    {
        var specializationName = new SpecializationName("Cardiology");

        var specialization = new Specialization(specializationName, "code", "description");

        Assert.Equal(specializationName, specialization.Name);
        Assert.True(specialization.Active);
        Assert.NotNull(specialization.Id);
    }

    [Fact]
    public void TestChangeSpecializationNameWhenSpecializationActive()
    {
        var specializationName = new SpecializationName("Cardiology");
        var specialization = new Specialization(specializationName, "code", "description");

        var newSpecializationName = new SpecializationName("Neurology");

        specialization.ChangeName(newSpecializationName);

        Assert.Equal(newSpecializationName, specialization.Name);
    }

    [Fact]
    public void TestChangeSpecializationNameWhenSpecializationInative()
    {
        var specializationName = new SpecializationName("Cardiology");
        var specialization = new Specialization(specializationName, "code", "description");

        var newSpecializationName = new SpecializationName("Neurology");

        specialization.MarkAsInative();

        var exception = Assert.Throws<BusinessRuleValidationException>(() => specialization.ChangeName(newSpecializationName));
        Assert.Equal("Error: It is not possible to change the name of an inactive specialization.", exception.Message);
    }

    [Fact]
    public void TestMarkSpecializationAsInative()
    {
        var specializationName = new SpecializationName("Cardiology");
        var specialization = new Specialization(specializationName, "code", "description");

        specialization.MarkAsInative();

        Assert.False(specialization.Active);
    }

    [Fact]
    public void TestMarkInativeSpecializationAsInative()
    {
        var specializationName = new SpecializationName("Cardiology");
        var specialization = new Specialization(specializationName, "code", "description");

        specialization.MarkAsInative();

        var exception = Assert.Throws<BusinessRuleValidationException>(() => specialization.MarkAsInative());
        Assert.Equal("Error: This specialization is already inactive.", exception.Message);
    }

    //After teoric class

    [Fact]
    public void WhenPassingASpecializationName_ThenSpecializationIsInstantiated()
    {

        Mock<SpecializationName> specNameDouble = new Mock<SpecializationName>();

        new Specialization(specNameDouble.Object,"code","description");
    }

    [Fact]
    public void WhenPassingNullAsSpecializationName_ThenThrowsException()
    {
        var ex = Assert.Throws<BusinessRuleValidationException>(() => new Specialization(null,null,null));
        Assert.Equal("Error: The specialization name can't be null.", ex.Message);
    }

    [Fact]
    public void WhenRequestingSpecializationName_ThenReturnSpecializationName()
    {
        string expectedName = "SpecializationA";

        var specMock = new Mock<SpecializationName>();

        specMock.Setup(s => s.Name).Returns(expectedName);

        string actualName = specMock.Object.Name;

        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void WhenChangingNameForActiveSpecialization_ThenNameIsChanged()
    {
        var oldName = new Mock<SpecializationName>("OldName");
        var newName = new Mock<SpecializationName>("NewName");

        var specialization = new Specialization(oldName.Object,"code","description");

        specialization.ChangeName(newName.Object);

        Assert.Equal(newName.Object, specialization.Name);
    }

    [Fact]
    public void WhenChangingNameForInactiveSpecialization_ThenThrowsException()
    {
        var oldName = new Mock<SpecializationName>("OldName");
        var newName = new Mock<SpecializationName>("NewName");

        var specialization = new Specialization(oldName.Object,"code","description");
        specialization.MarkAsInative();

        var exception = Assert.Throws<BusinessRuleValidationException>(() => specialization.ChangeName(newName.Object));

        Assert.Equal("Error: It is not possible to change the name of an inactive specialization.", exception.Message);
    }

    [Fact]
    public void WhenMarkingSpecializationAsInactive_ThenSpecializationBecomesInactive()
    {
        var specializationName = new Mock<SpecializationName>("SpecializationA");
        var specialization = new Specialization(specializationName.Object,"code","description");

        specialization.MarkAsInative();

        Assert.False(specialization.Active);
    }

    [Fact]
    public void WhenMarkingSpecializationAsInactiveTwice_ThenThrowsException()
    {
        var specializationName = new Mock<SpecializationName>("SpecializationA");
        var specialization = new Specialization(specializationName.Object,"code","description");
        specialization.MarkAsInative();

        var exception = Assert.Throws<BusinessRuleValidationException>(() => specialization.MarkAsInative());

        Assert.Equal("Error: This specialization is already inactive.", exception.Message);
    }




}
