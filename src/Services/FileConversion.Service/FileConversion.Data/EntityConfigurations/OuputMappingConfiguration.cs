using System;
using System.Linq.Expressions;
using ACOMSaaS.NetCore.EFCore.Abstractions.EntityConfiguration;
using FileConversion.Abstraction.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileConversion.Data.EntityConfigurations
{
    class OutputMappingConfiguration : BaseConfiguration<OutputMapping>
    {
        public override Expression<Func<OutputMapping, object>> KeyExpression => x => new { x.Key };

        public override void Configure(EntityTypeBuilder<OutputMapping> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.XmlConfiguration).HasMaxLength(4000);
        }
    }
}
