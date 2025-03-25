using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WriteOffService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_documents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("0ee65a30-f969-4903-ba74-03d6924ccb98"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("56501935-b306-4287-9e64-e2bf6651a15b"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("900151c2-80ee-4512-8fe1-1119707890a7"));

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "WriteOffRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "WriteOffReasons",
                columns: new[] { "Id", "IsDeleted", "Reason" },
                values: new object[,]
                {
                    { new Guid("5e90b547-a2d1-436e-a3be-0b9289e71960"), false, "Поломка" },
                    { new Guid("63707ba8-8dce-4b33-959e-891141d63a77"), false, "По причине продажи" },
                    { new Guid("7c019be0-d26d-4f50-8531-33a1bb73256b"), false, "Другое" },
                    { new Guid("e70a3815-0c42-4ec2-baf5-58c6ee2f11eb"), false, "Истёк срок годности" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("5e90b547-a2d1-436e-a3be-0b9289e71960"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("63707ba8-8dce-4b33-959e-891141d63a77"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("7c019be0-d26d-4f50-8531-33a1bb73256b"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("e70a3815-0c42-4ec2-baf5-58c6ee2f11eb"));

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "WriteOffRequests");

            migrationBuilder.InsertData(
                table: "WriteOffReasons",
                columns: new[] { "Id", "IsDeleted", "Reason" },
                values: new object[,]
                {
                    { new Guid("0ee65a30-f969-4903-ba74-03d6924ccb98"), false, "Поломка" },
                    { new Guid("56501935-b306-4287-9e64-e2bf6651a15b"), false, "Истёк срок годности" },
                    { new Guid("900151c2-80ee-4512-8fe1-1119707890a7"), false, "По причине продажи" }
                });
        }
    }
}
