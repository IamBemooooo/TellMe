using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Core.Entities;

namespace TellMe.Infrastructure.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(ur => ur.Id);

            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            builder.Property(ur => ur.UserId)
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .IsRequired();
        }
    }
}
