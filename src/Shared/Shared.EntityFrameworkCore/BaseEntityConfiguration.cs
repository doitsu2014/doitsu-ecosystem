using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Abstraction.Interfaces.Entities;

namespace Shared.EntityFrameworkCore
{
    public abstract class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity> 
        where TEntity : Entity<TKey>
    {
        private const string DefaultSchema = "dbo";
        public virtual string Schema => DefaultSchema;

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);

            if (Schema != "dbo")
            {
                builder.ToTable<TEntity>(typeof(TEntity).Name, this.Schema);
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasQueryFilter(p => !((ISoftDeletable) p).Deleted);
            }

            if (typeof(IConcurrencyCheck).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property(p => ((IConcurrencyCheck) p).Timestamp).IsRowVersion();
            }
        }
    }
}