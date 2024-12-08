using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updated_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoriesItemsWarehouses_InventoryItems_InventoryItemId",
                table: "InventoriesItemsWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoriesItemsWarehouses_Warehouses_WarehouseId",
                table: "InventoriesItemsWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_InventoriesItemsWarehouses_InventoryItemId",
                table: "InventoriesItemsWarehouses");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "InventoriesItemsWarehouses");

            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "InventoriesItemsWarehouses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_InventoriesItemsWarehouses_ItemId",
                table: "InventoriesItemsWarehouses",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoriesItemsWarehouses_InventoryItems_ItemId",
                table: "InventoriesItemsWarehouses",
                column: "ItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoriesItemsWarehouses_Warehouses_WarehouseId",
                table: "InventoriesItemsWarehouses",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoriesItemsWarehouses_InventoryItems_ItemId",
                table: "InventoriesItemsWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoriesItemsWarehouses_Warehouses_WarehouseId",
                table: "InventoriesItemsWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_InventoriesItemsWarehouses_ItemId",
                table: "InventoriesItemsWarehouses");

            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "InventoriesItemsWarehouses",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                table: "InventoriesItemsWarehouses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_InventoriesItemsWarehouses_InventoryItemId",
                table: "InventoriesItemsWarehouses",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoriesItemsWarehouses_InventoryItems_InventoryItemId",
                table: "InventoriesItemsWarehouses",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoriesItemsWarehouses_Warehouses_WarehouseId",
                table: "InventoriesItemsWarehouses",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
