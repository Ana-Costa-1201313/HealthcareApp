using System;
using Backoffice.Domain.Specializations;

namespace Backoffice.Domain.OperationTypes;

public class OperationTypeMapper
{
    public OperationTypeDto ToDto(OperationType operationType)
    {
        return new OperationTypeDto
        {
            Id = operationType.Id.AsGuid(),
            Name = operationType.Name.Name,
            AnesthesiaPatientPreparationInMinutes = (int)operationType.Duration.AnesthesiaPatientPreparationInMinutes.TotalMinutes,
            SurgeryInMinutes = (int)operationType.Duration.SurgeryInMinutes.TotalMinutes,
            CleaningInMinutes = (int)operationType.Duration.CleaningInMinutes.TotalMinutes,
            RequiredStaff = operationType.RequiredStaff.Select(staff => new RequiredStaffDto
            {
                Specialization = staff.Specialization.Name.Name,
                Total = staff.Total
            }).ToList(),
            Active = operationType.Active
        };
    }

    public OperationType ToDomain(OperationTypeDto dto)
    {
        return new OperationType(
            new OperationTypeName(dto.Name),
            new OperationTypeDuration(dto.AnesthesiaPatientPreparationInMinutes, dto.SurgeryInMinutes, dto.CleaningInMinutes),
            dto.RequiredStaff.Select(staffDto => new OperationTypeRequiredStaff(
                new Specialization(new SpecializationName(staffDto.Specialization),"code","description"), staffDto.Total)).ToList()
        );
    }

    public OperationType ToDomain(CreatingOperationTypeDto dto)
    {
        return new OperationType(
            new OperationTypeName(dto.Name),
            new OperationTypeDuration(dto.AnesthesiaPatientPreparationInMinutes, dto.SurgeryInMinutes, dto.CleaningInMinutes),
            dto.RequiredStaff.Select(staffDto => new OperationTypeRequiredStaff(
                new Specialization(new SpecializationName(staffDto.Specialization),"code","description"), staffDto.Total)).ToList()
        );
    }

    //Only for tests
    public OperationType ToDomainForTests(string OpTypeName,
                                            int OpTypeDurationPhase1,
                                            int OpTypeDurationPhase2,
                                            int OpTypeDurationPhase3,
                                            List<(string SpecializationName, int Total)> requiredStaff)
    {

        var staffList = requiredStaff
        .Select(staff => new OperationTypeRequiredStaff(
            new Specialization(new SpecializationName(staff.SpecializationName),"code","description"), staff.Total))
        .ToList();


        return new OperationType(
                new OperationTypeName(OpTypeName),
                new OperationTypeDuration(OpTypeDurationPhase1, OpTypeDurationPhase2, OpTypeDurationPhase3),
                staffList);
    }

    //Only for tests
    public CreatingOperationTypeDto ToCreateDtoForTests(string opTypeName,
                                                    int opTypeDurationPhase1,
                                                    int opTypeDurationPhase2,
                                                    int opTypeDurationPhase3,
                                                    List<(string SpecializationName, int Total)> requiredStaff)
    {
        var staffList = requiredStaff
            .Select(staff => new RequiredStaffDto
            {
                Specialization = staff.SpecializationName,
                Total = staff.Total
            })
            .ToList();

        return new CreatingOperationTypeDto(
            opTypeName,
            opTypeDurationPhase1,
            opTypeDurationPhase2,
            opTypeDurationPhase3,
            staffList);
    }
}
