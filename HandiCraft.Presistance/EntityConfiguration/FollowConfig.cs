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
    internal class FollowConfig : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.HasKey(x => new { x.FollowerId, x.FollowedId });

            builder.HasOne(x => x.Follower)
                   .WithMany()
                   .HasForeignKey(x => x.FollowerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Followed)
                   .WithMany()
                   .HasForeignKey(x => x.FollowedId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
