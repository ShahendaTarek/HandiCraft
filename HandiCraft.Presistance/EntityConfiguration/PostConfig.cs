using HandiCraft.Domain.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Presistance.EntityConfiguration
{
    internal class PostConfig : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);


             builder.HasMany(p => p.MediaItems)
                    .WithOne(m => m.Post)
                    .HasForeignKey(m => m.PostId);
            
        }
    }
}
