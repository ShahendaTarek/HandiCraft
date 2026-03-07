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
    internal class ChatConfig:IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(c => c.Id);
           

            builder.HasMany(c => c.Participants)
                   .WithOne(p => p.Chat)
                   .HasForeignKey(p => p.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Chat)
                   .HasForeignKey(m => m.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.CreatedAt)
                   .IsRequired();
        }

    }
}
