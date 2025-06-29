using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Specializations;
using Xunit;
using Backoffice.Domain.Logs;

namespace Backoffice.Tests
{
    public class OperationTypeServiceTests
    {
        private readonly OperationTypeMapper _mapper = new OperationTypeMapper();

        private OperationTypeService Setup(List<OperationType> operationTypesDatabase, List<Specialization> specializationsDatabase)
        {
            var operationTypeRepository = new Mock<IOperationTypeRepository>();
            var specializationRepository = new Mock<ISpecializationRepository>();
            var logRepository = new Mock<ILogRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            // Mock GetAllWithDetailsAsync
            operationTypeRepository.Setup(repo => repo.GetAllWithDetailsAsync()).ReturnsAsync(operationTypesDatabase);

            // Mock FilterOperationTypesAsync
            operationTypeRepository.Setup(repo => repo.FilterOperationTypesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ReturnsAsync((string name, string specialization, bool? status) =>
                    operationTypesDatabase
                        .Where(op =>
                            (string.IsNullOrEmpty(name) || op.Name.Name.Contains(name)) &&
                            (string.IsNullOrEmpty(specialization) ||
                            op.RequiredStaff.Any(rs => rs.Specialization.Name.Name.Contains(specialization))) &&
                            (!status.HasValue || op.Active == status))
                        .ToList());

            // Mock GetByIdWithDetailsAsync
            operationTypeRepository.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync((OperationTypeId id) => operationTypesDatabase.SingleOrDefault(op => op.Id.Equals(id)));

            operationTypeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync((OperationTypeId id) => operationTypesDatabase.SingleOrDefault(op => op.Id.Equals(id)));

            // Mock AddAsync
            operationTypeRepository.Setup(repo => repo.AddAsync(It.IsAny<OperationType>()))
                .Callback<OperationType>(op => operationTypesDatabase.Add(op));

            // Mock OperationTypeNameExists
            operationTypeRepository.Setup(repo => repo.OperationTypeNameExists(It.IsAny<string>()))
                .ReturnsAsync((string name) => operationTypesDatabase.Any(op => op.Name.Name == name));


            // Mock SpecializationNameExists
            specializationRepository.Setup(repo => repo.SpecializationNameExists(It.IsAny<string>()))
                .ReturnsAsync((string name) => specializationsDatabase.Any(spec => spec.Name.Name == name));

            // Mock GetBySpecializationName
            specializationRepository.Setup(repo => repo.GetBySpecializationName(It.IsAny<string>()))
                .ReturnsAsync((string name) => specializationsDatabase.SingleOrDefault(spec => spec.Name.Name == name));

            unitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(0);

            return new OperationTypeService(unitOfWork.Object, operationTypeRepository.Object, specializationRepository.Object, logRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ExistingOperationTypes()
        {
            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>();
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            var requiredStaff2 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 2),
                    ("Cardio", 3)
                };
            var operationType2 = _mapper.ToDomainForTests("Embolectomy", 30, 60, 15, requiredStaff2);


            operationTypesDatabase.Add(operationType1);
            operationTypesDatabase.Add(operationType2);

            var result = await service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("Surgery", result[0].Name);
            Assert.Equal(30, result[0].AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, result[0].SurgeryInMinutes);
            Assert.Equal(15, result[0].CleaningInMinutes);
            Assert.Equal("Surgeon", result[0].RequiredStaff[0].Specialization);
            Assert.Equal(5, result[0].RequiredStaff[0].Total);


            Assert.Equal("Embolectomy", result[1].Name);
            Assert.Equal(30, result[1].AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, result[1].SurgeryInMinutes);
            Assert.Equal(15, result[1].CleaningInMinutes);
            Assert.Equal("Surgeon", result[1].RequiredStaff[0].Specialization);
            Assert.Equal(2, result[1].RequiredStaff[0].Total);
            Assert.Equal("Cardio", result[1].RequiredStaff[1].Specialization);
            Assert.Equal(3, result[1].RequiredStaff[1].Total);
        }

        [Fact]
        public async Task GetByIdAsync_OperationTypeExists()
        {
            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>();
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            var requiredStaff2 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 2),
                    ("Cardio", 3)
                };
            var operationType2 = _mapper.ToDomainForTests("Embolectomy", 30, 60, 15, requiredStaff2);

            operationTypesDatabase.Add(operationType1);
            operationTypesDatabase.Add(operationType2);

            var result = await service.GetByIdAsync(operationType2.Id.AsGuid());

