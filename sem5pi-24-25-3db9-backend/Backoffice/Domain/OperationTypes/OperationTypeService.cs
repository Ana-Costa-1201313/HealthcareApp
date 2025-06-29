using System.Threading.Tasks;
using System.Collections.Generic;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Specializations;
using Backoffice.Domain.Logs;

namespace Backoffice.Domain.OperationTypes
{
    public class OperationTypeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationTypeRepository _repo;
        private readonly ISpecializationRepository _repoSpecialization;
        private readonly ILogRepository _repoLog;
        private readonly OperationTypeMapper _mapper;


        public OperationTypeService(IUnitOfWork unitOfWork, IOperationTypeRepository repo, ISpecializationRepository repoSpecification, ILogRepository repoLog, OperationTypeMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._repoSpecialization = repoSpecification;
            this._repoLog = repoLog;
            this._mapper = mapper;
        }

        public async Task<List<OperationTypeDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllWithDetailsAsync();

            List<OperationTypeDto> listDto = list.Select(opType => _mapper.ToDto(opType)).ToList();

            return listDto;
        }

        public async Task<OperationTypeDto> GetByIdAsync(Guid id)
        {
            var opType = await this._repo.GetByIdWithDetailsAsync(new OperationTypeId(id));

            if (opType == null)
                return null;

            return _mapper.ToDto(opType);
        }

        public async Task<OperationTypeDto> AddAsync(CreatingOperationTypeDto dto)
        {

            var requiredStaffList = new List<OperationTypeRequiredStaff>();
            var specializationNamesSet = new HashSet<string>();

            if (dto.RequiredStaff == null)
            {
                throw new BusinessRuleValidationException("Error: Required Staff can't be null.");
            }

            foreach (var staff in dto.RequiredStaff)
            {
                if (!await this._repoSpecialization.SpecializationNameExists(staff.Specialization))
                {
                    throw new BusinessRuleValidationException("Error: There is no specialization with the name " + staff.Specialization + ".");
                }

                // Check for duplicates in specialization names
                if (!specializationNamesSet.Add(staff.Specialization))
                {
                    throw new BusinessRuleValidationException("Error: Can't have duplicate specializations -> " + staff.Specialization + ".");
                }
                var specialization = await _repoSpecialization.GetBySpecializationName(staff.Specialization);
                var requiredStaff = new OperationTypeRequiredStaff(specialization, staff.Total);
                requiredStaffList.Add(requiredStaff);
            }

            var opType = _mapper.ToDomain(dto);

            opType.ChangeRequiredStaff(requiredStaffList);

            if (await this._repo.OperationTypeNameExists(opType.Name.Name))
            {
                throw new BusinessRuleValidationException("Error: This operation type name is already being used.");
            }

            await this._repo.AddAsync(opType);

            await this._repoLog.AddAsync(new Log(opType.ToJSON(), LogType.Create, LogEntity.OperationType, opType.Id));

            await this._unitOfWork.CommitAsync();

            return _mapper.ToDto(opType);
        }

        public async Task<OperationTypeDto> InactivateAsync(Guid id)
        {
            var opType = await this._repo.GetByIdWithDetailsAsync(new OperationTypeId(id));

            if (opType == null)
                return null;

            opType.MarkAsInative();

            await this._unitOfWork.CommitAsync();

            return _mapper.ToDto(opType);
        }


        public async Task<OperationTypeDto> Patch(Guid id, EditOperationTypeDto operationTypeDto)
        {
            var operationType = await _repo.GetByIdWithDetailsAsync(new OperationTypeId(id));

            if (operationType == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(operationTypeDto.Name))
            {
                operationType.ChangeName(new OperationTypeName(operationTypeDto.Name));
            }

            if (operationTypeDto.AnesthesiaPatientPreparationInMinutes != 0)
            {
                operationType.ChangeAnesthesiaPatientPreparationDuration(operationTypeDto.AnesthesiaPatientPreparationInMinutes);
            }

            if (operationTypeDto.SurgeryInMinutes != 0)
            {
                operationType.ChangeSurgeryDuration(operationTypeDto.SurgeryInMinutes);
            }

            if (operationTypeDto.CleaningInMinutes != 0)
            {
                operationType.ChangeCleaningDuration(operationTypeDto.CleaningInMinutes);
            }

            if (operationTypeDto.RequiredStaff != null && operationTypeDto.RequiredStaff.Any())
            {
                var specializationsNamesSet = new HashSet<String>();
                var updatedStaff = new List<OperationTypeRequiredStaff>();

                foreach (var rsDto in operationTypeDto.RequiredStaff)
                {
                    var specialization = await _repoSpecialization.GetBySpecializationName(rsDto.Specialization);
                    if (specialization == null)
                    {
                        throw new BusinessRuleValidationException("Error: There is no specialization with the name " + rsDto.Specialization + ".");
                    }

                    if (!specializationsNamesSet.Add(rsDto.Specialization))
                    {
                        throw new BusinessRuleValidationException("Error: Can't have duplicate specializations -> " + rsDto.Specialization + ".");
                    }

                    var requiredStaff = new OperationTypeRequiredStaff(specialization, rsDto.Total);
                    updatedStaff.Add(requiredStaff);
                }

                operationType.ChangeRequiredStaff(updatedStaff);
            }

            await _unitOfWork.CommitAsync();

            return _mapper.ToDto(operationType);
        }

        public async Task<OperationTypeDto> Put(Guid id, EditOperationTypeDto operationTypeDto)
        {
            var operationType = await _repo.GetByIdAsync(new OperationTypeId(id));

            if (operationType == null)
            {
                return null;
            }

            var specializationsNamesSet = new HashSet<String>();
            var updatedStaff = new List<OperationTypeRequiredStaff>();

            foreach (var rsDto in operationTypeDto.RequiredStaff)
            {
                var specialization = await _repoSpecialization.GetBySpecializationName(rsDto.Specialization);
                if (specialization == null)
                {
                    throw new BusinessRuleValidationException("Error: There is no specialization with the name " + rsDto.Specialization + ".");
                }

                if (!specializationsNamesSet.Add(rsDto.Specialization))
                {
                    throw new BusinessRuleValidationException("Error: Can't have duplicate specializations -> " + rsDto.Specialization + ".");
                }

                var requiredStaff = new OperationTypeRequiredStaff(specialization, rsDto.Total);
                updatedStaff.Add(requiredStaff);
            }

            operationType.ChangeAll(
                new OperationTypeName(operationTypeDto.Name),
                operationTypeDto.AnesthesiaPatientPreparationInMinutes,
                operationTypeDto.SurgeryInMinutes,
                operationTypeDto.CleaningInMinutes,
                updatedStaff);



            await _unitOfWork.CommitAsync();
            return _mapper.ToDto(operationType);
        }

        public async Task<List<OperationTypeDto>> FilterOperationTypesAsync(string name, string specialization, bool? status)
        {
            var opTypes = await _repo.FilterOperationTypesAsync(name, specialization, status);

            if (opTypes == null)
            {
                return null;
            }

            List<OperationTypeDto> listDto = opTypes.Select(opType => _mapper.ToDto(opType)).ToList();

            return listDto;
        }

        public async Task DeleteAsync(Guid id)
        {
            var opType = await this._repo.GetByIdAsync(new OperationTypeId(id));

            if (opType == null)
            {
                throw new BusinessRuleValidationException("Error: Operation type with that id doesn't exist");
            }

            this._repo.Remove(opType);

            await this._unitOfWork.CommitAsync();
        }


    }
}