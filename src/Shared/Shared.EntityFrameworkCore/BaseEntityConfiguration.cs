using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Abstraction.Interfaces.Entities;

namespace Shared.EntityFrameworkCore
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> 
        where TEntity : class
    {
        private const string DefaultSchema = "dbo";
        public virtual string Schema => DefaultSchema;
        public abstract Expression<Func<TEntity, object>> KeyExpression { get; }

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(KeyExpression);

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