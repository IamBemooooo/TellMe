using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TellMe.Core.Entities;

public class MessageReactionConfiguration : IEntityTypeConfiguration<MessageReaction>
{
    public void Configure(EntityTypeBuilder<MessageReaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MessageId).IsRequired().HasMaxLength(500);

        builder.Property(x => x.Reaction).HasMaxLength(50);
        builder.Property(x => x.SenderPsid).HasMaxLength(100);
    }
}