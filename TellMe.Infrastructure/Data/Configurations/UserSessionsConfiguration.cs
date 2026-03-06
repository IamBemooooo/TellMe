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
    public class UserSessionsConfiguration : IEntityTypeConfiguration<UserSessions>
    {
        public void Configure(EntityTypeBuilder<UserSessions> builder)
        {
            builder.ToTable("UserSessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ExpiredAt)
                .IsRequired();

            builder.Property(x => x.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(x => x.User)
                .WithMany(u => u.UserSessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
