using System;
using Backoffice.Domain.Specializations;
using Backoffice.Domain.Shared;
using Moq;
using Xunit;
using Backoffice.Infraestructure.Specializations;
using Backoffice.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Tests;

public class SpecializationRepositoryMockTests
{
    private readonly Mock<ISpecializationRepository> _mockSpecializationRepository;


    public SpecializationRepositoryMockTests()
    {
        _mockSpecializationRepository = new Mock<ISpecializationRepository>();
    }

    [Fact]
    public async Task TestSpecializationNameExistsWithMock()
    {
        var specializationName = "Cardiology";
        _mockSpecializationRepository.Setup(repo => repo.SpecializationNameExists(specializationName))
            .ReturnsAsync(true);

        var exists = await _mockSpecializationRepository.Object.SpecializationNameExists(specializationName);

        Assert.True(exists);
        _mockSpecializationRepository.Verify(repo => repo.SpecializationNameExists(specializationName), Times.Once);
    }

    [Fact]
    public async Task TestSpecializationNameDoesNotExistWithMock()
    {
        var specializationName = "NonExistentSpecialization";
        _mockSpecializationRepository.Setup(repo => repo.SpecializationNameExists(specializationName))
            .ReturnsAsync(false);

        var exists = await _mockSpecializationRepository.Object.SpecializationNameExists(specializationName);

        Assert.False(exists);
        _mockSpecializationRepository.Verify(repo => repo.SpecializationNameExists(specializationName), Times.Once);
    }

    [Fact]
    public async Task TestGetSpecializationByNameWithMock()
    {
        var specializationName = "Cardiology";
        var specialization = new Specialization(new SpecializationName(specializationName),"code","description");
        _mockSpecializationRepository.Setup(repo => repo.GetBySpecializationName(specializationName))
            .ReturnsAsync(specialization);

        var result = await _mockSpecializationRepository.Object.GetBySpecializationName(specializationName);

        Assert.NotNull(result);
        Assert.Equal(specializationName, result.Name.Name);
        _mockSpecializationRepository.Verify(repo => repo.GetBySpecializationName(specializationName), Times.Once);
    }

    [Fact]
    public async Task TestGetSpecializationByNameNotFoundWithMock()
    {
        var specializationName = "NonExistentSpecialization";
        _mockSpecializationRepository.Setup(repo => repo.GetBySpecializationName(specializationName))
            .ReturnsAsync((Specialization)null);

        var result = await _mockSpecializationRepository.Object.GetBySpecializationName(specializationName);

        Assert.Null(result);
        _mockSpecializationRepository.Verify(repo => repo.GetBySpecializationName(specializationName), Times.Once);
    }
}
