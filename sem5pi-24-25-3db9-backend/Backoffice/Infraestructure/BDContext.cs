using Microsoft.EntityFrameworkCore;
using Backoffice.Domain.Users;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Patients;
using Backoffice.Domain.OperationRequests;
using Backoffice.Infraestructure.Users;
using Backoffice.Infraestructure.Staffs;
using Backoffice.Infraestructure.Patients;
using Backoffice.Infraestructure.OperationRequests;
using Backoffice.Domain.OperationTypes;
using Backoffice.Infraestructure.OperationTypes;
using Backoffice.Domain.Specializations;
using Backoffice.Infraestructure.Specializations;
using Backoffice.Domain.Logs;
using Backoffice.Infraestructure.Logs;
using Backoffice.Domain.Appointments;
using Backoffice.Infraestructure.Appointments;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Backoffice.Infraestructure.SurgeryRooms;

namespace Backoffice.Infraestructure
{
    public class BDContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<OperationRequest> OperationRequests { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<SurgeryRoom> SurgeryRooms { get; set; }
        public DbSet<SurgeryRoomType> SurgeryRoomTypes { get; set; }
        public DbSet<Log> Logs { get; set; }

        public BDContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new StaffEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SpecializationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SurgeryRoomEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SurgeryRoomTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LogEntityTypeConfiguration());

            modelBuilder.Ignore<RequiredStaffDto>();
        }
    }
}