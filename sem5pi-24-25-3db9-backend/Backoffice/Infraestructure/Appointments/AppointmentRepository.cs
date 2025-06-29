using Backoffice.Domain.Appointments;
using Backoffice.Domain.Staffs;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.Appointments
{
    public class AppointmentRepository : BaseRepository<Appointment, AppointmentId>, IAppointmentRepository
    {
        private readonly BDContext _context;
        public AppointmentRepository(BDContext context) : base(context.Appointments)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.SurgeryRoom)
                .ThenInclude(s => s.SurgeryRoomType)
                .ToListAsync()
            ;
        }

        public async Task<List<Appointment>> GetAllAsDoctorAsync(StaffId doctorId)
        {
            return await _context.Appointments
                .Where(a => a.OperationRequest.DoctorId == doctorId)
                .Include(a => a.SurgeryRoom)
                .ThenInclude(s => s.SurgeryRoomType)
                .ToListAsync()
            ;
        }

        public async Task<List<Appointment>> GetAllWithDetailsAsync()
        {
            return await _context.Appointments
                .Include(a => a.OperationRequest).ThenInclude(a => a.OpType).ThenInclude(a => a.RequiredStaff).ThenInclude(a => a.Specialization)
                .Include(a => a.OperationRequest).ThenInclude(a => a.Patient)
                .Include(a => a.OperationRequest).ThenInclude(a => a.Doctor)
                .Include(a => a.SurgeryRoom)
                .ThenInclude(s => s.SurgeryRoomType)
                .ToListAsync()
            ;
        }

        public async Task<Appointment> GetWithDetailsAsync(AppointmentId id)
        {
            return await _context.Appointments
                .Include(a => a.OperationRequest)
                //.ThenInclude(or => or.Id)
                .Include(a => a.SurgeryRoom)
                .ThenInclude(s => s.SurgeryRoomType)
                //.ThenInclude(s => s.RoomNumber)
                .FirstOrDefaultAsync(a => a.Id == id)
            ;
        }

        public async Task<List<Appointment>> GetByDateWithDetailsAsync(DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.OperationRequest).ThenInclude(a => a.OpType).ThenInclude(a => a.RequiredStaff).ThenInclude(a => a.Specialization)
                .Include(a => a.OperationRequest).ThenInclude(a => a.Patient)
                .Include(a => a.OperationRequest).ThenInclude(a => a.Doctor)
                .Include(a => a.SurgeryRoom)
                .ThenInclude(s => s.SurgeryRoomType)
                .Where(a => a.DateTime.Date == date.Date)
                .ToListAsync();
        }
    }
}