using System.Threading.Tasks;
using System.Collections.Generic;
using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Specializations
{
    public class SpecializationService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpecializationRepository _repo;
        private readonly SpecializationMapper _mapper;
        private readonly ICD11Service _icd11Service;

        public SpecializationService(IUnitOfWork unitOfWork, ISpecializationRepository repo, SpecializationMapper mapper, ICD11Service iCD11Service)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._mapper = mapper;
            this._icd11Service = iCD11Service;
        }

        public async Task<List<SpecializationDto>> GetAllAsync()
        {
            List<Specialization> specList = await this._repo.GetAllAsync();

            List<SpecializationDto> listDto = new List<SpecializationDto>();

            foreach (Specialization spec in specList)
            {
                listDto.Add(_mapper.ToDto(spec));
            }

            return listDto;
        }

        public async Task<SpecializationDto> GetByIdAsync(Guid id)
        {
            var spec = await this._repo.GetByIdAsync(new SpecializationId(id));

            if (spec == null)
                return null;

            return _mapper.ToDto(spec);
        }

        public async Task<SpecializationDto> AddAsync(CreatingSpecializationDto dto)
        {

            if (await this._repo.SpecializationNameExists(dto.Name))
            {
                throw new BusinessRuleValidationException("Error: This specialization name is already being used.");
            }

            string code;
            do
            {
                code = _icd11Service.GenerateICD11Code();
            }
            while (await this._repo.SpecializationCodeExists(code));

            var spec = _mapper.ToDomain(dto, code);

            await this._repo.AddAsync(spec);

            await this._unitOfWork.CommitAsync();

            return _mapper.ToDto(spec);
        }

        public async Task<SpecializationDto> UpdateAsync(Guid id, EditSpecializationDto dto)
        {
            var specialization = await _repo.GetByIdAsync(new SpecializationId(id));

            if (specialization == null)
                return null;



            if (!string.IsNullOrEmpty(dto.Name))
            {
                SpecializationName sName = new SpecializationName(dto.Name);
                specialization.ChangeName(sName);
            }

            if (!string.IsNullOrEmpty(dto.Description))
                specialization.ChangeDescription(dto.Description);

            await _unitOfWork.CommitAsync();
            return _mapper.ToDto(specialization);
        }

        public async Task<SpecializationDto> InactivateAsync(Guid id)
        {
            var spec = await this._repo.GetByIdAsync(new SpecializationId(id));

            if (spec == null)
                return null;

            spec.MarkAsInative();

            await this._unitOfWork.CommitAsync();

            return _mapper.ToDto(spec);
        }
    }
}