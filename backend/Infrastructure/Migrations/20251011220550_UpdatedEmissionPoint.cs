using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEmissionPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PuntoEmision_Establecimiento_EstablishmentId",
                table: "PuntoEmision");

            migrationBuilder.RenameColumn(
                name: "EstablishmentId",
                table: "PuntoEmision",
                newName: "EstablecimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_PuntoEmision_EstablishmentId",
                table: "PuntoEmision",
                newName: "IX_PuntoEmision_EstablecimientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PuntoEmision_Establecimiento_EstablecimientoId",
                table: "PuntoEmision",
                column: "EstablecimientoId",
                principalTable: "Establecimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PuntoEmision_Establecimiento_EstablecimientoId",
                table: "PuntoEmision");

            migrationBuilder.RenameColumn(
                name: "EstablecimientoId",
                table: "PuntoEmision",
                newName: "EstablishmentId");

            migrationBuilder.RenameIndex(
                name: "IX_PuntoEmision_EstablecimientoId",
                table: "PuntoEmision",
                newName: "IX_PuntoEmision_EstablishmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PuntoEmision_Establecimiento_EstablishmentId",
                table: "PuntoEmision",
                column: "EstablishmentId",
                principalTable: "Establecimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
