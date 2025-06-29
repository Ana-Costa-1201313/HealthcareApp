using System.Text.Json.Serialization;
using Backoffice.Domain.OperationRequests.ValueObjects;
using Backoffice.Domain.OperationTypes;

namespace Backoffice.Domain.OperationRequests
{
    public class OperationRequestDto
    {
        public Guid Id { get; set; }
        public OperationTypeDto OpTypeName { get; set; }
        public Guid OpTypeId { get; set; }
        public string DeadlineDate { get; set; }
        public string Priority { get; set; }
        public string PatientName { get; set; }
        public Guid PatientId { get; set; }
        public string DoctorName { get; set; }
        public Guid DoctorId { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<string> SelectedStaff { get; set; }
    }
}