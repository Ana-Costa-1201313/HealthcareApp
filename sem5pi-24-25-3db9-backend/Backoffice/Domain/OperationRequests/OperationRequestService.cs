using Backoffice.Domain.Shared;
using Backoffice.Domain.Patients;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Logs;
using Backoffice.Domain.OperationRequests.ValueObjects;
using Microsoft.IdentityModel.Tokens;

namespace Backoffice.Domain.OperationRequests
{
    public class OperationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationRequestRepository _repo;
        private readonly IOperationTypeRepository _optyperepo;
        private readonly IPatientRepository _patientrepo;
        private readonly IStaffRepository _doctorrepo;
        private readonly ILogRepository _logrepo;
        private readonly OperationTypeMapper _opTypeMapper;

        public OperationRequestService(IUnitOfWork unitOfWork, IOperationRequestRepository repo,
                                        IOperationTypeRepository optyperepo, IPatientRepository patientrepo,
                                        IStaffRepository doctorrepo, ILogRepository logrepo, OperationTypeMapper operationTypeMapper)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _optyperepo = optyperepo;
            _patientrepo = patientrepo;
            _doctorrepo = doctorrepo;
            _logrepo = logrepo;
            _opTypeMapper = operationTypeMapper;
        }

        public async Task<List<OperationRequestDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();

            List<OperationRequestDto> listDto = new List<OperationRequestDto>();

            foreach (var item in list)
            {
                OperationType operationTypeName = await _optyperepo.GetByIdWithDetailsAsync(item.OpTypeId);
                string patientName = _patientrepo.GetByIdAsync(item.PatientId).Result.FullName.ToString();
                string doctorName = _doctorrepo.GetByIdAsync(item.DoctorId).Result.Name.ToString();

                listDto.Add(OperationRequestMapper.ToDto(item, _opTypeMapper.ToDto(operationTypeName), patientName, doctorName));
            }

            return listDto;
            //return null;
        }

        public async Task<List<OperationRequestDto>> GetAllByDoctorEmailAsync(string doctorEmail)
        {
            Staff doctor = await this._doctorrepo.GetStaffByEmailAsync(new Email(doctorEmail));

            var list = await this._repo.GetOpRequestsByDoctorIdAsync(doctor.Id);

            List<OperationRequestDto> listDto = new List<OperationRequestDto>();

            foreach (var item in list)
            {
                OperationType operationTypeName = await _optyperepo.GetByIdWithDetailsAsync(item.OpTypeId);
                string patientName = _patientrepo.GetByIdAsync(item.PatientId).Result.FullName.ToString();

                listDto.Add(OperationRequestMapper.ToDto(item, _opTypeMapper.ToDto(operationTypeName), patientName, doctor.Name));
            }

            if (listDto.IsNullOrEmpty())
                throw new BusinessRuleValidationException("Error: No Operation Requests found!");

            return listDto;
            //return null;
        }

        public async Task<OperationRequestDto> GetByIdAsync(Guid id)
        {
            var opReq = await this._repo.GetByIdAsync(new OperationRequestId(id));

            if (opReq == null)
                throw new BusinessRuleValidationException("Error: Operation Request not found!");

            OperationType operationTypeName = await _optyperepo.GetByIdWithDetailsAsync(opReq.OpTypeId);
            string patientName = _patientrepo.GetByIdAsync(opReq.PatientId).Result.FullName.ToString();
            string doctorName = _doctorrepo.GetByIdAsync(opReq.DoctorId).Result.Name.ToString();

            return OperationRequestMapper.ToDto(opReq, _opTypeMapper.ToDto(operationTypeName), patientName, doctorName);
            //return null;
        }

        public async Task<OperationRequestDto> PickByIdAsync(Guid id, List<string> selectedStaff)
        {
            var opReq = await this._repo.GetByIdAsync(new OperationRequestId(id));

            if (opReq == null)
                throw new BusinessRuleValidationException("Error: Operation Request not found!");

            OperationType operationType = await _optyperepo.GetByIdWithDetailsAsync(opReq.OpTypeId);
            string patientName = _patientrepo.GetByIdAsync(opReq.PatientId).Result.FullName.ToString();
            string doctorName = _doctorrepo.GetByIdAsync(opReq.DoctorId).Result.Name.ToString();

            opReq.ChangeStatus(selectedStaff);

            await _unitOfWork.CommitAsync();

            return OperationRequestMapper.ToDto(opReq, _opTypeMapper.ToDto(operationType), patientName, doctorName);
            //return null;
        }

        public async Task<OperationRequestDto> AddAsync(CreateOperationRequestDto dto)
        {
            var opType = await this._optyperepo.GetByOperationTypeName(dto.OpTypeName);

            List<Patient> patients = await this._patientrepo.SearchPatientsAsync(null, dto.PatientEmail, null, null);
            var patient = patients.FirstOrDefault();
            var doctor = await this._doctorrepo.GetStaffByEmailAsync(new Email(dto.DoctorEmail));

            var opRequest = OperationRequestMapper.ToDomain(dto, opType, patient, doctor);

            await this._repo.AddAsync(opRequest);

            OperationType operationTypeName = opType;
            string patientName = patient.FullName.ToString();
            string doctorName = doctor.Name.ToString();

            await this._logrepo.AddAsync(new Log(opRequest.ToJSON(operationTypeName.Name.Name, patientName, doctorName), LogType.Update, LogEntity.OperationRequest, opRequest.Id));
            await this._unitOfWork.CommitAsync();

            return OperationRequestMapper.ToDto(opRequest, _opTypeMapper.ToDto(operationTypeName), patientName, doctorName);
            //return null;
        }

        public async Task<OperationRequestDto> AddPickedAsync(CreatePickedOperationRequestDto dto)
        {
            var opType = await this._optyperepo.GetByOperationTypeName(dto.OpTypeName);
            List<Patient> patients = await this._patientrepo.SearchPatientsAsync(null, dto.PatientEmail, null, null);
            var patient = patients.FirstOrDefault();
            var doctor = await this._doctorrepo.GetStaffByEmailAsync(new Email(dto.DoctorEmail));

            List<Staff> selectedStaff = new List<Staff>();
            List<string> selectedStaffEmails = new List<string>();

            foreach (string staffEmail in dto.StaffList)
            {
                Staff staff = await this._doctorrepo.GetStaffByEmailAsync(new Email(staffEmail));
                selectedStaff.Add(staff);
            }

            foreach (Staff staff in selectedStaff)
            {
                selectedStaffEmails.Add(staff.Email.ToString());
            }

            var opRequest = OperationRequestMapper.ToDomainPicked(dto, opType, patient, doctor, new List<string>(selectedStaffEmails));

            opRequest.ChangeStatus(selectedStaffEmails);

            await this._repo.AddAsync(opRequest);

            OperationType operationTypeName = opType;
            string patientName = patient.FullName.ToString();
            string doctorName = doctor.Name.ToString();

            await this._logrepo.AddAsync(new Log(opRequest.ToJSON(operationTypeName.Name.Name, patientName, doctorName), LogType.Update, LogEntity.OperationRequest, opRequest.Id));
            await this._unitOfWork.CommitAsync();

            return OperationRequestMapper.ToDto(opRequest, _opTypeMapper.ToDto(operationTypeName), patientName, doctorName);
            //return null;
        }

        public async Task<List<OperationRequestDto>> FilterOperationRequestsAsync(Guid doctorId, Guid patientId, string operationTypeName, string priority, string status)
        {
            Priority prio = Priority.Null;
            Status stat = Status.Null;

            if (priority != null)
            {
                try
                {
                    prio = (Priority)Enum.Parse(typeof(Priority), priority, true);
                }
                catch (ArgumentException)
                {
                    throw new BusinessRuleValidationException("Error: Invalid Priority value provided!");
                }
            }

            if (status != null)
            {
                try
                {
                    stat = (Status)Enum.Parse(typeof(Status), status, true);
                }
                catch (ArgumentException)
                {
                    throw new BusinessRuleValidationException("Error: Invalid Status value provided!");
                }
            }

            PatientId patientIdObj = null;

            if (patientId != Guid.Empty)
                patientIdObj = new PatientId(patientId);

            StaffId doctorIdObj = null;

            if (doctorId != Guid.Empty)
                doctorIdObj = new StaffId(doctorId);

            var list = await this._repo.FilterOpRequestsAsync(doctorIdObj, patientIdObj, operationTypeName, prio, stat);

            List<OperationRequestDto> listDto = new List<OperationRequestDto>();

            foreach (var item in list)
            {
                OperationType operationTypeNameObj = await _optyperepo.GetByIdWithDetailsAsync(item.OpTypeId);
                string patientName = _patientrepo.GetByIdAsync(item.PatientId).Result.FullName.ToString();
                string doctorName = _doctorrepo.GetByIdAsync(item.DoctorId).Result.Name.ToString();

                listDto.Add(OperationRequestMapper.ToDto(item, _opTypeMapper.ToDto(operationTypeNameObj), patientName, doctorName));
            }

            if (listDto.IsNullOrEmpty())
                throw new BusinessRuleValidationException("No Operation Requests mathcing the criteria found!");

            return listDto;
            //return null;
        }

        public async Task<OperationRequestDto> PatchAsync(Guid id, EditOperationRequestDto dto)
        {
            var opReq = await this._repo.GetByIdAsync(new OperationRequestId(id));

            Priority prio = Priority.Null;

            if (opReq == null)
                throw new BusinessRuleValidationException("Error: Operation Request not found!");

            // Only update DeadlineDate if it's provided
            if (string.IsNullOrEmpty(dto.DeadlineDate))
            {
                dto.DeadlineDate = opReq.DeadlineDate.ToString();
            }

            // Only update Priority if it's provided (default enum value is not null)
            if (dto.Priority == null)
            {
                prio = opReq.Priority;
            }
            else
            {
                try
                {
                    prio = (Priority)Enum.Parse(typeof(Priority), dto.Priority, true);
                }
                catch (ArgumentException)
                {
                    throw new BusinessRuleValidationException("Error: Invalid Priority value provided!");
                }
            }

            // Only update Description if it's provided
            if (string.IsNullOrEmpty(dto.Description))
            {
                dto.Description = opReq.Description;
            }

            var deadlineDate = DateTime.Parse(dto.DeadlineDate);

            OperationType operationTypeName = await _optyperepo.GetByIdWithDetailsAsync(opReq.OpTypeId);
            string patientName = _patientrepo.GetByIdAsync(opReq.PatientId).Result.FullName.ToString();
            string doctorName = _doctorrepo.GetByIdAsync(opReq.DoctorId).Result.Name.ToString();

            await this._logrepo.AddAsync(new Log(opReq.ToJSON(operationTypeName.Name.Name, patientName, doctorName), LogType.Update, LogEntity.OperationRequest, opReq.Id));

            opReq.UpdateOperationRequest(deadlineDate, prio, dto.Description, null);

            await this._unitOfWork.CommitAsync();

            return OperationRequestMapper.ToDto(opReq, _opTypeMapper.ToDto(operationTypeName), patientName, doctorName);
            //return null;
        }

        public async Task<OperationRequestDto> DeleteAsync(Guid id)
        {
            var opReq = await this._repo.GetByIdAsync(new OperationRequestId(id));

            if (opReq == null)
                throw new BusinessRuleValidationException("Error: Operation Request not found!");

            OperationType operationTypeName = await _optyperepo.GetByIdWithDetailsAsync(opReq.OpTypeId);
            string patientName = _patientrepo.GetByIdAsync(opReq.PatientId).Result.FullName.ToString();
            string doctorName = _doctorrepo.GetByIdAsync(opReq.DoctorId).Result.Name.ToString();

            await this._logrepo.AddAsync(new Log(opReq.ToJSON(operationTypeName.Name.Name, patientName, doctorName), LogType.Delete, LogEntity.OperationRequest, opReq.Id));

            await this._repo.DeleteOpRequestAsync(opReq.Id);

            await this._unitOfWork.CommitAsync();

            return OperationRequestMapper.ToDto(opReq, _opTypeMapper.ToDto(operationTypeName), patientName, doctorName);
            //return null;
        }
    }
}