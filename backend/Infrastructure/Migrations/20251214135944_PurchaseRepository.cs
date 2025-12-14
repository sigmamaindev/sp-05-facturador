using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PurchaseRepository : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Bodega_BodegaId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_CompraDetalle_Impuesto_ImpuestoId",
                table: "CompraDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_Kardex_Empresa_BusinessId",
                table: "Kardex");

            migrationBuilder.DropForeignKey(
                name: "FK_Kardex_Factura_InvoiceId",
                table: "Kardex");

            migrationBuilder.DropIndex(
                name: "IX_Kardex_InvoiceId",
                table: "Kardex");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Kardex");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Kardex",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Kardex_BusinessId",
                table: "Kardex",
                newName: "IX_Kardex_EmpresaId");

            migrationBuilder.RenameColumn(
                name: "BodegaId",
                table: "Compra",
                newName: "EmissionPointId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_BodegaId",
                table: "Compra",
                newName: "IX_Compra_EmissionPointId");

            migrationBuilder.AlterColumn<int>(
                name: "ImpuestoId",
                table: "CompraDetalle",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "CompraDetalle",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalBase",
                table: "Compra",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDescuento",
                table: "Compra",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_PuntoEmision_EmissionPointId",
                table: "Compra",
                column: "EmissionPointId",
                principalTable: "PuntoEmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompraDetalle_Impuesto_ImpuestoId",
                table: "CompraDetalle",
                column: "ImpuestoId",
                principalTable: "Impuesto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Kardex_Empresa_EmpresaId",
                table: "Kardex",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_PuntoEmision_EmissionPointId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_CompraDetalle_Impuesto_ImpuestoId",
                table: "CompraDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_Kardex_Empresa_EmpresaId",
                table: "Kardex");

            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "CompraDetalle");

            migrationBuilder.DropColumn(
                name: "SubtotalBase",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "TotalDescuento",
                table: "Compra");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "Kardex",
                newName: "BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Kardex_EmpresaId",
                table: "Kardex",
                newName: "IX_Kardex_BusinessId");

            migrationBuilder.RenameColumn(
                name: "EmissionPointId",
                table: "Compra",
                newName: "BodegaId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_EmissionPointId",
                table: "Compra",
                newName: "IX_Compra_BodegaId");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Kardex",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ImpuestoId",
                table: "CompraDetalle",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Kardex_InvoiceId",
                table: "Kardex",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Bodega_BodegaId",
                table: "Compra",
                column: "BodegaId",
                principalTable: "Bodega",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompraDetalle_Impuesto_ImpuestoId",
                table: "CompraDetalle",
                column: "ImpuestoId",
                principalTable: "Impuesto",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Kardex_Empresa_BusinessId",
                table: "Kardex",
                column: "BusinessId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Kardex_Factura_InvoiceId",
                table: "Kardex",
                column: "InvoiceId",
                principalTable: "Factura",
                principalColumn: "Id");
        }
    }
}
