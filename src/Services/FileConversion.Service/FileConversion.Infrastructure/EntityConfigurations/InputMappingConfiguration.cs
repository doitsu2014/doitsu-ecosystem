﻿using System;
using System.Linq.Expressions;
using FileConversion.Abstraction.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.EntityFrameworkCore;

namespace FileConversion.Infrastructure.EntityConfigurations
{
    public class InputMappingConfiguration : BaseEntityConfiguration<InputMapping>
    {
        public override Expression<Func<InputMapping, object>> KeyExpression => d => new {d.Key, d.InputType};

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