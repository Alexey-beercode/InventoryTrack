using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updated_user_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("101687c4-4c76-4b52-bce4-fcd062622677"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("65704147-c1d0-4743-a6f5-714d843337e4"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6cb3c815-ecff-45fb-b425-5d711afc8de3"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e6cb6e64-33f6-4ba1-9542-4eca61a3f025"));

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("1277bae3-0814-47a7-8fd5-2506c1fae2de"), false, "Начальник склада" },
                    { new Guid("5cfe2a2d-d7bb-4d7c-8a3a-4f6dc585c0ec"), false, "Бухгалтер" },
                    { new Guid("ac5e0c16-3823-4e6f-9efb-aadaaf6239b6"), false, "Пользователь" },
                    { new Guid("dd25a17b-19e1-4fac-b346-44fc1911809c"), false, "Начальник подразделения" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1277bae3-0814-47a7-8fd5-2506c1fae2de"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("5cfe2a2d-d7bb-4d7c-8a3a-4f6dc585c0ec"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ac5e0c16-3823-4e6f-9efb-aadaaf6239b6"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("dd25a17b-19e1-4fac-b346-44fc1911809c"));

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("101687c4-4c76-4b52-bce4-fcd062622677"), false, "Начальник склада" },
                    { new Guid("65704147-c1d0-4743-a6f5-714d843337e4"), false, "Бухгалтер" },
                    { new Guid("6cb3c815-ecff-45fb-b425-5d711afc8de3"), false, "Пользователь" },
                    { new Guid("e6cb6e64-33f6-4ba1-9542-4eca61a3f025"), false, "Начальник подразделения" }
                });
        }
    }
}
