using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_documents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "MovementRequests",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "MovementRequests");
        }
    }
}
