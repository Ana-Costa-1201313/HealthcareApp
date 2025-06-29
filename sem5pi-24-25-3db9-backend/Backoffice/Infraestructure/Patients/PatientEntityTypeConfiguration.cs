using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice.Domain.Patients;
using Backoffice.Infraestructure.Shared;
using Newtonsoft.Json;
using Backoffice.Domain.Appointments;

namespace Backoffice.Infraestructure.Patients
{
    internal class PatientEntityTypeConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patient", SchemaNames.Backoffice);
            builder.HasKey(b => b.Id);


            builder.Property(b => b.FirstName).HasColumnName("FirstName").IsRequired();
            builder.Property(b => b.LastName).HasColumnName("LastName").IsRequired();
            builder.Property(b => b.FullName).HasColumnName("FullName").IsRequired();
            builder.Property(b => b.Gender).HasColumnName("Gender").IsRequired();
            builder.Property(b => b.DateOfBirth).HasColumnName("DateOfBirth").IsRequired();

            builder.Property(b => b.Email).HasConversion(new EmailConverter());
            builder.HasIndex(b => b.Email).IsUnique();

            builder.Property(b => b.Phone).HasConversion(new PhoneNumberConverter());
            builder.HasIndex(b => b.Phone).IsUnique();

            builder.Property(b => b.EmergencyContact).HasColumnName("EmergencyContact").IsRequired();
            builder.HasIndex(b => b.EmergencyContact).IsUnique();

            builder.Property(b => b.EmergencyContact).HasConversion(new PhoneNumberConverter());

            builder.Property(b => b.MedicalRecordNumber).HasColumnName("MedicalRecordNumber").IsRequired();
            builder.HasIndex(b => b.MedicalRecordNumber).IsUnique();

            builder.Property(b => b.Allergies)
           .HasColumnName("Allergies")
           .HasConversion(
               v => string.Join(',', v),   // Converte a lista de alergias para uma string separada por vírgulas ao salvar na bd
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));   // Converte a string de volta para uma lista ao carregar na bd

            builder.HasMany(b => b.AppointmentHistory)
               .WithOne() // There is no navigation property on the Appointment side
               .HasForeignKey("PatientId") // Use the foreign key PatientId in the Appointment table
               .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
