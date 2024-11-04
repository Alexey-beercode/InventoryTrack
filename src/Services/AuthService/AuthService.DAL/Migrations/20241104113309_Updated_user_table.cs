using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Updated_user_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2b1b2203-e4bd-48b0-9008-478c9f214d56"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("32b293fa-2336-4a20-b0bd-cbfaf7d117a9"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f40cf8e0-bc49-4688-8ed9-89c91fd07fb1"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("fad5a4ab-c1f8-43fe-bf3e-f0b07ab4b442"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "Users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("2e3a8e1c-f00b-4f6a-99d0-2d84c150313b"), false, "Accountant" },
                    { new Guid("84299b0e-7ad1-4378-884e-7979c1d56978"), false, "Department Head" },
                    { new Guid("b0a4c4d1-180a-4884-952b-3ebf51616af1"), false, "Resident" },
                    { new Guid("c478504c-142b-4fdc-b592-c3ff7273c1b6"), false, "Warehouse Manager" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2e3a8e1c-f00b-4f6a-99d0-2d84c150313b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("84299b0e-7ad1-4378-884e-7979c1d56978"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b0a4c4d1-180a-4884-952b-3ebf51616af1"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c478504c-142b-4fdc-b592-c3ff7273c1b6"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("2b1b2203-e4bd-48b0-9008-478c9f214d56"), false, "Resident" },
                    { new Guid("32b293fa-2336-4a20-b0bd-cbfaf7d117a9"), false, "Warehouse Manager" },
                    { new Guid("f40cf8e0-bc49-4688-8ed9-89c91fd07fb1"), false, "Accountant" },
                    { new Guid("fad5a4ab-c1f8-43fe-bf3e-f0b07ab4b442"), false, "Department Head" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
