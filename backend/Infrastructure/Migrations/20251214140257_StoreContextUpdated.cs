using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StoreContextUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_PuntoEmision_EmissionPointId",
                table: "Compra");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Proveedor",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "EmissionPointId",
                table: "Compra",
                newName: "PuntoEmisionId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_EmissionPointId",
                table: "Compra",
                newName: "IX_Compra_PuntoEmisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_PuntoEmision_PuntoEmisionId",
                table: "Compra",
                column: "PuntoEmisionId",
                principalTable: "PuntoEmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_PuntoEmision_PuntoEmisionId",
                table: "Compra");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Proveedor",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "PuntoEmisionId",
                table: "Compra",
                newName: "EmissionPointId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_PuntoEmisionId",
                table: "Compra",
                newName: "IX_Compra_EmissionPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_PuntoEmision_EmissionPointId",
                table: "Compra",
                column: "EmissionPointId",
                principalTable: "PuntoEmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
