﻿// <auto-generated />
using FileConversion.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FileConversion.Data.Migrations
{
    [DbContext(typeof(FileConversionContext))]
    [Migration("20190905092743_InitOutputMapping")]
    partial class InitOutputMapping
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("FileConversion.Abstraction.Model.InputMapping", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(50);

                    b.Property<int>("InputType");

                    b.Property<string>("Description")
                        .HasMaxLength(512);

                    b.Property<string>("Mapper")
                        .HasMaxLength(256);

                    b.Property<int>("StreamType");

                    b.Property<string>("XmlConfiguration")
                        .HasMaxLength(4000);

                    b.HasKey("Key", "InputType");

                    b.ToTable("InputMapping");
                });

            modelBuilder.Entity("FileConversion.Abstraction.Model.OutputMapping", b =>
                {
                    b.Property<int>("Key");

                    b.Property<string>("XmlConfiguration")
                        .HasMaxLength(4000);

                    b.HasKey("Key");

                    b.ToTable("OutputMapping");
                });
#pragma warning restore 612, 618
        }
    }
}
