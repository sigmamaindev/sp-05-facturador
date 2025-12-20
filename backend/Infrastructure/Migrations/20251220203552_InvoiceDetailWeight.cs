using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceDetailWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PesoBruto",
                table: "Producto",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoNeto",
                table: "Producto",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoBruto",
                table: "FacturaDetalle",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoNeto",
                table: "FacturaDetalle",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PesoBruto",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "PesoNeto",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "PesoBruto",
                table: "FacturaDetalle");

            migrationBuilder.DropColumn(
                name: "PesoNeto",
                table: "FacturaDetalle");
        }
    }
}
