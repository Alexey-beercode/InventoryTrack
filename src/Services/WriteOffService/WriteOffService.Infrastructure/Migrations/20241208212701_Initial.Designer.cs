﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WriteOffService.Infrastructure.Config.Database;

#nullable disable

namespace WriteOffService.Infrastructure.Migrations
{
    [DbContext(typeof(WriteOffDbContext))]
    [Migration("20241208212701_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WriteOffService.Domain.Entities.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("WriteOffService.Domain.Entities.WriteOffAct", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DocumentId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("WriteOffRequestId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("WriteOffRequestId");

                    b.ToTable("WriteOffActs", (string)null);
                });

            modelBuilder.Entity("WriteOffService.Domain.Entities.WriteOffReason", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.ToTable("WriteOffReasons", (string)null);
                });

            modelBuilder.Entity("WriteOffService.Domain.Entities.WriteOffRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ApprovedByUserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("uuid");

                    b.Property<long>("Quantity")
                        .HasColumnType("bigint");

                    b.Property<Guid>("ReasonId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("WarehouseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("ReasonId");

                    b.HasIndex("Status");

                    b.HasIndex("WarehouseId");

                    b.ToTable("WriteOffRequests", (string)null);
                });

            modelBuilder.Entity("WriteOffService.Domain.Entities.WriteOffAct", b =>
                {
                    b.HasOne("WriteOffService.Domain.Entities.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WriteOffService.Domain.Entities.WriteOffRequest", "WriteOffRequest")
                        .WithMany()
                        .HasForeignKey("WriteOffRequestId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("WriteOffRequest");
                });

            modelBuilder.Entity("WriteOffService.Domain.Entities.WriteOffRequest", b =>
                {
                    b.HasOne("WriteOffService.Domain.Entities.WriteOffReason", "Reason")
                        .WithMany()
                        .HasForeignKey("ReasonId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Reason");
                });
#pragma warning restore 612, 618
        }
    }
}
