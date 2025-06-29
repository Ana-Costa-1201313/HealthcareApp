using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backoffice.Infraestructure.SurgeryRooms
{
    internal class SurgeryRoomEntityTypeConfiguration : IEntityTypeConfiguration<SurgeryRoom>
    {
        public void Configure(EntityTypeBuilder<SurgeryRoom> builder)
        {
            builder.ToTable("SurgeryRooms", SchemaNames.Backoffice);

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => new SurgeryRoomId(value));

            builder.Property(b => b.RoomNumber)
                .HasColumnName("RoomNumber").IsRequired();

            //builder.HasOne(s => s.SurgeryRoomType);
            builder.HasOne(s => s.SurgeryRoomType);
            
            // builder.HasOne(s => s.SurgeryRoomType)
            //     .WithMany() // or .WithOne() depending on your model
            //     .HasForeignKey(s => s.SurgeryRoomType); // Ensure the foreign key is set


            builder.Property(b => b.Capacity)
                .HasColumnName("Capacity").IsRequired();

            builder.Property(b => b.SurgeryRoomStatus)
                .HasConversion(
                    s => s.ToString(),
                    s => (SurgeryRoomStatus)Enum.Parse(typeof(SurgeryRoomStatus), s))
                .HasColumnName("SurgeryRoomStatus")
                .IsRequired();

            builder.OwnsMany(b => b.MaintenanceSlots, a =>
            {
                a.ToTable("MaintenanceSlot");
                a.HasKey("SurgeryRoomId", "StartTime", "EndTime");
                a.WithOwner().HasForeignKey("SurgeryRoomId");
            });
        }
    }
}