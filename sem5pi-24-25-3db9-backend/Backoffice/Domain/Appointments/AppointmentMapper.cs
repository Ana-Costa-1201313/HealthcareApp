using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.SurgeryRooms;

namespace Backoffice.Domain.Appointments
{
    public static class AppointmentMapper
    {
        public static AppointmentDto ToDto(Appointment appointment)
        {
            if (appointment == null) throw new ArgumentNullException(nameof(appointment));

            return new AppointmentDto
            {
                AppointmentId = appointment.Id?.AsGuid() ?? Guid.Empty,
                OpRequestId = appointment.OperationRequestId?.AsGuid() ?? Guid.Empty,
                //StaffIds = appointment.OperationRequest.SelectedStaff ?? new List<string>(),
                SurgeryRoomId = appointment.SurgeryRoomId?.AsGuid() ?? Guid.Empty,
                SurgeryRoomNumber = appointment.SurgeryRoom.RoomNumber,
                DateTime = appointment.DateTime.ToString(),
                Status = appointment.Status.ToString()
            };
        }

        public static AppointmentWithDetailsDto ToDtoWithDetails(Appointment appointment, OperationRequest operationRequest, OperationType operationType)
        {
            if (appointment == null) throw new ArgumentNullException(nameof(appointment));
            var operationTypeMapper = new OperationTypeMapper();

            if (operationRequest == null) throw new ArgumentNullException(nameof(operationRequest));

            Guid AppointmentId = appointment.Id?.AsGuid() ?? Guid.Empty;
            OperationRequestDto OperationRequest = new OperationRequestDto();
            OperationTypeDto OperationType = new OperationTypeDto();

            try{
                OperationType = operationTypeMapper.ToDto(operationType);
            }
            catch (Exception)
            {
                throw new Exception("Error mapping OperationTypeDto");
            }
            
            try
            {
                OperationRequest = OperationRequestMapper.ToDto(operationRequest, operationTypeMapper.ToDto(operationRequest.OpType), operationRequest.Patient.FullName, operationRequest.Doctor.Name);
            }
            catch (Exception)
            {
                throw new Exception("Error mapping OperationRequestDto"); 
            }
            Guid SurgeryRoomId = appointment.SurgeryRoomId?.AsGuid() ?? Guid.Empty;
            Int32 SurgeryRoomNumber = appointment.SurgeryRoom.RoomNumber;
            string DateTime = appointment.DateTime.ToString();
            string Status = appointment.Status.ToString();

            AppointmentWithDetailsDto dto = new AppointmentWithDetailsDto
            {
                AppointmentId = AppointmentId,
                OperationRequest = OperationRequest,
                SurgeryRoomId = SurgeryRoomId,
                SurgeryRoomNumber = SurgeryRoomNumber,
                DateTime = DateTime,
                Status = Status
            };

            return dto;
        }

        public static PatientAppointmentDto ToPatientDto(Appointment appointment)
        {
            if (appointment == null) throw new ArgumentNullException(nameof(appointment));

            return new PatientAppointmentDto
            {
                //AppointmentId = appointment.Id?.AsGuid() ?? Guid.Empty,
                //OpRequestId = appointment.OperationRequestId?.AsGuid() ?? Guid.Empty,
                //StaffIds = appointment.OperationRequest.SelectedStaff ?? new List<string>(),
                OpTypeName = appointment.OperationRequest.OpType.Name.Name,
                //SurgeryRoomId = appointment.SurgeryRoomId?.AsGuid() ?? Guid.Empty,
                //SurgeryRoomNumber = appointment.SurgeryRoom.RoomNumber,
                DateTime = appointment.DateTime.ToString(),
                Status = appointment.Status.ToString()
            };
        }

        public static Appointment ToDomain(CreateAppointmentDto dto, OperationRequest operationRequest, /*List<Staff> staff,*/ SurgeryRoom surgeryRoom)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Appointment(
                operationRequest,
                //staff,
                surgeryRoom,
                DateTime.Parse(dto.DateTime)
            );
            //return null;
        }/*

        public static Appointment ToDomainTests(OperationType operationType, DateTime deadlineDate, Priority priority, Patient patient, Staff doctor, string description)
        {
            return new Appointment(
                operationType,
                deadlineDate,
                priority,
                patient,
                doctor,
                description
            );
        }
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            
            return new Appointment(
                operationType,
                DateTime.Parse(dto.DeadlineDate),
                Enum.TryParse<Priority>(dto.Priority, out var priority) ? priority : (Priority?)null,
                patient,
                doctor,
                dto.Description
            );
            //return null;
        }*/
    }
}