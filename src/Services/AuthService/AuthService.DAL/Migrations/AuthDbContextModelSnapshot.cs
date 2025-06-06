﻿// <auto-generated />
using System;
using AuthService.DAL.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.DAL.Migrations
{
    [DbContext(typeof(AuthDbContext))]
    partial class AuthDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AuthService.Domain.Enities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LegalAddress")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("PostalAddress")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid>("ResponsibleUserId")
                        .HasColumnType("uuid");

                    b.Property<int>("Unp")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ResponsibleUserId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("AuthService.Domain.Enities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("ac5e0c16-3823-4e6f-9efb-aadaaf6239b6"),
                            IsDeleted = false,
                            Name = "Пользователь"
                        },
                        new
                        {
                            Id = new Guid("dd25a17b-19e1-4fac-b346-44fc1911809c"),
                            IsDeleted = false,
                            Name = "Начальник подразделения"
                        },
                        new
                        {
                            Id = new Guid("1277bae3-0814-47a7-8fd5-2506c1fae2de"),
                            IsDeleted = false,
                            Name = "Начальник склада"
                        },
                        new
                        {
                            Id = new Guid("5cfe2a2d-d7bb-4d7c-8a3a-4f6dc585c0ec"),
                            IsDeleted = false,
                            Name = "Бухгалтер"
                        });
                });

            modelBuilder.Entity("AuthService.Domain.Enities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CompanyId")
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("WarehouseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AuthService.Domain.Enities.UserRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("AuthService.Domain.Enities.Company", b =>
                {
                    b.HasOne("AuthService.Domain.Enities.User", null)
                        .WithMany()
                        .HasForeignKey("ResponsibleUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthService.Domain.Enities.User", b =>
                {
                    b.HasOne("AuthService.Domain.Enities.Company", null)
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("AuthService.Domain.Enities.UserRole", b =>
                {
                    b.HasOne("AuthService.Domain.Enities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthService.Domain.Enities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
