using Backoffice.Infraestructure.Shared;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Staffs;
using Microsoft.EntityFrameworkCore;
using Backoffice.Domain.OperationRequests.ValueObjects;
using Backoffice.Domain.Shared;

namespace Backoffice.Infraestructure.OperationRequests
{
    public class OperationRequestRepository : BaseRepository<OperationRequest, OperationRequestId>, IOperationRequestRepository
    {
        private readonly BDContext _context;
        public OperationRequestRepository(BDContext context) : base(context.OperationRequests)
        {
            _context = context;
        }

        public async Task<List<OperationRequest>> GetAllOpRequestsAsync()
        {
            return await _context.OperationRequests
                .ToListAsync()
            ;
        }

        public async Task<OperationRequest> GetOpRequestByIdAsync(OperationRequestId id)
        {
            return await _context.OperationRequests
                .Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }

        public async Task<OperationRequest> GetOpRequestByIdWithDetailsAsync(OperationRequestId id)
        {
            return await _context.OperationRequests
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.OpType)
                .ThenInclude(ot => ot.RequiredStaff)
                .ThenInclude(rs => rs.Specialization)
                .Include(x => x.OpType.Duration)
                .Where(x => id.Equals(x.Id)).FirstOrDefaultAsync()
            ;
        }

        public async Task<OperationRequest> PickOperationRequestByIdAsync(OperationRequestId id, List<string> selectedStaff)
        {
            var operationRequest = await _context.OperationRequests
            .Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();

            if (operationRequest != null)
            {
                if (operationRequest.Status == Status.Picked) throw new BusinessRuleValidationException("Error: The operation request is already picked.");

                operationRequest.ChangeStatus(selectedStaff);
                await _context.SaveChangesAsync();
            }

            return operationRequest;
        }

        public async Task<List<OperationRequest>> GetOpRequestsByDoctorIdAsync(StaffId doctorId)
        {
            return await _context.OperationRequests
                .Where(x => x.DoctorId.Equals(doctorId))
                .ToListAsync();
        }

        public async Task<List<OperationRequest>> DeleteOpRequestAsync(OperationRequestId id)
        {
            var entity = await _context.OperationRequests
                .Where(x => id.Equals(x.Id))
                .FirstOrDefaultAsync()
            ;

            if (entity != null)
            {
                _context.OperationRequests.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return await _context.OperationRequests
                .ToListAsync()
            ;
        }

        public async Task<List<OperationRequest>> FilterOpRequestsAsync(StaffId doctorId, PatientId patientId, string operationTypeName, Priority? priority, Status? status)
        {
            var query = _context.OperationRequests
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.OpType)
                .AsQueryable()
            ;

            if (doctorId != null)
            {
                query = query.Where(x => x.Doctor.Id.Equals(doctorId));
            }

            if (patientId != null)
            {
                query = query.Where(x => x.Patient.Id.Equals(patientId));
            }

            if (!string.IsNullOrEmpty(operationTypeName))
            {
                query = query.Where(x => x.OpType.Name.Name.Equals(operationTypeName));
            }

            if (priority.HasValue && priority.Value != Priority.Null)
            {
                query = query.Where(x => x.Priority.Equals(priority.Value));
            }

            if (status.HasValue && status.Value != Status.Null)
            {
                query = query.Where(x => x.Status.Equals(status.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<List<OperationRequest>> GetListByIdWithDetailsAsync(List<OperationRequestId> ids)
        {
            return await _context.OperationRequests
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .ThenInclude(d => d.Specialization)
                .ThenInclude(s => s.Name)
                .Include(x => x.OpType)
                //.ThenInclude(ot => ot.OperationTypeDuration
                .Where(x => ids.Contains(x.Id))
                .ToListAsync()
            ;
        }

        /*
        public async Task<List<OperationRequest>> GetOpRequestsByPatientIdAsDoctorAsync(StaffId staffId, PatientId patientId)
        {
            return await _context.OperationRequests
                .Where(x => x.DoctorId.Equals(staffId) && x.PatientId.Equals(patientId))
                .ToListAsync();
        }

        public async Task<List<OperationRequest>> GetOpRequestsByOperationTypeIdAsDoctorAsync(StaffId staffId, OperationTypeId operationTypeId)
        {
            return await _context.OperationRequests
                .Where(x => x.DoctorId.Equals(staffId) && x.OpTypeId.Equals(operationTypeId))
                .ToListAsync();
        }

        public async Task<List<OperationRequest>> GetOpRequestsByPriorityAsDoctorAsync(StaffId staffId, Priority priority)
        {
            return await _context.OperationRequests
                .Where(x => x.DoctorId.Equals(staffId) && x.Priority.Equals(priority))
                .ToListAsync();
        }

        public async Task<List<OperationRequest>> GetOpRequestsByStatusAsDoctorAsync(StaffId staffId, Status status)
        {
            return await _context.OperationRequests
                .Where(x => x.DoctorId.Equals(staffId) && x.Status.Equals(status))
                .ToListAsync();
        }*/
    }
}
