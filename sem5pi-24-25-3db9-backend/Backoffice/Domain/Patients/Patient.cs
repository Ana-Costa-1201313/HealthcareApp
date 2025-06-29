using System.Security.Cryptography;
using Backoffice.Domain.Appointments;
using Backoffice.Domain.Shared;
using Newtonsoft.Json;

namespace Backoffice.Domain.Patients
{
   public class Patient : Entity<PatientId>, IAggregateRoot
   {
      public string FirstName { get; private set; }

      public string LastName { get; private set; }

      public string FullName { get; private set; }

      public string Gender { get; private set; }

      public DateTime DateOfBirth { get; private set; }

      public Email Email { get; private set; }

      public PhoneNumber Phone { get; private set; }
      public PhoneNumber EmergencyContact { get; private set; }

      public string[] Allergies { get; private set; }

      public string MedicalRecordNumber { get; private set; }

      public List<Appointment> AppointmentHistory { get; private set; }

      public bool isInactive { get; private set; }
      private Patient() { }

      public Patient(CreatePatientDto dto, string sequencialNumber)
      {
         this.Id = new PatientId(Guid.NewGuid());
         this.FirstName = dto.FirstName;
         this.LastName = dto.LastName;
         this.FullName = dto.FullName;
         this.Gender = dto.Gender;
         this.DateOfBirth = dto.DateOfBirth;
         this.Email = new Email(dto.Email);
         this.Phone = new PhoneNumber(dto.Phone);
         this.EmergencyContact = new PhoneNumber(dto.EmergencyContact);
         this.MedicalRecordNumber = sequencialNumber;
         this.AppointmentHistory = new List<Appointment>();
      }

      public void UpdateDetails(string firstName, string lastName, string fullName,
       string email, string phoneNumber, string[] allergies, string emergencyContact)
      {
         this.FirstName = firstName;
         this.LastName = lastName;
         this.FullName = fullName;
         this.Email = new Email(email);
         this.Phone = new PhoneNumber(phoneNumber);
         this.Allergies = allergies;
         this.EmergencyContact = new PhoneNumber(emergencyContact);
      }

      public void MarkAsInactive()
      {
         isInactive = true;
      }

      public void ChangeFirstName(string firstName)
      {
         this.FirstName = firstName;
      }
      public void ChangeLastName(string lastName)
      {
         this.LastName = lastName;
      }
      public void ChangeFullName(string fullName)
      {
         this.FullName = fullName;
      }
      public void ChangeEmail(string email)
      {
         this.Email = new Email(email);
      }
      public void ChangePhone(string phone)
      {
         this.Phone = new PhoneNumber(phone);
      }
      public void ChangeAllergies(string[] allergies)
      {
         this.Allergies = allergies;
      }
      public void ChangeEmergencyContact(string emergencyContact)
      {
         this.EmergencyContact = new PhoneNumber(emergencyContact);
      }

      public void AddAppointment(Appointment appointment)
      {
         this.AppointmentHistory.Add(appointment);
      }

      public void UpdateAppointment(Appointment appointment)
      {
         var existingAppointment = this.AppointmentHistory.FirstOrDefault(appointment);
         if (existingAppointment != null)
         {
            var index = this.AppointmentHistory.IndexOf(existingAppointment);
            this.AppointmentHistory[index] = appointment;
         }
      }

      public string ToJSON()
      {
         var jsonRepresentation = new
         {
            PatientId = this.Id,
            FullName = this.FullName,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Gender = this.Gender,
            DateOfBirth = this.DateOfBirth.ToString("yyyy-MM-dd"),
            Email = this.Email._Email,
            Phone = this.Phone.PhoneNum,
            EmergencyContact = this.EmergencyContact,
            Allergies = this.Allergies != null ? string.Join(", ", this.Allergies) : null,
            MedicalRecordNumber = this.MedicalRecordNumber,
            IsInactive = this.isInactive,
         };

         return JsonConvert.SerializeObject(jsonRepresentation, Formatting.Indented);
      }
   }
}