using System;
using System.Linq.Expressions;
using ACOMSaaS.NetCore.EFCore.Abstractions.EntityConfiguration;
using FileConversion.Abstraction.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileConversion.Data.EntityConfigurations
{
    class InputMappingConfiguration : BaseConfiguration<InputMapping>
    {
        public override Expression<Func<InputMapping, object>> KeyExpression => x => new { x.Key, x.InputType };

        public override void Configure(EntityTypeBuilder<InputMapping> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Key).HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(512);
            builder.Property(p => p.Mapper).HasMaxLength(256);
            builder.Property(p => p.XmlConfiguration).HasMaxLength(4000);
            builder.Property(p => p.MapperSourceTextId).IsRequired(false);
        }
    }
}
