using System;
using System.Linq.Expressions;
using ACOMSaaS.NetCore.EFCore.Abstractions.EntityConfiguration;
using FileConversion.Abstraction.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileConversion.Data.EntityConfigurations
{
    public class MapperSourceTextConfiguration : BaseConfiguration<MapperSourceText>
    {
        public override Expression<Func<MapperSourceText, object>> KeyExpression => x => x.Id;

        public override void Configure(EntityTypeBuilder<MapperSourceText> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.SourceText).HasMaxLength(4000);
            builder.HasMany(p => p.InputMappings)
                .WithOne(p => p.MapperSourceText)
                .HasForeignKey(p => p.MapperSourceTextId);
        }
    }
}
