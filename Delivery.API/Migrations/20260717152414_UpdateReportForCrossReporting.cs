using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportForCrossReporting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Persons_ClientId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Reports",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "SourcePersonId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SourcePersonId",
                table: "Reports",
                column: "SourcePersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Persons_ClientId",
                table: "Reports",
                column: "ClientId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Persons_SourcePersonId",
                table: "Reports",
                column: "SourcePersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Persons_ClientId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Persons_SourcePersonId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_SourcePersonId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "SourcePersonId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Persons_ClientId",
                table: "Reports",
                column: "ClientId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
