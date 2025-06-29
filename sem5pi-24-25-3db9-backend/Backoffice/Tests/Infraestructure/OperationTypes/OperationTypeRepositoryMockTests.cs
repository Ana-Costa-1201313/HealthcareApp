using System;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Moq;
using Xunit;
using Backoffice.Infraestructure.OperationTypes;
using Backoffice.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Backoffice.Domain.Specializations;

namespace Backoffice.Tests;

public class OperationTypeRepositoryMockTests
{
    private readonly Mock<IOperationTypeRepository> _mockOperationTypeRepository;


    public OperationTypeRepositoryMockTests()
    {
        _mockOperationTypeRepository = new Mock<IOperationTypeRepository>();
    }

    [Fact]
    public async Task TestOperationTypeNameExistsWithMock()
    {
        var operationTypeName = "Operation Type A";
        _mockOperationTypeRepository.Setup(repo => repo.OperationTypeNameExists(operationTypeName))
            .ReturnsAsync(true);

        var exists = await _mockOperationTypeRepository.Object.OperationTypeNameExists(operationTypeName);

        Assert.True(exists);
        _mockOperationTypeRepository.Verify(repo => repo.OperationTypeNameExists(operationTypeName), Times.Once);
    }

    [Fact]
    public async Task TestOperationTypeNameDoesNotExistWithMock()
    {
        var operationTypeName = "NonExistentOperationType";
        _mockOperationTypeRepository.Setup(repo => repo.OperationTypeNameExists(operationTypeName))
            .ReturnsAsync(false);

        var exists = await _mockOperationTypeRepository.Object.OperationTypeNameExists(operationTypeName);

        Assert.False(exists);
        _mockOperationTypeRepository.Verify(repo => repo.OperationTypeNameExists(operationTypeName), Times.Once);
    }

