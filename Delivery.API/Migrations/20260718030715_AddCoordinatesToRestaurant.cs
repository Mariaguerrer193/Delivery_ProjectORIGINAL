using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinatesToRestaurant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Restaurant_Latitude",
                table: "Persons",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Restaurant_Longitude",
                table: "Persons",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Restaurant_Latitude",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Restaurant_Longitude",
                table: "Persons");
        }
    }
}
