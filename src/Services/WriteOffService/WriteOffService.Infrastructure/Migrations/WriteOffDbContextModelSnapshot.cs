﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WriteOffService.Infrastructure.Config.Database;

#nullable disable

namespace WriteOffService.Infrastructure.Migrations
{
    [DbContext(typeof(WriteOffDbContext))]
    partial class WriteOffDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
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

                    b.HasData(
                        new
                        {
                            Id = new Guid("63707ba8-8dce-4b33-959e-891141d63a77"),
                            IsDeleted = false,
                            Reason = "По причине продажи"
                        },
                        new
                        {
                            Id = new Guid("e70a3815-0c42-4ec2-baf5-58c6ee2f11eb"),
                            IsDeleted = false,
                            Reason = "Истёк срок годности"
                        },
                        new
                        {
                            Id = new Guid("5e90b547-a2d1-436e-a3be-0b9289e71960"),
                            IsDeleted = false,
                            Reason = "Поломка"
                        },
                        new
                        {
                            Id = new Guid("7c019be0-d26d-4f50-8531-33a1bb73256b"),
                            IsDeleted = false,
                            Reason = "Другое"
                        });
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

                    b.Property<Guid>("DocumentId")
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
