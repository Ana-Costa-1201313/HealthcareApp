using System;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Xunit;

namespace Backoffice.Tests;
public class OperationTypeDurationTests
{
    [Fact]
    public void TestOperationTypeDurationWithValidInputs()
    {
        int anesthesiaPatientPreparationInMinutes = 30;
        int surgeryInMinutes = 120;
        int cleaningInMinutes = 60;

        var operationTypeDuration = new OperationTypeDuration(
            anesthesiaPatientPreparationInMinutes,
            surgeryInMinutes,
            cleaningInMinutes
        );

        Assert.Equal(TimeSpan.FromMinutes(anesthesiaPatientPreparationInMinutes), operationTypeDuration.AnesthesiaPatientPreparationInMinutes);
        Assert.Equal(TimeSpan.FromMinutes(surgeryInMinutes), operationTypeDuration.SurgeryInMinutes);
        Assert.Equal(TimeSpan.FromMinutes(cleaningInMinutes), operationTypeDuration.CleaningInMinutes);
    }

    [Fact]
    public void TestOperationTypeDurationWith0MinutesAnesthesia()
    {
        int anesthesiaPatientPreparationInMinutes = 0;
        int surgeryInMinutes = 120;
        int cleaningInMinutes = 60;

        var exception = Assert.Throws<BusinessRuleValidationException>(() =>
            new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, surgeryInMinutes, cleaningInMinutes));

        Assert.Equal("Error: The anesthesia/preparation duration must be more than 0 minutes.", exception.Message);
    }


    [Fact]
    public void TestOperationTypeDurationWith0MinutesSurgery()
    {
        int anesthesiaPatientPreparationInMinutes = 30;
        int surgeryInMinutes = 0;
        int cleaningInMinutes = 60;

        var exception = Assert.Throws<BusinessRuleValidationException>(() =>
            new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, surgeryInMinutes, cleaningInMinutes));

        Assert.Equal("Error: The surgery duration must be more than 0 minutes.", exception.Message);
    }


    [Fact]
    public void TestOperationTypeDurationWith0MinutesCleaning()
    {
        int anesthesiaPatientPreparationInMinutes = 30;
        int surgeryInMinutes = 120;
        int cleaningInMinutes = 0;

        var exception = Assert.Throws<BusinessRuleValidationException>(() =>
            new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, surgeryInMinutes, cleaningInMinutes));

        Assert.Equal("Error: The cleaning duration must be more than 0 minutes.", exception.Message);
    }



    [Fact]
    public void TestOperationTypeDurationWithNegativeInputs()
    {
        int anesthesiaPatientPreparationInMinutes = -10;
        int surgeryInMinutes = -30;
        int cleaningInMinutes = -5;

        var anesthesiaException = Assert.Throws<BusinessRuleValidationException>(() =>
            new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, 120, 60));

        Assert.Equal("Error: The anesthesia/preparation duration must be more than 0 minutes.", anesthesiaException.Message);

        var surgeryException = Assert.Throws<BusinessRuleValidationException>(() =>
            new OperationTypeDuration(30, surgeryInMinutes, 60));

        Assert.Equal("Error: The surgery duration must be more than 0 minutes.", surgeryException.Message);

        var cleaningException = Assert.Throws<BusinessRuleValidationException>(() =>
            new OperationTypeDuration(30, 120, cleaningInMinutes));

        Assert.Equal("Error: The cleaning duration must be more than 0 minutes.", cleaningException.Message);
    }


    //After teoric class

    [Theory]
    [InlineData(100, 20, 20)]
    [InlineData(50, 5, 55)]
    [InlineData(1000, 1000, 1000)]
    [InlineData(1, 1, 1)]
    public void WhenPassingCorrectData_ThenOperationTypeDurationIsInstantiated(int anesthesiaPatientPreparationInMinutes, int surgeryInMinutes, int cleaningInMinutes)
    {
        new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, surgeryInMinutes, cleaningInMinutes);
    }

    [Theory]
    [InlineData(0, 5, 55)]
    [InlineData(-1, 1000, 1000)]
    [InlineData(-1000, 1, 1)]
    public void WhenPassingInvalidAnesthesiaPatientPreparationInMinutes_ThenThrowsException(int anesthesiaPatientPreparationInMinutes, int surgeryInMinutes, int cleaningInMinutes)
    {
        var ex = Assert.Throws<BusinessRuleValidationException>(() =>

            new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, surgeryInMinutes, cleaningInMinutes)
        );
        Assert.Equal("Error: The anesthesia/preparation duration must be more than 0 minutes.", ex.Message);
    }

    [Theory]
    [InlineData(50, 0, 55)]
    [InlineData(1000, -1, 1000)]
    [InlineData(1, -1000, 1)]
    public void WhenPassingInvalidSurgeryInMinutes_ThenThrowsException(int anesthesiaPatientPreparationInMinutes, int surgeryInMinutes, int cleaningInMinutes)
    {
        var ex = Assert.Throws<BusinessRuleValidationException>(() =>

            new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, surgeryInMinutes, cleaningInMinutes)
        );
        Assert.Equal("Error: The surgery duration must be more than 0 minutes.", ex.Message);
    }

    [Theory]
    [InlineData(50, 5, 0)]
    [InlineData(1000, 1000, -1)]
    [InlineData(1, 1, -1000)]
    public void WhenPassingInvalidCleaningInMinutes_ThenThrowsException(int anesthesiaPatientPreparationInMinutes, int surgeryInMinutes, int cleaningInMinutes)
    {
        var ex = Assert.Throws<BusinessRuleValidationException>(() =>

            new OperationTypeDuration(anesthesiaPatientPreparationInMinutes, surgeryInMinutes, cleaningInMinutes)
        );
        Assert.Equal("Error: The cleaning duration must be more than 0 minutes.", ex.Message);
    }
}
