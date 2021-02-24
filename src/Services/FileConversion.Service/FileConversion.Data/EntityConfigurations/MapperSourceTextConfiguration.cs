﻿using System;
using System.Linq.Expressions;
using FileConversion.Abstraction.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.EntityFrameworkCore;

namespace FileConversion.Data.EntityConfigurations
{
    public class MapperSourceTextConfiguration : BaseEntityConfiguration<MapperSourceText, Guid>
    {
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
