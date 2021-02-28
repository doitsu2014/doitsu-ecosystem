﻿// <auto-generated />
using System;
using FileConversion.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FileConversion.Infrastructure.Migrations
{
    [DbContext(typeof(FileConversionContext))]
    [Migration("20210227150147_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("FileConversion.Abstraction.Model.InputMapping", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("InputType")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("Mapper")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int?>("MapperSourceTextId")
                        .HasColumnType("integer");

                    b.Property<int>("StreamType")
                        .HasColumnType("integer");

                    b.Property<string>("XmlConfiguration")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.HasKey("Key", "InputType");

                    b.HasIndex("MapperSourceTextId");

                    b.ToTable("InputMappings");
                });

            modelBuilder.Entity("FileConversion.Abstraction.Model.MapperSourceText", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("SourceText")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.HasKey("Id");

                    b.ToTable("MapperSourceTexts");
                });

            modelBuilder.Entity("FileConversion.Abstraction.Model.OutputMapping", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<bool>("IsXml")
                        .HasColumnType("boolean");

                    b.Property<int?>("NumberOfFooter")
                        .HasColumnType("integer");

                    b.Property<int?>("NumberOfHeader")
                        .HasColumnType("integer");

                    b.Property<string>("XmlConfiguration")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.HasKey("Id");

                    b.ToTable("OutputMappings");
                });

            modelBuilder.Entity("FileConversion.Abstraction.Model.InputMapping", b =>
                {
                    b.HasOne("FileConversion.Abstraction.Model.MapperSourceText", "MapperSourceText")
                        .WithMany("InputMappings")
                        .HasForeignKey("MapperSourceTextId");

                    b.Navigation("MapperSourceText");
                });

            modelBuilder.Entity("FileConversion.Abstraction.Model.MapperSourceText", b =>
                {
                    b.Navigation("InputMappings");
                });
#pragma warning restore 612, 618
        }
    }
}
