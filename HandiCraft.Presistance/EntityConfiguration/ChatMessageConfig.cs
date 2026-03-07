using HandiCraft.Domain.UserConnections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Presistance.EntityConfiguration
{
    internal class ChatMessageConfig: IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Chat)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(x => x.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Sender)
                   .WithMany() 
                   .HasForeignKey(x => x.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Text)
                   .IsRequired();
                   

            builder.Property(x => x.SentAt)
                   .IsRequired();

            builder.Property(x => x.IsRead)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(x => x.ReadAt)
                   .IsRequired(false);
        }
    }
}
