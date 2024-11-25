using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configuration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasMany(p => p.PostUpvotes)
                .WithOne(p => p.Post)
                .HasForeignKey(p => p.PostId);

            builder.HasMany(p => p.PostDevotes)
                .WithOne(p => p.Post)
                .HasForeignKey(p => p.PostId);

            builder.HasMany(p => p.PostShares)
                .WithOne(p => p.Post)
                .HasForeignKey(p => p.PostId);
        }
    }
}
