using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backoffice.Infraestructure.SurgeryRooms
{
    internal class SurgeryRoomTypeEntityTypeConfiguration : IEntityTypeConfiguration<SurgeryRoomType>
    {
        public void Configure(EntityTypeBuilder<SurgeryRoomType> builder)
        {

            builder.ToTable("SurgeryRoomTypes", SchemaNames.Backoffice);
            
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => new SurgeryRoomTypeId(value));

            builder.Property<string>(b => b.Type).IsRequired();
            // builder.OwnsOne(b=> b.Type, n =>{
            //     n.Property(c => c.Type).IsUnique();
            // })
            builder.Property<bool>(b => b.fitForSurgery).IsRequired();
            builder.Property<string>(b => b.Description);
        }
    }
}