            Assert.NotNull(result);
            Assert.Equal("Embolectomy", result.Name);
            Assert.Equal(30, result.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, result.SurgeryInMinutes);
            Assert.Equal(15, result.CleaningInMinutes);
            Assert.Equal("Surgeon", result.RequiredStaff[0].Specialization);
            Assert.Equal(2, result.RequiredStaff[0].Total);
            Assert.Equal("Cardio", result.RequiredStaff[1].Specialization);
            Assert.Equal(3, result.RequiredStaff[1].Total);
        }

        [Fact]
        public async Task GetByIdAsync_OperationTypeDoesntExists()
        {
            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>();
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 5)
                };
            var operationType1 = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff1);

            var requiredStaff2 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 2),
                    ("Cardio", 3)
                };
            var operationType2 = _mapper.ToDomainForTests("Embolectomy", 30, 60, 15, requiredStaff2);

            operationTypesDatabase.Add(operationType1);

            var result = await service.GetByIdAsync(operationType2.Id.AsGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_WithValidData()
        {

            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description")
            };
            var service = Setup(operationTypesDatabase, specializationsDatabase);


            var requiredStaff = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 2)
                };
            var dto = _mapper.ToCreateDtoForTests("Surgery", 30, 60, 15, requiredStaff);


            var result = await service.AddAsync(dto);

            Assert.NotNull(result);
            Assert.Single(operationTypesDatabase);
            Assert.Equal("Surgery", result.Name);
            Assert.Equal(30, result.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, result.SurgeryInMinutes);
            Assert.Equal(15, result.CleaningInMinutes);
            Assert.Equal("Surgeon", result.RequiredStaff[0].Specialization);
            Assert.Equal(2, result.RequiredStaff[0].Total);
        }

        [Fact]
        public async Task AddAsync_WithInvalidSpecializationName()
        {

            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description")
            };
            var service = Setup(operationTypesDatabase, specializationsDatabase);


            var requiredStaff = new List<(string SpecializationName, int Total)>
                {
                    ("Cardio", 2)
                };
            var dto = _mapper.ToCreateDtoForTests("Surgery", 30, 60, 15, requiredStaff);


            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.AddAsync(dto));

            Assert.Equal("Error: There is no specialization with the name Cardio.", exception.Message);
            Assert.Empty(operationTypesDatabase);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateSpecialization()
        {

            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Cardio"),"code","description")
            };
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var requiredStaff = new List<(string SpecializationName, int Total)>
                {
                    ("Cardio", 2),
                    ("Cardio", 2)
                };
            var dto = _mapper.ToCreateDtoForTests("Surgery", 30, 60, 15, requiredStaff);

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.AddAsync(dto));

            Assert.Equal("Error: Can't have duplicate specializations -> Cardio.", exception.Message);
            Assert.Empty(operationTypesDatabase);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateOperationName()
        {

            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description")
            };
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var requiredStaff1 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 2)
                };
            var dto1 = _mapper.ToCreateDtoForTests("Surgery", 30, 60, 15, requiredStaff1);


            var requiredStaff2 = new List<(string SpecializationName, int Total)>
                {
                    ("Surgeon", 2)
                };
            var dto2 = _mapper.ToCreateDtoForTests("Surgery", 30, 60, 15, requiredStaff2);


            await service.AddAsync(dto1);
            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.AddAsync(dto2));

            Assert.Equal("Error: This operation type name is already being used.", exception.Message);
            Assert.Single(operationTypesDatabase);
        }

        [Fact]
        public async Task InactivateAsync_OperationTypeExistsAndIsActive_ShouldInactivate()
        {
            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>();
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var requiredStaff = new List<(string SpecializationName, int Total)>
            {
                ("Surgeon", 2)
            };

            var operationType = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff);
            operationTypesDatabase.Add(operationType);

            Assert.True(operationType.Active);

            var result = await service.InactivateAsync(operationType.Id.AsGuid());

            Assert.NotNull(result);
            Assert.False(operationType.Active);
            Assert.Equal("Surgery", result.Name);
        }

        [Fact]
        public async Task InactivateAsync_OperationTypeDoesNotExist_ShouldReturnNull()
        {
            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>();
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var nonExistingId = Guid.NewGuid();


            var result = await service.InactivateAsync(nonExistingId);

            Assert.Null(result);
        }

        [Fact]
        public async Task InactivateAsync_OperationTypeExistsAndIsInactive_ShouldThrow()
        {
            var operationTypesDatabase = new List<OperationType>();
            var specializationsDatabase = new List<Specialization>();
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var requiredStaff = new List<(string SpecializationName, int Total)>
            {
                ("Surgeon", 2)
            };

            var operationType = _mapper.ToDomainForTests("Surgery", 30, 60, 15, requiredStaff);
            operationTypesDatabase.Add(operationType);

            Assert.True(operationType.Active);
            operationType.MarkAsInative();

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => service.InactivateAsync(operationType.Id.AsGuid()));

            Assert.Equal("Error: The operation type is already inactive.", exception.Message);
        }

        [Fact]
        public async Task Patch_WithValidData()
        {
            var operationTypesDatabase = new List<OperationType>();

            var opType = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>{
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"),5)
                }
            );
            operationTypesDatabase.Add(opType);

            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description"),
                new Specialization(new SpecializationName("Anesthesiologist"),"code","description"),
                new Specialization(new SpecializationName("Nurse"),"code","description")
            };

            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
                {
                    new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
                    new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 },
                    new RequiredStaffDto { Specialization = "Nurse", Total = 3 }
                }
            };

            var result = await service.Patch(opType.Id.AsGuid(), editOperationTypeDto);

            Assert.Single(operationTypesDatabase);
            Assert.Equal("Appendectomy", result.Name);
            Assert.Equal(30, result.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(90, result.SurgeryInMinutes);
            Assert.Equal(20, result.CleaningInMinutes);
            Assert.Equal("Surgeon", result.RequiredStaff[0].Specialization);
            Assert.Equal(2, result.RequiredStaff[0].Total);
            Assert.Equal("Anesthesiologist", result.RequiredStaff[1].Specialization);
            Assert.Equal(1, result.RequiredStaff[1].Total);
            Assert.Equal("Nurse", result.RequiredStaff[2].Specialization);
            Assert.Equal(3, result.RequiredStaff[2].Total);
        }

        [Fact]
        public async Task Patch_ThrowsException_WhenSpecializationDoesNotExist()
        {
            var operationTypesDatabase = new List<OperationType>();

            var opType = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                {
            new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                }
            );
            operationTypesDatabase.Add(opType);

            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description"),
                new Specialization(new SpecializationName("Nurse"),"code","description")
            };

            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
            {
                new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
                new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 },
                new RequiredStaffDto { Specialization = "Nurse", Total = 3 }
            }
            };

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () =>
                await service.Patch(opType.Id.AsGuid(), editOperationTypeDto));

            Assert.Equal("Error: There is no specialization with the name Anesthesiologist.", exception.Message);
        }

        [Fact]
        public async Task Patch_ThrowsException_WhenDuplicateSpecializationsExist()
        {
            var operationTypesDatabase = new List<OperationType>();

            var opType = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                {
            new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                }
            );
            operationTypesDatabase.Add(opType);

            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description"),
                new Specialization(new SpecializationName("Anesthesiologist"),"code","description"),
                new Specialization(new SpecializationName("Nurse"),"code","description")
            };

            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
        {
            new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
            new RequiredStaffDto { Specialization = "Surgeon", Total = 3 },
            new RequiredStaffDto { Specialization = "Nurse", Total = 3 }
        }
            };

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () =>
                await service.Patch(opType.Id.AsGuid(), editOperationTypeDto));

            Assert.Equal("Error: Can't have duplicate specializations -> Surgeon.", exception.Message);
        }

        [Fact]
        public async Task Put_WithValidData()
        {
            var operationTypesDatabase = new List<OperationType>();

            var opType = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>{
                new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"),5)
                }
            );
            operationTypesDatabase.Add(opType);

            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description"),
                new Specialization(new SpecializationName("Anesthesiologist"),"code","description"),
                new Specialization(new SpecializationName("Nurse"),"code","description")
            };

            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
        {
            new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
            new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 },
            new RequiredStaffDto { Specialization = "Nurse", Total = 3 }
        }
            };

            var result = await service.Put(opType.Id.AsGuid(), editOperationTypeDto);

            Assert.Single(operationTypesDatabase);
            Assert.Equal("Appendectomy", result.Name);
            Assert.Equal(30, result.AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(90, result.SurgeryInMinutes);
            Assert.Equal(20, result.CleaningInMinutes);
            Assert.Equal("Surgeon", result.RequiredStaff[0].Specialization);
            Assert.Equal(2, result.RequiredStaff[0].Total);
            Assert.Equal("Anesthesiologist", result.RequiredStaff[1].Specialization);
            Assert.Equal(1, result.RequiredStaff[1].Total);
            Assert.Equal("Nurse", result.RequiredStaff[2].Specialization);
            Assert.Equal(3, result.RequiredStaff[2].Total);
        }

        [Fact]
        public async Task Put_ThrowsException_WhenSpecializationDoesNotExist()
        {
            var operationTypesDatabase = new List<OperationType>();

            var opType = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                {
            new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                }
            );
            operationTypesDatabase.Add(opType);

            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description"),
                new Specialization(new SpecializationName("Nurse"),"code","description")
            };

            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
            {
                new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
                new RequiredStaffDto { Specialization = "Anesthesiologist", Total = 1 },
                new RequiredStaffDto { Specialization = "Nurse", Total = 3 }
            }
            };

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () =>
                await service.Put(opType.Id.AsGuid(), editOperationTypeDto));

            Assert.Equal("Error: There is no specialization with the name Anesthesiologist.", exception.Message);
        }

        [Fact]
        public async Task Put_ThrowsException_WhenDuplicateSpecializationsExist()
        {
            var operationTypesDatabase = new List<OperationType>();

            var opType = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                {
            new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                }
            );
            operationTypesDatabase.Add(opType);

            var specializationsDatabase = new List<Specialization>
            {
                new Specialization(new SpecializationName("Surgeon"),"code","description"),
                new Specialization(new SpecializationName("Anesthesiologist"),"code","description"),
                new Specialization(new SpecializationName("Nurse"),"code","description")
            };

            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var editOperationTypeDto = new EditOperationTypeDto
            {
                Name = "Appendectomy",
                AnesthesiaPatientPreparationInMinutes = 30,
                SurgeryInMinutes = 90,
                CleaningInMinutes = 20,
                RequiredStaff = new List<RequiredStaffDto>
        {
            new RequiredStaffDto { Specialization = "Surgeon", Total = 2 },
            new RequiredStaffDto { Specialization = "Surgeon", Total = 3 },
            new RequiredStaffDto { Specialization = "Nurse", Total = 3 }
        }
            };

            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () =>
                await service.Put(opType.Id.AsGuid(), editOperationTypeDto));

            Assert.Equal("Error: Can't have duplicate specializations -> Surgeon.", exception.Message);
        }

        [Fact]
        public async Task FilterOperationTypesAsync_ReturnsOperationType_WhenMatchingRecordsExist()
        {
            var operationTypesDatabase = new List<OperationType>();
            var opType1 = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                    }
                );

            var opType2 = new OperationType(
                new OperationTypeName("SUG"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                    }
                );
            operationTypesDatabase.Add(opType1);
            operationTypesDatabase.Add(opType2);

            var specializationsDatabase = new List<Specialization>();
            var expectedName = "Surgery";
            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var result = await service.FilterOperationTypesAsync(expectedName, null, null);

            Assert.Equal(opType1.Name.Name, result[0].Name);

            Assert.Equal(20, result[0].AnesthesiaPatientPreparationInMinutes);
            Assert.Equal(60, result[0].SurgeryInMinutes);
            Assert.Equal(30, result[0].CleaningInMinutes);
            Assert.Single(result[0].RequiredStaff);

            var returnedStaff = result[0].RequiredStaff.First();
            Assert.Equal("Surgeon", returnedStaff.Specialization);
            Assert.Equal(5, returnedStaff.Total);
        }

        [Fact]
        public async Task FilterOperationTypesAsync_ReturnsEmptyList_WhenNoMatchingRecordsExist()
        {
            var operationTypesDatabase = new List<OperationType>();
            var opType1 = new OperationType(
                new OperationTypeName("Surgery"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                    }
                );

            var opType2 = new OperationType(
                new OperationTypeName("SUG"),
                new OperationTypeDuration(20, 60, 30),
                new List<OperationTypeRequiredStaff>
                    {
                        new OperationTypeRequiredStaff(new Specialization(new SpecializationName("Surgeon"),"code","description"), 5)
                    }
                );
            operationTypesDatabase.Add(opType1);
            operationTypesDatabase.Add(opType2);

            var specializationsDatabase = new List<Specialization>();

            var expectedName = "NOTHING";

            var service = Setup(operationTypesDatabase, specializationsDatabase);

            var result = await service.FilterOperationTypesAsync(expectedName, null, null);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
