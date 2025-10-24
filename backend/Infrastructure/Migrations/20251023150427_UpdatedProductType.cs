using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProductType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacturaDetalle_ProductType_TipoProductoId",
                table: "FacturaDetalle");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_ProductType_TipoProductoId",
                table: "Producto");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.DropIndex(
                name: "IX_Producto_TipoProductoId",
                table: "Producto");

            migrationBuilder.DropIndex(
                name: "IX_FacturaDetalle_TipoProductoId",
                table: "FacturaDetalle");

            migrationBuilder.DropColumn(
                name: "TipoProductoId",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "TipoProductoId",
                table: "FacturaDetalle");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Producto",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Grupo",
                table: "Impuesto",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Factura",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreado",
                table: "Cliente",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Cliente",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "FechaCreado",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Cliente");

            migrationBuilder.AddColumn<int>(
                name: "TipoProductoId",
                table: "Producto",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Grupo",
                table: "Impuesto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "TipoProductoId",
                table: "FacturaDetalle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Factura",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Producto_TipoProductoId",
                table: "Producto",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_TipoProductoId",
                table: "FacturaDetalle",
                column: "TipoProductoId");

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
    }
}
