using Backoffice.Domain.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backoffice.Infraestructure.Appointments
{
    internal class AppointmentEntityTypeConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments", SchemaNames.Backoffice);

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => new AppointmentId(value));

            builder.HasOne(b => b.OperationRequest)
                .WithMany()
                .HasForeignKey(b => b.OperationRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.SurgeryRoom)
                .WithMany()
                .HasForeignKey(b => b.SurgeryRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.DateTime)
                .HasColumnName("DateTime").IsRequired();

            builder.Property(b => b.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), s))
                .HasColumnName("Status")
                .IsRequired();
            
            builder.Property(b => b.PatientId)
                .HasColumnName("PatientId")
                .IsRequired();
        }
    }
}