

using Backoffice.Domain.Shared;
using Backoffice.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Backoffice.Domain.Logs;
using Backoffice.Domain.Users;
using System.Text.Json;
using System.Xml.Serialization;
using System.Text;
using System.Xml;


namespace Backoffice.Domain.Patients
{


    public class PatientService : IPatientService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _repo;
        private readonly PatientMapper _patientMapper;
        private readonly ILogRepository _repoLog;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PatientService(IUnitOfWork unitOfWork, IPatientRepository patientRepository, PatientMapper patientMapper, ILogRepository repoLog, IEmailService emailService, IConfiguration configuration)
        {
            this._unitOfWork = unitOfWork;
            this._repo = patientRepository;
            this._patientMapper = patientMapper;
            this._repoLog = repoLog;
            this._emailService = emailService;
            this._configuration = configuration;

        }
        //Obter todos os Patient profiles
        public async Task<List<PatientDto>> GetAllAsync()
        {
            var list = await _repo.GetAllWithDetailsAsync();
            List<PatientDto> listDto = new List<PatientDto>();

            foreach (var patient in list)
            {
                listDto.Add(_patientMapper.ToPatientDto(patient));
            }
            return listDto;
        }
        public async Task<SearchPatientDto> GetByEmailAsync(Email email)
        {
            var patient = await _repo.GetPatientByEmailAsync(email);

            if (patient == null)
            {
                return null;
            }

            return _patientMapper.ToSearchPatientDto(patient);
        }

