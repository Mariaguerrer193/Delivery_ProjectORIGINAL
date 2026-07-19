using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Delivery.API.Migrations
{
    /// <inheritdoc />
    public partial class Restaurdelive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Restaurants_RestaurantId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropColumn(
                name: "Plate",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "Vehicle",
                table: "Persons",
                newName: "Category");

            migrationBuilder.AddColumn<string>(
                name: "CommercialName",
                table: "Persons",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Persons",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "Persons",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restaurant_IsApproved",
                table: "Persons",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "Persons",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_VehicleId",
                table: "Persons",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Vehicles_VehicleId",
                table: "Persons",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Persons_RestaurantId",
                table: "Products",
                column: "RestaurantId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Vehicles_VehicleId",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Persons_RestaurantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Persons_VehicleId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CommercialName",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Restaurant_IsApproved",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Persons",
                newName: "Vehicle");

            migrationBuilder.AddColumn<string>(
                name: "Plate",
                table: "Persons",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CommercialName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Restaurants_RestaurantId",
                table: "Products",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
