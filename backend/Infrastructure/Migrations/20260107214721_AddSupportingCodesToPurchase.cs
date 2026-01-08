using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportingCodesToPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoDocumentoSustento",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoSustento",
                table: "Compra",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoDocumentoSustento",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "CodigoSustento",
                table: "Compra");
        }
    }
}
