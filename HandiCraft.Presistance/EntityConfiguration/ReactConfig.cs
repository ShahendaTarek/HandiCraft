using HandiCraft.Domain.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Presistance.EntityConfiguration
{
    internal class ReactConfig : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);





            builder .HasOne(r => r.Post)
                      .WithMany(p => p.Reactions)
                      .HasForeignKey(r => r.PostId)
                      .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
