using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice.Domain.Staffs;
using Backoffice.Infraestructure.Shared;

namespace Backoffice.Infraestructure.Staffs
{
    internal class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staff", SchemaNames.Backoffice);
            builder.HasKey(b => b.Id);

            builder.Property(r => r.Role).HasConversion(new RoleConverter());

            builder.Property(l => l.LicenseNumber).HasConversion(new LicenseNumberConverter());

            builder.Property(s => s.Phone).HasConversion(new PhoneNumberConverter());
            builder.HasIndex(s => s.Phone).IsUnique();

            builder.Property(m => m.MechanographicNum).HasConversion(new MechanographicNumConverter());
            builder.HasIndex(m => m.MechanographicNum).IsUnique();

            builder.Property(e => e.Email).HasConversion(new EmailConverter());
            builder.HasIndex(e => e.Email).IsUnique();

            builder.HasIndex(l => l.LicenseNumber).IsUnique();

            builder.HasOne(s => s.Specialization);

            builder.OwnsMany(s => s.AvailabilitySlots, a =>
            {
                a.ToTable("AvailabilitySlot");
                a.HasKey("StaffId", "StartTime", "EndTime");
                a.WithOwner().HasForeignKey("StaffId");
            });
        }
    }
}