using System;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Xunit;

namespace Backoffice.Tests;
public class OperationTypeNameTests
{
    [Fact]
    public void TestOperationTypeNameWithValidInputs()
    {
        var validName = "Surgery A";

        var operationTypeName = new OperationTypeName(validName);

        Assert.Equal(validName, operationTypeName.Name);
    }

    [Fact]
    public void TestOperationTypeNameWithNullName()
    {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeName(null));

        Assert.Equal("Error: The operation type name can't be null, empty or consist in only white spaces.", exception.Message);
    }

    [Fact]
    public void TestOperationTypeNameWithEmptyName()
    {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeName(""));

        Assert.Equal("Error: The operation type name can't be null, empty or consist in only white spaces.", exception.Message);
    }

    [Fact]
    public void TestOperationTypeNameWithWhiteSpacesName()
    {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeName("   "));

        Assert.Equal("Error: The operation type name can't be null, empty or consist in only white spaces.", exception.Message);
    }


    //After teoric class

    [Theory]
    [InlineData("asdasdas")]
    [InlineData("Operation Type B")]
    [InlineData("ASGA  ASDASA SDS")]
    public void WhenPassingCorrectData_ThenOperationTypeNameIsInstantiated(string name)
    {
        new OperationTypeName(name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("        ")]
    public void WhenPassingInvalidName_ThenThrowsException(string name)
    {
        var ex = Assert.Throws<BusinessRuleValidationException>(() =>

            new OperationTypeName(name)
        );
        Assert.Equal("Error: The operation type name can't be null, empty or consist in only white spaces.", ex.Message);
    }
}