        //Obter Patient profile por Id
        public async Task<PatientDto> GetByIdAsync(Guid id)
        {
            var patient = await _repo.GetWithDetailsAsync(new PatientId(id));

            if (patient == null)
            {
                return null;
            }

            return _patientMapper.ToPatientDto(patient);
        }
        // Adicionar um novo Patient profile
        public async Task<PatientDto> AddAsync(CreatePatientDto dto, string medicalRecordNumber)
        {
            var patient = _patientMapper.ToPatient(dto, medicalRecordNumber);

            try
            {
                await _repo.AddAsync(patient);
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("UNIQUE constraint failed: Patient.Email"))
                {
                    throw new BusinessRuleValidationException("Error: This email is already in use !!!");
                }
                if (e.InnerException != null && e.InnerException.Message.Contains("UNIQUE constraint failed: Patient.Phone"))
                {
                    throw new BusinessRuleValidationException("Error: This Phone Number is already in use !!!");
                }
                else
                {
                    throw new BusinessRuleValidationException("Error: Can't save this patient data !!!");
                }
            }
            return _patientMapper.ToPatientDto(patient);
        }
        // Dar um put de um patient profile
        public async Task<PatientDto> UpdateAsync(Guid id, EditPatientDto dto)
        {
            var patient = await _repo.GetByIdAsync(new PatientId(id));
            if (patient == null)
                return null;
            //Guardar a informação sensível antiga
            var oldEmail = patient.Email._Email;
            var oldPhoneNumber = patient.Phone.PhoneNum;


            patient.UpdateDetails(dto.FirstName, dto.LastName, dto.FullName, dto.Email, dto.Phone, dto.Allergies, dto.EmergencyContact);

            //Verificar se houve mudanças
            bool emailChanged = oldEmail != dto.Email;
            bool phoneChanged = oldPhoneNumber != dto.Phone;

            if (emailChanged || phoneChanged)
            {
                var message = "Your contact information has been updated, if you didn't request this change please contact us !!!";
                var subject = "Patient Profile Updated";

                // Manda um email para o email antigo do patient
                await _emailService.SendEmail(oldEmail, message, subject);
            }

            await _repoLog.AddAsync(new Log(patient.ToJSON(), LogType.Update, LogEntity.Patient, patient.Id));

            await _unitOfWork.CommitAsync();

            return _patientMapper.ToPatientDto(patient);
        }

        //Dar um patch de um patient profile
        public async Task<PatientDto> PatchAsync(Guid id, EditPatientDto dto)
        {
            var patient = await _repo.GetByIdAsync(new PatientId(id));
            if (patient == null)
                return null;

            var oldEmail = patient.Email._Email;
            var oldPhoneNumber = patient.Phone.PhoneNum;

            if (!string.IsNullOrEmpty(dto.FirstName))
                patient.ChangeFirstName(dto.FirstName);

            if (!string.IsNullOrEmpty(dto.LastName))
                patient.ChangeLastName(dto.LastName);

            if (!string.IsNullOrEmpty(dto.FullName))
                patient.ChangeFullName(dto.FullName);

            if (!string.IsNullOrEmpty(dto.Email))
                patient.ChangeEmail(dto.Email);

            if (!string.IsNullOrEmpty(dto.Phone))
                patient.ChangePhone(dto.Phone);

            if (dto.Allergies != null)
                patient.ChangeAllergies(dto.Allergies);

            if (!string.IsNullOrEmpty(dto.EmergencyContact))
                patient.ChangeEmergencyContact(dto.EmergencyContact);


            bool emailChanged = oldEmail != dto.Email;
            bool phoneChanged = oldPhoneNumber != dto.Phone;

            if (emailChanged || phoneChanged)
            {
                var message = "Your contact information has been updated, if you didn't request this change please contact us !!!";
                var subject = "Patient Profile Updated";


                await _emailService.SendEmail(oldEmail, message, subject);
            }

            await _repoLog.AddAsync(new Log(patient.ToJSON(), LogType.Update, LogEntity.Patient, patient.Id));

            await _unitOfWork.CommitAsync();
            return _patientMapper.ToPatientDto(patient);
        }

        public async Task<PatientDto> PatchAsPatientAsync(Guid id, EditPatientAsPatientDto dto)
        {
            var patient = await _repo.GetByIdAsync(new PatientId(id));
            if (patient == null)
                return null;

            var oldEmail = patient.Email._Email;
            var oldPhoneNumber = patient.Phone.PhoneNum;

            if (!string.IsNullOrEmpty(dto.FirstName))
                patient.ChangeFirstName(dto.FirstName);

            if (!string.IsNullOrEmpty(dto.LastName))
                patient.ChangeLastName(dto.LastName);

            if (!string.IsNullOrEmpty(dto.FullName))
                patient.ChangeFullName(dto.FullName);

            if (!string.IsNullOrEmpty(dto.Email))
                patient.ChangeEmail(dto.Email);

            if (!string.IsNullOrEmpty(dto.Phone))
                patient.ChangePhone(dto.Phone);

            //if(dto.Allergies != null)
            //patient.ChangeAllergies(dto.Allergies);

            if (!string.IsNullOrEmpty(dto.EmergencyContact))
                patient.ChangeEmergencyContact(dto.EmergencyContact);


            bool emailChanged = oldEmail != dto.Email;
            bool phoneChanged = oldPhoneNumber != dto.Phone;

            if (emailChanged || phoneChanged)
            {
                var message = "Your contact information has been updated, if you didn't request this change please contact us !!!";
                var subject = "Patient Profile Updated";


                await _emailService.SendEmail(oldEmail, message, subject);
            }

            await _repoLog.AddAsync(new Log(patient.ToJSON(), LogType.Update, LogEntity.Patient, patient.Id));

            await _unitOfWork.CommitAsync();
            return _patientMapper.ToPatientDto(patient);
        }

        //Dar delete de um Patient profile
        public async Task<PatientDto> DeleteAsync(Guid id)
        {
            var patient = await _repo.GetByIdAsync(new PatientId(id));

            if (patient == null)
                return null;

            _repo.Remove(patient);

            await _repoLog.AddAsync(new Log(patient.ToJSON(), LogType.Delete, LogEntity.Patient, patient.Id));

            await _unitOfWork.CommitAsync();

            return _patientMapper.ToPatientDto(patient);
        }

        //Get de patients por varios atributos 
        public async Task<List<SearchPatientDto>> SearchPatientsAsync(string name, string email, DateTime? dateOfBirth, string medicalRecordNumber)
        {
            var patients = await _repo.SearchPatientsAsync(name, email, dateOfBirth, medicalRecordNumber);
            List<SearchPatientDto> listDto = new List<SearchPatientDto>();

            foreach (var patient in patients)
                listDto.Add(_patientMapper.ToSearchPatientDto(patient));

            return listDto;
        }
        //Gerar um medical record number de acordo com o Ano Mês e pessoal anterior 
        public async Task<string> GenerateNextMedicalRecordNumber()
        {
            DateTime date = DateTime.Now;
            int year = date.Year;
            int month = date.Month;

            var latestPatient = await _repo.GetLatestPatientByMonthAsync();
            int nextSequential = 1; // se nao existir ninguem começa com o 1

            if (latestPatient != null)
            {
                string latestMedicalRN = latestPatient.MedicalRecordNumber;
                string latestYear = latestMedicalRN.Substring(0, 4);  // Vai buscar os 4 primeiros ou seja o ano
                string latestMonth = latestMedicalRN.Substring(4, 2); // vai buscar os 2 seguintes ou seja o mês 
                string latestSequential = latestMedicalRN.Substring(6); //vai buscar o resto ou seja o numero sequencial

                if (int.Parse(latestYear) == year && int.Parse(latestMonth) == month)
                    nextSequential = int.Parse(latestSequential) + 1;
            }
            string finalSequential = nextSequential.ToString("D6");

            string medicalRecordNumber = $"{year}{month:D2}{finalSequential}";

            return medicalRecordNumber;

        }

        public async void DeletePacientProfileAsync(string email)
        {
            Patient p = await _repo.GetPatientByEmailAsync(new Email(email));

            if (p == null) throw new NullReferenceException("Patient Profile does not exist");
            try
            {
                _repo.Remove(p);
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateException e)
            {
                throw new BusinessRuleValidationException($"Error: Can't save this patient data !!!\n{e.Message}");
            }
            catch
            {
                throw new BusinessRuleValidationException($"Error: Can't save this patient data !!!");
            }
        }

        public async Task<byte[]> createMedicalHistoryJson(string email)
        {
            var patient = await _repo.GetPatientByEmailAsync(new Email(email));

            using var client = new HttpClient();
            var response = await client.GetAsync($"http://localhost:4000/api/patientMedicalRecord/{email}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve patient medical record.");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var externalMedicalRecord = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

            var patientDto = _patientMapper.ToPatientDto(patient);

            var patientDict = JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(patientDto)
            );

            foreach (var entry in externalMedicalRecord)
            {
                patientDict[entry.Key] = entry.Value;
            }

            var finalJson = JsonSerializer.Serialize(patientDict);

            return System.Text.Encoding.UTF8.GetBytes(finalJson);
        }


        public async Task<byte[]> createMedicalHistoryXml(string email)
        {
            var patient = await _repo.GetPatientByEmailAsync(new Email(email));
            var patientDto = _patientMapper.ToPatientDto(patient);

            var serializer = new XmlSerializer(patientDto.GetType());
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            serializer.Serialize(writer, patientDto);
            writer.Flush();

            using var client = new HttpClient();
            var response = await client.GetAsync($"http://localhost:4000/api/patientMedicalRecord/{email}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve patient medical record.");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(jsonResponse);
            var xmlDoc = new XmlDocument();
            var xmlNode = xmlDoc.CreateElement("MedicalRecord");
            xmlNode.InnerXml = JsonToXml(jsonDoc.RootElement);
            xmlDoc.AppendChild(xmlNode);

            memoryStream.Position = 0;
            var patientXmlDoc = new XmlDocument();
            patientXmlDoc.Load(memoryStream);

            XmlNode importedNode = patientXmlDoc.ImportNode(xmlDoc.DocumentElement, true);
            patientXmlDoc.DocumentElement.AppendChild(importedNode);

            using var finalMemoryStream = new MemoryStream();
            patientXmlDoc.Save(finalMemoryStream);

            return finalMemoryStream.ToArray();
        }

        private string JsonToXml(JsonElement element)
        {
            var builder = new StringBuilder();

            foreach (var property in element.EnumerateObject())
            {
                builder.Append($"<{property.Name}>");

                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    builder.Append(JsonToXml(property.Value));
                }
                else
                {
                    builder.Append(property.Value.ToString());
                }

                builder.Append($"</{property.Name}>");
            }

            return builder.ToString();
        }


        public async Task<PatientDto> RequestPatientDataDeletion(DataDeletionPatientRequestDto dto)
        {
            var patient = await _repo.GetPatientByEmailAsync(new Email(dto.Email));

            if (patient == null)
            {
                throw new BusinessRuleValidationException("Error: Email doesn't exist.");
            }

            var dpo = _configuration["EmailSmtp:DPO"];

            await _emailService.SendEmail(dpo, "Patient Data Deletion Request:\nPatient with id " + patient.Id.AsString() + " has requested that his/her personal data be deleted", "Patient wants to delete his/her personal data.");

            return _patientMapper.ToPatientDto(patient);
        }

    }
}

