using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice.Domain.Logs;

namespace Backoffice.Infraestructure.Logs
{
    internal class LogEntityTypeConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Logs", SchemaNames.Backoffice);

            builder.HasKey(b => b.Id);


            builder.Property(b => b.Id)
                   .ValueGeneratedOnAdd();


            builder.Property(b => b.LogType)
                .HasConversion(
                    v => v.ToString(),
                    v => (LogType)Enum.Parse(typeof(LogType), v)
                )
                .IsRequired();

            builder.Property(b => b.LogEntity)
            .HasConversion(
                v => v.ToString(),
                v => (LogEntity)Enum.Parse(typeof(LogEntity), v)
            )
            .IsRequired();

            builder.Property(b => b.Description)
                .IsRequired();
        }
    }
}