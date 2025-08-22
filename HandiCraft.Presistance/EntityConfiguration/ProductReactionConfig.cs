using HandiCraft.Domain.ProductList;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Presistance.EntityConfiguration
{
    internal class ProductReactionConfig : IEntityTypeConfiguration<ProductReaction>
    {
        public void Configure(EntityTypeBuilder<ProductReaction> builder)
        {
             builder.HasOne(r => r.Product)
                    .WithMany(p => p.Reactions)
                    .HasForeignKey(r => r.ProductId);

           builder.HasOne(r => r.User)
                  .WithMany()
                  .HasForeignKey(r => r.UserId);

            builder.HasIndex(r => new { r.UserId, r.ProductId })
                   .IsUnique();
        }
    }
}
