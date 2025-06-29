using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice.Domain.Users;
using Backoffice.Infraestructure.Shared;

namespace Backoffice.Infraestructure.Users
{
    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", SchemaNames.Backoffice);
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => new UserId(value));

            builder.OwnsOne(u => u.Password);
            builder.Property(r => r.Role).HasConversion(new RoleConverter());

            //builder.OwnsOne(v => v.Email);

            builder.Property(e => e.Email).HasConversion(new EmailConverter());
            builder.HasIndex(e => e.Email).IsUnique();


        }
    }
}