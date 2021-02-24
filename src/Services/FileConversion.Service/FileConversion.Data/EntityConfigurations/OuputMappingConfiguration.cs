using System;
using System.Linq.Expressions;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.EntityFrameworkCore;

namespace FileConversion.Data.EntityConfigurations
{
    class OutputMappingConfiguration : BaseEntityConfiguration<OutputMapping, InputType>
    {
        public override void Configure(EntityTypeBuilder<OutputMapping> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.XmlConfiguration).HasMaxLength(4000);
        }
    }
}
