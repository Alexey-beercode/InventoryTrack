using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WriteOffService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingBatchFieldsToWriteOffRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ ДОБАВЛЕНО: Создание колонки BatchNumber
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "WriteOffRequests",
                type: "text",
                nullable: true);

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("2c72028a-354b-4ff5-9062-b8a7aca2e9ac"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("46726104-169a-467c-a806-eb25d1984409"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("79c999bd-36a1-4522-b5ad-0426a4950d75"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("88479026-59f2-4d20-8a49-c642b8d63629"));

            migrationBuilder.InsertData(
                table: "WriteOffReasons",
                columns: new[] { "Id", "IsDeleted", "Reason" },
                values: new object[,]
                {
                    { new Guid("48a65e5e-5854-422b-be75-fba4d793dbd1"), false, "Другое" },
                    { new Guid("58698145-c649-4f9b-8eda-972b0595f5ed"), false, "Поломка" },
                    { new Guid("8153538e-177e-4680-8197-176b4af2777f"), false, "Истёк срок годности" },
                    { new Guid("8769cbc0-9aa1-449d-a441-a0d1eec9b24b"), false, "По причине продажи" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ✅ ДОБАВЛЕНО: Удаление колонки BatchNumber при откате
            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "WriteOffRequests");

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("48a65e5e-5854-422b-be75-fba4d793dbd1"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("58698145-c649-4f9b-8eda-972b0595f5ed"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("8153538e-177e-4680-8197-176b4af2777f"));

            migrationBuilder.DeleteData(
                table: "WriteOffReasons",
                keyColumn: "Id",
                keyValue: new Guid("8769cbc0-9aa1-449d-a441-a0d1eec9b24b"));

            migrationBuilder.InsertData(
                table: "WriteOffReasons",
                columns: new[] { "Id", "IsDeleted", "Reason" },
                values: new object[,]
                {
                    { new Guid("2c72028a-354b-4ff5-9062-b8a7aca2e9ac"), false, "Другое" },
                    { new Guid("46726104-169a-467c-a806-eb25d1984409"), false, "Поломка" },
                    { new Guid("79c999bd-36a1-4522-b5ad-0426a4950d75"), false, "По причине продажи" },
                    { new Guid("88479026-59f2-4d20-8a49-c642b8d63629"), false, "Истёк срок годности" }
                });
        }
    }
}