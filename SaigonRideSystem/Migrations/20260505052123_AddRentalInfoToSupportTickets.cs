using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRideSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalInfoToSupportTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "SupportTickets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentalId",
                table: "SupportTickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleId",
                table: "SupportTickets",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_RentalId",
                table: "SupportTickets",
                column: "RentalId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_Rentals_RentalId",
                table: "SupportTickets",
                column: "RentalId",
                principalTable: "Rentals",
                principalColumn: "RentalId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_Rentals_RentalId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_RentalId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "RentalId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "SupportTickets");
        }
    }
}
