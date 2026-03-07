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
    internal class ChatParticipantConfig : IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            builder.HasKey(cp => new { cp.ChatId, cp.UserId }); 

            builder.HasOne(cp => cp.Chat)
                   .WithMany(c => c.Participants)
                   .HasForeignKey(cp => cp.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.User)
                   .WithMany()
                   .HasForeignKey(cp => cp.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(cp => cp.JoinedAt)
                   .IsRequired();
        }
    }
}

