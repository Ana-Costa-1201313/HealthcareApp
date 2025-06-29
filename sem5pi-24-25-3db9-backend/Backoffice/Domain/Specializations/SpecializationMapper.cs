using System;
using Backoffice.Domain.Specializations;

namespace Backoffice.Domain.Specializations;

public class SpecializationMapper
{
    public SpecializationDto ToDto(Specialization specialization)
    {
        return new SpecializationDto
        {
            Id = specialization.Id.AsGuid(),
            Name = specialization.Name.Name,
            Code = specialization.Code,
            Description = specialization.Description
        };
    }

    public Specialization ToDomain(SpecializationDto dto)
    {
        return new Specialization(
            new SpecializationName(dto.Name),
            dto.Code,
            dto.Description
        );
    }

    public Specialization ToDomain(CreatingSpecializationDto dto, string code)
    {
        return new Specialization(
            new SpecializationName(dto.Name),
            code,
            dto.Description
        );
    }

    //Only for tests
    public Specialization ToDomainForTests(string specName)
    {
        return new Specialization(
                new SpecializationName(specName),
                "code",
                "description");
    }

    //Only for tests
    public CreatingSpecializationDto ToCreateDtoForTests(string specName)
    {
        return new CreatingSpecializationDto(specName, "description");
    }

}
