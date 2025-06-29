using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;

namespace Backoffice.Domain.Appointments
{
    public interface IAppointmentRepository : IRepository<Appointment, AppointmentId>
    {
        public Task<List<Appointment>> GetAllAppointmentsAsync();
        //public Task<Appointment> GetOpRequestByIdAsync(AppointmentId id);
        //public Task<List<Appointment>> GetOpRequestsByDoctorIdAsync(StaffId doctorId);
        public Task<List<Appointment>> GetAllWithDetailsAsync();
        public Task<List<Appointment>> GetAllAsDoctorAsync(StaffId doctorId);
        public Task<Appointment> GetWithDetailsAsync(AppointmentId id);
        public Task<List<Appointment>> GetByDateWithDetailsAsync(DateTime date);
    }
}