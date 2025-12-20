using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedInvoiceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnidadMedidaId",
                table: "FacturaDetalle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_UnidadMedidaId",
                table: "FacturaDetalle",
                column: "UnidadMedidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacturaDetalle_UnidadMedida_UnidadMedidaId",
                table: "FacturaDetalle",
                column: "UnidadMedidaId",
                principalTable: "UnidadMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacturaDetalle_UnidadMedida_UnidadMedidaId",
                table: "FacturaDetalle");

            migrationBuilder.DropIndex(
                name: "IX_FacturaDetalle_UnidadMedidaId",
                table: "FacturaDetalle");

            migrationBuilder.DropColumn(
                name: "UnidadMedidaId",
                table: "FacturaDetalle");
        }
    }
}
