using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRideSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppliedDiscountCode",
                table: "Rentals",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CodeDiscountAmount",
                table: "Rentals",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CodeDiscountPercent",
                table: "Rentals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountCodeId",
                table: "Rentals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiscountCodes",
                columns: table => new
                {
                    DiscountCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DiscountPercent = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCodes", x => x.DiscountCodeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_DiscountCodeId",
                table: "Rentals",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCodes_Code",
                table: "DiscountCodes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_DiscountCodes_DiscountCodeId",
                table: "Rentals",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "DiscountCodeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_DiscountCodes_DiscountCodeId",
                table: "Rentals");

            migrationBuilder.DropTable(
                name: "DiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_DiscountCodeId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "AppliedDiscountCode",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "CodeDiscountAmount",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "CodeDiscountPercent",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "DiscountCodeId",
                table: "Rentals");
        }
    }
}
