using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated_inventoryitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Documents_DocumentId",
                table: "InventoryItems");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Documents_DocumentId",
                table: "InventoryItems",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Documents_DocumentId",
                table: "InventoryItems");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Documents_DocumentId",
                table: "InventoryItems",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
