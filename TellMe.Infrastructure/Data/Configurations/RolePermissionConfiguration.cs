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
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");

            builder.HasKey(rp => rp.Id);

            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();

            builder.Property(rp => rp.RoleId)
                .IsRequired();

            builder.Property(rp => rp.PermissionId)
                .IsRequired();
        }
    }
}
