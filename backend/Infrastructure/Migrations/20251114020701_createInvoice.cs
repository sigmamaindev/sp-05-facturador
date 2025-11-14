using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createInvoice : Migration
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
                    Secuencial = table.Column<string>(type: "text", nullable: false),
                    ClaveAcceso = table.Column<string>(type: "text", nullable: false),
                    Ambiente = table.Column<string>(type: "text", nullable: false),
                    DocumentType = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Electronico = table.Column<bool>(type: "boolean", nullable: false),
                    FechaFactura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaAutorizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    EstablecimientoId = table.Column<int>(type: "integer", nullable: false),
                    PuntoEmisionId = table.Column<int>(type: "integer", nullable: false),
                    SubtotalBase = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDescuento = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    MetodoPago = table.Column<string>(type: "text", nullable: false),
                    DiasPago = table.Column<int>(type: "integer", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    InformacionAdicional = table.Column<string>(type: "text", nullable: true),
                    FirmaXml = table.Column<string>(type: "text", nullable: true),
                    NumeroAutorizacion = table.Column<string>(type: "text", nullable: true),
                    MensajeSri = table.Column<string>(type: "text", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Factura_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturaDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacturaId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    ImpuestoId = table.Column<int>(type: "integer", nullable: false),
                    TasaImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaDetalle_Bodega_BodegaId",
                        column: x => x.BodegaId,
                        principalTable: "Bodega",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaDetalle_Factura_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Factura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaDetalle_Impuesto_ImpuestoId",
                        column: x => x.ImpuestoId,
                        principalTable: "Impuesto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
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
                name: "IX_Factura_UsuarioId",
                table: "Factura",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_BodegaId",
                table: "FacturaDetalle",
                column: "BodegaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_FacturaId",
                table: "FacturaDetalle",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_ImpuestoId",
                table: "FacturaDetalle",
                column: "ImpuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalle_ProductoId",
                table: "FacturaDetalle",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacturaDetalle");

            migrationBuilder.DropTable(
                name: "Factura");
        }
    }
}
