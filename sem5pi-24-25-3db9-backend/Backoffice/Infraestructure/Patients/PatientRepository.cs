using Backoffice.Domain.Patients;
using Backoffice.Domain.Shared;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.Patients
{
    public class PatientRepository : BaseRepository<Patient, PatientId>, IPatientRepository
    {
        private readonly BDContext _context;
        public PatientRepository(BDContext context) : base(context.Patients)
        {
            _context = context;
        }

        public async Task<List<Patient>> SearchPatientsAsync(string name, string email, DateTime? dateOfBirth, string medicalRecordNumber)
        {
            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.FullName.Contains(name));

            if (dateOfBirth.HasValue)
                query = query.Where(p => p.DateOfBirth == dateOfBirth);

            if (!string.IsNullOrEmpty(medicalRecordNumber))
                query = query.Where(p => p.MedicalRecordNumber == medicalRecordNumber);

            // Carrega a lista resultante para aplicar o filtro de email em memória
            var result = await query.ToListAsync();

            // Filtro de email em memória
            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(p => p.Email._Email.Contains(email)).ToList();
            }

            return result;
        }


        public async Task<Patient> GetPatientByEmailAsync(Email email)
        {
            return await _context.Patients.Where(p => p.Email == email)
                .Include(p => p.AppointmentHistory)
                .ThenInclude(a => a.OperationRequest)
                .ThenInclude(o => o.OpType)
                .Include(p => p.AppointmentHistory)
                .ThenInclude(a => a.SurgeryRoom)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient> GetLatestPatientByMonthAsync()
        {
            return await _context.Patients.OrderByDescending(p => p.MedicalRecordNumber).FirstOrDefaultAsync();


        }

        public async Task<Patient> GetWithDetailsAsync(PatientId id)
        {
            return await _context.Patients
                .Include(p => p.AppointmentHistory)
                    .ThenInclude(a => a.OperationRequest)
                        .ThenInclude(o => o.OpType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Patient>> GetAllWithDetailsAsync()
        {
            return await _context.Patients
            .Include(p => p.AppointmentHistory)
            .ThenInclude(a => a.OperationRequest)
                .ThenInclude(o => o.OpType)
            .Include(p => p.AppointmentHistory)
                .ThenInclude(a => a.SurgeryRoom)
            .ToListAsync();
        }
    }
}