    [Fact]
    public async Task TestGetAllWithDetailsAsync()
    {
        var operationTypes = new List<OperationType>
        {
            new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(30, 90, 15), new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            }),
            new OperationType(new OperationTypeName("Anesthesia"), new OperationTypeDuration(15, 30, 5), new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Anesthesiologist"),"code","description"), 5),
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 1)
            })
        };

        _mockOperationTypeRepository.Setup(repo => repo.GetAllWithDetailsAsync())
            .ReturnsAsync(operationTypes);

        var result = await _mockOperationTypeRepository.Object.GetAllWithDetailsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var surgery = result[0];
        Assert.Equal("Surgery", surgery.Name.Name);
        Assert.Equal(30, surgery.Duration.AnesthesiaPatientPreparationInMinutes.TotalMinutes);
        Assert.Equal(90, surgery.Duration.SurgeryInMinutes.TotalMinutes);
        Assert.Equal(15, surgery.Duration.CleaningInMinutes.TotalMinutes);
        Assert.Single(surgery.RequiredStaff);
        Assert.Equal("Surgeon", surgery.RequiredStaff[0].Specialization.Name.Name);
        Assert.Equal(2, surgery.RequiredStaff[0].Total);

        var anesthesia = result[1];
        Assert.Equal("Anesthesia", anesthesia.Name.Name);
        Assert.Equal(15, anesthesia.Duration.AnesthesiaPatientPreparationInMinutes.TotalMinutes);
        Assert.Equal(30, anesthesia.Duration.SurgeryInMinutes.TotalMinutes);
        Assert.Equal(5, anesthesia.Duration.CleaningInMinutes.TotalMinutes);
        Assert.Equal(2, anesthesia.RequiredStaff.Count);

        Assert.Equal("Anesthesiologist", anesthesia.RequiredStaff[0].Specialization.Name.Name);
        Assert.Equal(5, anesthesia.RequiredStaff[0].Total);

        Assert.Equal("Surgeon", anesthesia.RequiredStaff[1].Specialization.Name.Name);
        Assert.Equal(1, anesthesia.RequiredStaff[1].Total);

        _mockOperationTypeRepository.Verify(repo => repo.GetAllWithDetailsAsync(), Times.Once);
    }


    [Fact]
    public async Task TestGetByIdWithDetailsAsync_ShouldReturnOperationType()
    {

        var operationType = new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(30, 60, 15), new List<OperationTypeRequiredStaff>
            {
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
            });

        var operationTypeId = operationType.Id;

        _mockOperationTypeRepository.Setup(repo => repo.GetByIdWithDetailsAsync(operationTypeId))
            .ReturnsAsync(operationType);

        var result = await _mockOperationTypeRepository.Object.GetByIdWithDetailsAsync(operationTypeId);

        Assert.NotNull(result);
        Assert.Equal(operationTypeId, result.Id);
        Assert.Equal("Surgery", result.Name.Name);
        _mockOperationTypeRepository.Verify(repo => repo.GetByIdWithDetailsAsync(operationTypeId), Times.Once);
    }

    [Fact]
    public async Task TestGetByIdWithDetailsAsync_ShouldReturnNull()
    {
        var operationTypeId = new OperationTypeId(Guid.NewGuid());

        _mockOperationTypeRepository.Setup(repo => repo.GetByIdWithDetailsAsync(operationTypeId))
            .ReturnsAsync((OperationType)null);

        var result = await _mockOperationTypeRepository.Object.GetByIdWithDetailsAsync(operationTypeId);

        Assert.Null(result);
        _mockOperationTypeRepository.Verify(repo => repo.GetByIdWithDetailsAsync(operationTypeId), Times.Once);
    }

    [Fact]
    public async Task TestFilterOperationTypesAsync_ReturnsMatchingOperationTypes()
    {
        var operationTypesDatabase = new List<OperationType>
            {
                new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(30, 60, 15),
                    new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
                    }),
                new OperationType(new OperationTypeName("Anesthesia"), new OperationTypeDuration(15, 30, 5),
                    new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Anesthesiologist"),"code","description"), 5)
                    }),
                new OperationType(new OperationTypeName("General Surgery"), new OperationTypeDuration(40, 80, 20),
                    new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 1)
                    })
            };

        var nameFilter = "Surgery";
        var specializationFilter = "Surgeon";
        bool? statusFilter = null;

        _mockOperationTypeRepository.Setup(repo => repo.FilterOperationTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
            .ReturnsAsync((string name, string specialization, bool? status) =>
                operationTypesDatabase
                    .Where(op => (string.IsNullOrEmpty(name) || op.Name.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                                 (string.IsNullOrEmpty(specialization) || op.RequiredStaff.Any(rs => rs.Specialization.Name.Name.Equals(specialization, StringComparison.OrdinalIgnoreCase))) &&
                                 (!status.HasValue || op.Active == status))
                    .ToList());


        var result = await _mockOperationTypeRepository.Object.FilterOperationTypesAsync(nameFilter, specializationFilter, statusFilter);


        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, op => op.Name.Name == "Surgery");
        Assert.Contains(result, op => op.Name.Name == "General Surgery");
        Assert.DoesNotContain(result, op => op.Name.Name == "Anesthesia");
    }

    [Fact]
    public async Task TestFilterOperationTypesAsync_NoMatchingOperationTypes()
    {

        var operationTypesDatabase = new List<OperationType>
            {
                new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(30, 60, 15),
                    new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
                    })
            };

        var nameFilter = "NonExistent";
        string specializationFilter = null;
        bool? statusFilter = null;

        _mockOperationTypeRepository.Setup(repo => repo.FilterOperationTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
            .ReturnsAsync((string name, string specialization, bool? status) =>
                operationTypesDatabase
                    .Where(op => (string.IsNullOrEmpty(name) || op.Name.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                                 (string.IsNullOrEmpty(specialization) || op.RequiredStaff.Any(rs => rs.Specialization.Name.Name.Equals(specialization, StringComparison.OrdinalIgnoreCase))) &&
                                 (!status.HasValue || op.Active == status))
                    .ToList());


        var result = await _mockOperationTypeRepository.Object.FilterOperationTypesAsync(nameFilter, specializationFilter, statusFilter);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task TestFilterOperationTypesAsync_WithStatusFilter()
    {
        var operationTypesDatabase = new List<OperationType>
            {
            new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(30, 60, 15),
                new List<OperationTypeRequiredStaff>
                {
                    new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
                }),

            new OperationType(new OperationTypeName("Anesthesia"), new OperationTypeDuration(15, 30, 5),
                new List<OperationTypeRequiredStaff>
                {
                    new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Anesthesiologist"),"code","description"), 5)
                })
            };

        operationTypesDatabase[1].MarkAsInative();

        string nameFilter = null;
        string specializationFilter = null;
        bool? statusFilter = false;

        _mockOperationTypeRepository.Setup(repo => repo.FilterOperationTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
            .ReturnsAsync((string name, string specialization, bool? status) =>
                operationTypesDatabase
                    .Where(op => (string.IsNullOrEmpty(name) || op.Name.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                                 (string.IsNullOrEmpty(specialization) || op.RequiredStaff.Any(rs => rs.Specialization.Name.Name.Equals(specialization, StringComparison.OrdinalIgnoreCase))) &&
                                 (!status.HasValue || op.Active == status))
                    .ToList());

        var result = await _mockOperationTypeRepository.Object.FilterOperationTypesAsync(nameFilter, specializationFilter, statusFilter);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Anesthesia", result[0].Name.Name);
    }

    [Fact]
    public async Task TestFilterOperationTypesAsync_ReturnsOperationTypesBySpecialization()
    {
        var operationTypesDatabase = new List<OperationType>
            {
                new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(30, 60, 15),
                    new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 2)
                    }),
                new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(15, 30, 5),
                    new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Anesthesiologist"),"code","description"), 5)
                    }),
                new OperationType(new OperationTypeName("Surgery"), new OperationTypeDuration(40, 80, 20),
                    new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 1),
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Nurse"),"code","description"), 1)
                    })
            };

        string nameFilter = null;
        var specializationFilter = "Surgeon";
        bool? statusFilter = null;

        _mockOperationTypeRepository.Setup(repo => repo.FilterOperationTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
            .ReturnsAsync((string name, string specialization, bool? status) =>
                operationTypesDatabase
                    .Where(op => (string.IsNullOrEmpty(name) || op.Name.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                                 (string.IsNullOrEmpty(specialization) || op.RequiredStaff.Any(rs => rs.Specialization.Name.Name.Equals(specialization, StringComparison.OrdinalIgnoreCase))) &&
                                 (!status.HasValue || op.Active == status))
                    .ToList());

        var result = await _mockOperationTypeRepository.Object.FilterOperationTypesAsync(nameFilter, specializationFilter, statusFilter);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, op => op.Name.Name == "Surgery");
    }
}

