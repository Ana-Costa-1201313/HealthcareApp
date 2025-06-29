using Backoffice.Domain.Logs;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Net.Mail;

namespace Backoffice.Domain.Users
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;
        private readonly IStaffRepository _staffRepo;


        private readonly IEmailService _emailService;
        private readonly ExternalApiServices _externalApiServices;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;

        private readonly PatientService _patientService;

        // passar isto para o configurations file
        private readonly string emailBody = $"https://localhost:5001/api/Users/";

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo, IEmailService emailService, ExternalApiServices externalApiServices, IStaffRepository staffRepo, PatientService patientService,TokenService tokenService, IConfiguration config)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._emailService = emailService;
            this._externalApiServices = externalApiServices;
            this._staffRepo = staffRepo;
            this._tokenService = tokenService;
            this._config = config;
            this._patientService = patientService;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();

            List<UserDto> listDto = list.ConvertAll<UserDto>(u => new UserDto
            {
                Id = u.Id.AsGuid(),
                Role = u.Role,
                Email = u.Email.ToString(),
                Password = u.Password
            });

            return listDto;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await this._repo.GetByIdAsync(new UserId(id));

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id.AsGuid(),
                Role = user.Role,
                Email = user.Email.ToString(),
                Password = user.Password
            };
        }

        public async Task<UserDto> AddAsync(CreateUserDto dto)
        {
            Staff staff = await _staffRepo.GetStaffByEmailAsync(new Email(dto.Email));

            if (dto.Role == Role.Patient || dto.Role == Role.Admin)

            {
                if (staff != null)
                {
                    throw new BusinessRuleValidationException("Error: That email is being used by a staff member!");
                }
            }
            else
            {
                if (staff == null || staff.Role != dto.Role)
                {
                    throw new BusinessRuleValidationException("Error: The user role doesn't match the email!");
                }
            }

            User user = new User(dto.Role, dto.Email);


            await this._repo.AddAsync(user);

            await this._unitOfWork.CommitAsync();

            string messageBodyParameters = $"{user.Id.AsGuid()}?password=changeMeToYourNewPassword";
            string messageSubject = "change your password";
            try
            {
                await _emailService.SendEmail(dto.Email, emailBody + messageBodyParameters, messageSubject);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new UserDto
            {
                Id = user.Id.AsGuid(),
                Role = user.Role,
                Email = user.Email.ToString(),
                Password = user.Password,
                Active = user.Active

            };
        }

        public async Task<UserDto> UpdatePassword(Guid id, string password)
        {

            //pasword should be hashed for security

            var user = await this._repo.GetByIdAsync(new UserId(id));

            if (user == null)
            {
                return null;
            }

            user.ActivateUser(password);

            await this._unitOfWork.CommitAsync();

            return new UserDto
            {
                Id = user.Id.AsGuid(),
                Role = user.Role,
                Email = user.Email.ToString(),
                Password = user.Password,
                Active = user.Active
            };
        }


        public async void sendConfirmationEmail(UserDto user)
        {
            if (user == null)
            {
                throw new NullReferenceException("No user to send confirmation eail to!");
            }
            try
            {
                await _emailService.SendEmail(user.Email, $"New account has been created with password: {user.Password.ToString} .", "Confirmation email");
                //await _emailService.SendEmail("1221695@isep.ipp.pt", $"New account has been created with password: { user.Password.Passwd} .","Confirmation email");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<UserDto> SendPasswordResetLink(string email)
        {

            var user = await this._repo.getUserByEmail(email);

            if (user == null)
            {
                throw new BusinessRuleValidationException("Error: Email doesn't exist.");
            }

            if (!user.Active)
            {
                throw new BusinessRuleValidationException("Error: User inactive.");
            }

            if (!(user.Role == Role.Admin || user.Role == Role.Doctor || user.Role == Role.Nurse || user.Role == Role.Technician))
            {
                throw new BusinessRuleValidationException("Error: User with this role can't ask for a password reset");
            }

            var token = _tokenService.GenerateJwtToken(email);
            var url = $"{_config["AppSettings:PasswordResetUrl"]}?token={token}";

            await _emailService.SendEmail(user.Email._Email, url, "Reset Password");

            return new UserDto
            {
                Id = user.Id.AsGuid(),
                Role = user.Role,
                Email = user.Email.ToString(),
                Active = user.Active
            };
        }

        public async Task<UserDto> NewPassword(string token, string password)
        {
            var email = this._tokenService.ValidateJwtToken(token);

            var user = await this._repo.getUserByEmail(email);

            user.ResetPassword(password);

            await _unitOfWork.CommitAsync();

            return new UserDto
            {
                Id = user.Id.AsGuid(),
                Role = user.Role,
                Email = user.Email.ToString(),
                Active = user.Active
            };
        }

        public async Task<UserDto> createPatient(CreatePatientRequestDto createPatientRequestDto)
        {
            if (createPatientRequestDto == null || createPatientRequestDto.UserDto == null || createPatientRequestDto.PatientDto == null)
            {
                throw new ArgumentNullException($"userdto or patientDto is null");
            }

            UserDto resultUserDto = null;
            try
            {
                resultUserDto = await AddAsync(createPatientRequestDto.UserDto);

                // log
            }
            catch (BusinessRuleValidationException e)
            {
                throw new BusinessRuleValidationException($"{e.Message}, create user fail");
            }

            try
            {
                var medicalRecordNumber = await _patientService.GenerateNextMedicalRecordNumber();

                var patient = await _patientService.AddAsync(createPatientRequestDto.PatientDto, medicalRecordNumber);

                // log
            }
            catch (BusinessRuleValidationException e)
            {
                throw new BusinessRuleValidationException($"{e.Message}, create patient fail");
            }

            return resultUserDto;
        }

        public async Task<Boolean> askConsentDeletePatientUserAsync(UserDto userDto)
        {
            var user = await _repo.getUserByEmail(userDto.Email);
            SearchPatientDto patientProfile = await _patientService.GetByEmailAsync(new Email(userDto.Email));

            await _emailService.SendEmail(userDto.Email, "If you want to delete account please click link:\n" +
                $"https://localhost:5001/api/Users/deletePatient?email={userDto.Email}", "Deletion of User account and Patient Profile");

            return true;
        }

        public async Task<Boolean> deletePatientUserAsync(String email)
        {
            var user = await _repo.getUserByEmail(email);
            SearchPatientDto patientProfile = await _patientService.GetByEmailAsync(new Email(email));

            if (user != null)
                _repo.Remove(user);
                //throw new BusinessRuleValidationException("Error: User/Patient doesn't exist !!!");

            try
            {
                if(patientProfile != null) _patientService.DeletePacientProfileAsync(email);
            }
            catch (Exception e) {
                //throw new BusinessRuleValidationException($"Error: Cant delete patient {e.Message} !!!");
                //ConsoleFactory($"Error: Cant delete patient {e.Message} !!!");
            }

            //await _unitOfWork.CommitAsync();

            return true;
        }
    }
}