using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WriteOffService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    FileData = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffReasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    ReasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WriteOffRequests_WriteOffReasons_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "WriteOffReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WriteOffActs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WriteOffRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteOffActs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WriteOffActs_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WriteOffActs_WriteOffRequests_WriteOffRequestId",
                        column: x => x.WriteOffRequestId,
                        principalTable: "WriteOffRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffActs_DocumentId",
                table: "WriteOffActs",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffActs_WriteOffRequestId",
                table: "WriteOffActs",
                column: "WriteOffRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_ItemId",
                table: "WriteOffRequests",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_ReasonId",
                table: "WriteOffRequests",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_Status",
                table: "WriteOffRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WriteOffRequests_WarehouseId",
                table: "WriteOffRequests",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WriteOffActs");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "WriteOffRequests");

            migrationBuilder.DropTable(
                name: "WriteOffReasons");
        }
    }
}
