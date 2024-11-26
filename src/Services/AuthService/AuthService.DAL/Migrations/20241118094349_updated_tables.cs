using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updated_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("123a0dd7-1881-4621-8b14-6f31515d4308"), false, "Resident" },
                    { new Guid("372881bd-0bef-4230-a9d4-ddea9b05e08e"), false, "Accountant" },
                    { new Guid("56e4822b-d29e-459f-ab2e-dd3499c2fdf3"), false, "Warehouse Manager" },
                    { new Guid("8697c755-c530-4a2e-b9d6-7744a9fa594f"), false, "Department Head" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("123a0dd7-1881-4621-8b14-6f31515d4308"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("372881bd-0bef-4230-a9d4-ddea9b05e08e"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("56e4822b-d29e-459f-ab2e-dd3499c2fdf3"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8697c755-c530-4a2e-b9d6-7744a9fa594f"));

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
        }
    }
}
