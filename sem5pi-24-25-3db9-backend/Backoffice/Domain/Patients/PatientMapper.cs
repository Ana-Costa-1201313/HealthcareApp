using Backoffice.Domain.Appointments;
using Backoffice.Domain.Patients;
using Microsoft.IdentityModel.Tokens;

namespace Backoffice.Domain.Patients;

public class PatientMapper
{

    public PatientDto ToPatientDto(Patient patient)
    {
        List<AppointmentDto> appointmentDtos = new List<AppointmentDto>();

        if (!patient.AppointmentHistory.IsNullOrEmpty())
        {
            foreach (Appointment appointment in patient.AppointmentHistory)
            {
                appointmentDtos.Add(new AppointmentDto
                {
                    AppointmentId = appointment.Id.AsGuid(),
                    OpRequestId = appointment.OperationRequestId.AsGuid(),
                    SurgeryRoomId = appointment.SurgeryRoomId.AsGuid(),
                    SurgeryRoomNumber = appointment.SurgeryRoom.RoomNumber,
                    DateTime = appointment.DateTime.ToString("yyyy-MM-dd HH:mm"),
                    Status = appointment.Status.ToString()
                });
            }
        }

        return new PatientDto
        {
            Id = patient.Id.AsGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FullName = patient.FullName,
            Gender = patient.Gender,
            DateOfBirth = patient.DateOfBirth,
            Email = patient.Email._Email,
            Phone = patient.Phone.PhoneNum,
            EmergencyContact = patient.EmergencyContact.PhoneNum,
            Allergies = patient.Allergies,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            AppointmentHistory = appointmentDtos
        };
    }
    public Patient ToPatient(CreatePatientDto dto, string medicalRecordNumber)
    {
        return new Patient(dto, medicalRecordNumber);
    }

    public SearchPatientDto ToSearchPatientDto(Patient patient)
    {
        return new SearchPatientDto
        {
            FullName = patient.FullName,
            Email = patient.Email._Email,
            DateOfBirth = patient.DateOfBirth
        };
    }
}