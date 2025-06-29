
using Backoffice.Domain.Shared;
using Backoffice.Domain.Specializations;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Backoffice.Domain.Logs;


namespace Backoffice.Domain.Staffs
{
    public class StaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffRepository _repo;
        private readonly StaffMapper _staffMapper;
        private readonly ISpecializationRepository _specRepo;
        private readonly ILogRepository _logRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;


        public StaffService(IUnitOfWork unitOfWork, IStaffRepository repo, StaffMapper staffMapper, ISpecializationRepository specRepo, ILogRepository logRepo, IEmailService emailService, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _staffMapper = staffMapper;
            _specRepo = specRepo;
            _logRepo = logRepo;
            _emailService = emailService;
            _config = config;
        }


        public async Task<List<StaffDto>> GetAllAsync(int pageNum, int pageSize)
        {
            List<Staff> list = await this._repo.GetAllWithDetailsAsync(pageNum, pageSize);

            List<StaffDto> listDto = new List<StaffDto>();

            foreach (Staff staff in list)
            {
                listDto.Add(_staffMapper.ToStaffDto(staff));
            }

            return listDto;
        }

        public async Task<StaffDto> GetByIdAsync(Guid id)
        {
            Staff staff = await this._repo.GetByIdWithDetailsAsync(new StaffId(id));

            if (staff == null)
                return null;

            return _staffMapper.ToStaffDto(staff);
        }

        public async Task<StaffDto> AddAsync(CreateStaffDto dto)
        {
            int mechanographicNumSeq = await this._repo.GetLastMechanographicNumAsync() + 1;

            if (mechanographicNumSeq == 0)
            {
                throw new BusinessRuleValidationException("Error: Couldn't get the number of staff members!");
            }

            Specialization specialization = null;

            if (dto.Specialization != null)
            {
                specialization = await _specRepo.GetBySpecializationName(dto.Specialization);

                if (specialization == null)
                {
                    throw new BusinessRuleValidationException("Error: There is no specialization with the name " + dto.Specialization + ".");
                }
            }

            Staff staff = _staffMapper.ToStaff(dto, specialization, mechanographicNumSeq, ReadDNS());

            try
            {
                await this._repo.AddAsync(staff);

                await this._unitOfWork.CommitAsync();

            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    var message = e.InnerException.Message;

                    if (message.Contains("UNIQUE constraint failed: Staff.Phone") || message.Contains("IX_Staff_Phone"))
                    {
                        throw new BusinessRuleValidationException("Error: This phone number is already in use!");
                    }
                    else if (message.Contains("UNIQUE constraint failed: Staff.Email") || message.Contains("IX_Staff_Email"))
                    {
                        throw new BusinessRuleValidationException("Error: This email is already in use!");
                    }
                    else if (message.Contains("UNIQUE constraint failed: Staff.LicenseNumber") || message.Contains("IX_Staff_LicenseNumber"))
                    {
                        throw new BusinessRuleValidationException("Error: This License number is already in use!");
                    }
                    else
                    {
                        throw new BusinessRuleValidationException("Error: Can't save this data!");
                    }
                }
                else
                {
                    throw new BusinessRuleValidationException("Error: Can't save this data!");
                }
            }

            return _staffMapper.ToStaffDto(staff);
        }

        public async Task<StaffDto> Deactivate(Guid id)
        {
            Staff staff = await this._repo.GetByIdWithDetailsAsync(new StaffId(id));

            if (staff == null)
            {
                return null;
            }

            staff.Deactivate();

            await this._logRepo.AddAsync(new Log("The staff with id " + staff.Id.AsGuid() + " was deactivated!", LogType.Deactivate, LogEntity.Staff, staff.Id));

            await this._unitOfWork.CommitAsync();

            return _staffMapper.ToStaffDto(staff);
        }

        public async Task<StaffDto> Delete(Guid id)
        {
            Staff staff = await this._repo.GetByIdWithDetailsAsync(new StaffId(id));

            if (staff == null)
            {
                return null;
            }

            this._repo.Remove(staff);

            await this._unitOfWork.CommitAsync();

            return _staffMapper.ToStaffDto(staff);
        }

        public async Task<StaffDto> UpdateAsync(EditStaffDto dto, bool partial)
        {
            Staff staff = await _repo.GetByIdWithDetailsAsync(new StaffId(dto.Id));

            if (staff == null)
            {
                return null;
            }

            Specialization specialization = null;

            if (dto.Specialization != null)
            {
                specialization = await _specRepo.GetBySpecializationName(dto.Specialization);

                if (specialization == null)
                {
                    throw new BusinessRuleValidationException("Error: There is no specialization with the name " + dto.Specialization + ".");
                }
            }

            if (!staff.Active)
            {
                throw new BusinessRuleValidationException("Error: Can't update an inactive staff!");
            }

            string oldPhoneNum = staff.Phone.PhoneNum;

            if (partial)
            {
                staff.PartialEdit(dto, specialization);
            }
            else
            {
                staff.Edit(dto, specialization);
            }

            await this._logRepo.AddAsync(new Log("The staff with id " + staff.Id.AsGuid() + " was edited!", LogType.Update, LogEntity.Staff, staff.Id));

            try
            {
                await this._unitOfWork.CommitAsync();

                if (dto.Phone != oldPhoneNum)
                {
                    string message = "Your phone number has been updated from " + oldPhoneNum + " to " + dto.Phone + ".";
                    string subject = "Phone number updated";

                    string defaultEmail = _config["EmailSmtp:DefaultStaffConfirmationEmail"];

                    if (!string.IsNullOrEmpty(defaultEmail))
                    {
                        await _emailService.SendEmail(defaultEmail, message, subject);
                    }
                    else
                    {
                        await _emailService.SendEmail(staff.Email._Email, message, subject);
                    }
                }
            }
            catch (DbUpdateException)
            {
                throw new BusinessRuleValidationException("Error: This phone number is already in use!");
            }

            return _staffMapper.ToStaffDto(staff);
        }

        public async Task<List<StaffDto>> FilterStaffAsync(string name, string email, string specialization, int pageNum, int pageSize)
        {
            List<Staff> list = await this._repo.FilterStaffAsync(name, email, specialization, pageNum, pageSize);

            List<StaffDto> listDto = new List<StaffDto>();

            foreach (Staff staff in list)
            {
                listDto.Add(_staffMapper.ToStaffDto(staff));
            }

            return listDto;
        }

        public async Task<int> GetTotalRecords()
        {
            int totalRecords = await this._repo.CountAsync();

            return totalRecords;
        }

        public virtual string ReadDNS()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DNS_URL"] ?? throw new ConfigurationErrorsException("Error: The DNS is not configured!");
        }
    }
}
