using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePurchaseData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Empresa_BusinessId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_CompraDetalle_UnidadMedida_UnitMeasureId",
                table: "CompraDetalle");

            migrationBuilder.RenameColumn(
                name: "UnitMeasureId",
                table: "CompraDetalle",
                newName: "UnidadMedidaId");

            migrationBuilder.RenameIndex(
                name: "IX_CompraDetalle_UnitMeasureId",
                table: "CompraDetalle",
                newName: "IX_CompraDetalle_UnidadMedidaId");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Compra",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_BusinessId",
                table: "Compra",
                newName: "IX_Compra_EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Empresa_EmpresaId",
                table: "Compra",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompraDetalle_UnidadMedida_UnidadMedidaId",
                table: "CompraDetalle",
                column: "UnidadMedidaId",
                principalTable: "UnidadMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Empresa_EmpresaId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_CompraDetalle_UnidadMedida_UnidadMedidaId",
                table: "CompraDetalle");

            migrationBuilder.RenameColumn(
                name: "UnidadMedidaId",
                table: "CompraDetalle",
                newName: "UnitMeasureId");

            migrationBuilder.RenameIndex(
                name: "IX_CompraDetalle_UnidadMedidaId",
                table: "CompraDetalle",
                newName: "IX_CompraDetalle_UnitMeasureId");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "Compra",
                newName: "BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_EmpresaId",
                table: "Compra",
                newName: "IX_Compra_BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Empresa_BusinessId",
                table: "Compra",
                column: "BusinessId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompraDetalle_UnidadMedida_UnitMeasureId",
                table: "CompraDetalle",
                column: "UnitMeasureId",
                principalTable: "UnidadMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
