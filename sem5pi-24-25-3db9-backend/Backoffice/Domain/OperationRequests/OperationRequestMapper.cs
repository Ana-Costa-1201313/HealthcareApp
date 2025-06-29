using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Staffs;

namespace Backoffice.Domain.OperationRequests
{
    public static class OperationRequestMapper
    {
        public static OperationRequestDto ToDto(OperationRequest operationRequest, OperationTypeDto opType, string patientName, string doctorName)
        {
            if (operationRequest == null) throw new ArgumentNullException(nameof(operationRequest));

            return new OperationRequestDto
            {
                Id = operationRequest.Id?.AsGuid() ?? Guid.Empty,
                OpTypeName = opType,
                OpTypeId = operationRequest.OpTypeId?.AsGuid() ?? Guid.Empty,
                DeadlineDate = operationRequest.DeadlineDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                Priority = operationRequest.Priority.ToString(),
                PatientName = patientName,
                PatientId = operationRequest.PatientId?.AsGuid() ?? Guid.Empty,
                DoctorName = doctorName,
                DoctorId = operationRequest.DoctorId?.AsGuid() ?? Guid.Empty,
                Status = operationRequest.Status.ToString(),
                Description = operationRequest.Description ?? string.Empty,
                SelectedStaff = operationRequest.SelectedStaff ?? new List<string>()
            };
        }

        public static OperationRequest ToDomain(CreateOperationRequestDto dto, OperationType operationType, Patient patient, Staff doctor)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new OperationRequest(
                operationType,
                DateTime.Parse(dto.DeadlineDate),
                Enum.TryParse<Priority>(dto.Priority, out var priority) ? priority : (Priority?)null,
                patient,
                doctor,
                dto.Description,
                new List<string>()
            );
            //return null;
        }

        public static OperationRequest ToDomainPicked(CreatePickedOperationRequestDto dto, OperationType operationType, Patient patient, Staff doctor, List<string> selectedStaff)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new OperationRequest(
                operationType,
                DateTime.Parse(dto.DeadlineDate),
                Enum.TryParse<Priority>(dto.Priority, out var priority) ? priority : (Priority?)null,
                patient,
                doctor,
                dto.Description,
                selectedStaff
            );
            //return null;
        }

        public static OperationRequest ToDomainTests(OperationType operationType, DateTime deadlineDate, Priority priority, Patient patient, Staff doctor, string description)
        {
            return new OperationRequest(
                operationType,
                deadlineDate,
                priority,
                patient,
                doctor,
                description,
                new List<string>()
            );
        }/*
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            
            return new OperationRequest(
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