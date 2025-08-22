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
    internal class BlockConfig : IEntityTypeConfiguration<UserBlock>
    {
        public void Configure(EntityTypeBuilder<UserBlock> builder)
        {
            builder.HasKey(x => new { x.BlockerId, x.BlockedId });


            builder.HasOne(x => x.Blocker)
                   .WithMany()
                   .HasForeignKey(x => x.BlockerId)
                   .OnDelete(DeleteBehavior.Restrict);

             builder.HasOne(x => x.Blocked)
                    .WithMany()
                    .HasForeignKey(x => x.BlockedId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
