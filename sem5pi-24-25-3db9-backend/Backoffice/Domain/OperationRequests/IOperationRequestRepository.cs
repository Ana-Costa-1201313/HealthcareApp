using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Patients;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.OperationRequests.ValueObjects;

namespace Backoffice.Domain.OperationRequests
{
    public interface IOperationRequestRepository : IRepository<OperationRequest, OperationRequestId>
    {
        public Task<List<OperationRequest>> GetAllOpRequestsAsync();
        public Task<OperationRequest> GetOpRequestByIdAsync(OperationRequestId id);
        public Task<OperationRequest> PickOperationRequestByIdAsync(OperationRequestId id, List<string> selectedStaff);
        public Task<List<OperationRequest>> GetOpRequestsByDoctorIdAsync(StaffId doctorId);
        public Task<List<OperationRequest>> FilterOpRequestsAsync(StaffId doctorId, PatientId patientId, string operationTypeName, Priority? priority, Status? status);
        public Task<List<OperationRequest>> DeleteOpRequestAsync(OperationRequestId id);
        public Task<List<OperationRequest>> GetListByIdWithDetailsAsync(List<OperationRequestId> ids);
        public Task<OperationRequest> GetOpRequestByIdWithDetailsAsync(OperationRequestId id);
        /*public Task<List<OperationRequest>> GetOpRequestsByPatientIdAsDoctorAsync(StaffId doctorId, PatientId patientId);
        public Task<List<OperationRequest>> GetOpRequestsByOperationTypeIdAsDoctorAsync(StaffId doctorId, OperationTypeId operationTypeName);
        public Task<List<OperationRequest>> GetOpRequestsByPriorityAsDoctorAsync(StaffId doctorId, Priority priority);
        public Task<List<OperationRequest>> GetOpRequestsByStatusAsDoctorAsync(StaffId doctorId, Status status);*/
    }
}