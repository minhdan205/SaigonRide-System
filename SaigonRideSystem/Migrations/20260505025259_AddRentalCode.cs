using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRideSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RentalCode",
                table: "Rentals",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_RentalCode",
                table: "Rentals",
                column: "RentalCode",
                unique: true,
                filter: "[RentalCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rentals_RentalCode",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "RentalCode",
                table: "Rentals");
        }
    }
}
