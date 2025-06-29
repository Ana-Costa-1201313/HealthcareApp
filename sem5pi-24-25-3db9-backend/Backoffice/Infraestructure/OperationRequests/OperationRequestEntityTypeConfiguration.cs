using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.OperationRequests.ValueObjects;
using Newtonsoft.Json;

namespace Backoffice.Infraestructure.OperationRequests
{
    internal class OperationRequestEntityTypeConfiguration : IEntityTypeConfiguration<OperationRequest>
    {
        public void Configure(EntityTypeBuilder<OperationRequest> builder)
        {
            builder.ToTable("OperationRequests", SchemaNames.Backoffice);

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => new OperationRequestId(value));

            builder.Property(b => b.DeadlineDate)
                .HasColumnName("DeadlineDate").IsRequired();

            builder.Property(b => b.Priority)
                .HasConversion(
                    p => p.ToString(),
                    p => (Priority)Enum.Parse(typeof(Priority), p))
                .HasColumnName("Priority")
                .IsRequired();

            builder.Property(b => b.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (Status)Enum.Parse(typeof(Status), s))
                .HasColumnName("Status")
                .IsRequired();

            builder.Property(b => b.Description)
                .HasColumnName("Description").IsRequired();

            builder.HasOne(b => b.OpType)
                .WithMany()
                .HasForeignKey(b => b.OpTypeId)
                .OnDelete(DeleteBehavior.Restrict); ;

            builder.HasOne(b => b.Patient)
                .WithMany()
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict); ;

            builder.HasOne(b => b.Doctor)
                .WithMany()
                .HasForeignKey(b => b.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.SelectedStaff)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v))
                .HasColumnName("SelectedStaff");
                //.IsRequired();
        }
    }
}