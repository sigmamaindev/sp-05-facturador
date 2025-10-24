using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Invoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Factura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sequencial = table.Column<string>(type: "text", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDescuento = table.Column<decimal>(type: "numeric", nullable: false),
                    Totaliactura = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FechaFactura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    EstablecimientoId = table.Column<int>(type: "integer", nullable: false),
                    PuntoEmisionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factura_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Factura_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Factura_Establecimiento_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Factura_PuntoEmision_PuntoEmisionId",
                        column: x => x.PuntoEmisionId,
                        principalTable: "PuntoEmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturaDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorIva = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    TasaImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    TipoProductoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    FacturaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaDetalle_Factura_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Factura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaDetalle_ProductoTipo_TipoProductoId",
                        column: x => x.TipoProductoId,
                        principalTable: "ProductoTipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturaImpuestoTotal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TasaImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    BaseImponible = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    CodigoImpuesto = table.Column<string>(type: "text", nullable: false),
                    FacturaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaImpuestoTotal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaImpuestoTotal_Factura_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Factura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Factura_ClienteId",
                table: "Factura",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_EmpresaId",
                table: "Factura",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_EstablecimientoId",
                table: "Factura",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_PuntoEmisionId",
                table: "Factura",
                column: "PuntoEmisionId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_FacturaId",
                table: "FacturaDetalle",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_ProductoId",
                table: "FacturaDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_TipoProductoId",
                table: "FacturaDetalle",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaImpuestoTotal_FacturaId",
                table: "FacturaImpuestoTotal",
                column: "FacturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacturaDetalle");

            migrationBuilder.DropTable(
                name: "FacturaImpuestoTotal");

            migrationBuilder.DropTable(
                name: "Factura");
        }
    }
}
