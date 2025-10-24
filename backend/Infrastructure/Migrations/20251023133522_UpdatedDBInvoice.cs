using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDBInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacturaDetalle_ProductoTipo_TipoProductoId",
                table: "FacturaDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_ProductoTipo_TipoProductoId",
                table: "Producto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoTipo",
                table: "ProductoTipo");

            migrationBuilder.RenameTable(
                name: "ProductoTipo",
                newName: "ProductType");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "ProductType",
                newName: "Type");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductType",
                table: "ProductType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacturaDetalle_ProductType_TipoProductoId",
                table: "FacturaDetalle",
                column: "TipoProductoId",
                principalTable: "ProductType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_ProductType_TipoProductoId",
                table: "Producto",
                column: "TipoProductoId",
                principalTable: "ProductType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacturaDetalle_ProductType_TipoProductoId",
                table: "FacturaDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_ProductType_TipoProductoId",
                table: "Producto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductType",
                table: "ProductType");

            migrationBuilder.RenameTable(
                name: "ProductType",
                newName: "ProductoTipo");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ProductoTipo",
                newName: "Tipo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoTipo",
                table: "ProductoTipo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacturaDetalle_ProductoTipo_TipoProductoId",
                table: "FacturaDetalle",
                column: "TipoProductoId",
                principalTable: "ProductoTipo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_ProductoTipo_TipoProductoId",
                table: "Producto",
                column: "TipoProductoId",
                principalTable: "ProductoTipo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
