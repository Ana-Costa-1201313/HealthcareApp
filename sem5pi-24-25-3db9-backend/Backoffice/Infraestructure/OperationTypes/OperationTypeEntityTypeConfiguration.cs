using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice.Domain.OperationTypes;

namespace Backoffice.Infraestructure.OperationTypes
{
    internal class OperationTypeEntityTypeConfiguration : IEntityTypeConfiguration<OperationType>
    {
        public void Configure(EntityTypeBuilder<OperationType> builder)
        {
            builder.ToTable("OperationTypes", SchemaNames.Backoffice);
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => new OperationTypeId(value));

            builder.OwnsOne(b => b.Name, n =>
            {
                n.Property(c => c.Name).HasColumnName("Name").IsRequired();
                n.HasIndex(c => c.Name).IsUnique();
            });

            builder.OwnsOne(b => b.Duration);

            builder.OwnsMany(b => b.RequiredStaff, rs =>
            {
                rs.ToTable("RequiredStaff");

                rs.HasKey("SpecializationId", "OperationTypeId");

                rs.HasOne(staff => staff.Specialization)
                    .WithMany()
                    .HasForeignKey(staff => staff.SpecializationId)
                    .IsRequired();

                rs.Property(staff => staff.Total)
                    .HasColumnName("Total")
                    .IsRequired();

                rs.WithOwner().HasForeignKey("OperationTypeId");
            });

            builder.Property<bool>(b => b.Active);
        }
    }
}