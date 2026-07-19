using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🎯 PASO DE SEGURIDAD: Esto rescata a todos tus usuarios viejos antes de alterar la columna
            migrationBuilder.Sql("UPDATE \"Persons\" SET \"IsActive\" = true WHERE \"IsActive\" IS NULL;");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Persons",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Persons",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
