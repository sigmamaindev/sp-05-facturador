using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_TipoDocumento_TipoDocumentoId",
                table: "Cliente");

            migrationBuilder.DropTable(
                name: "FacturaDetalle");

            migrationBuilder.DropTable(
                name: "FacturaImpuestoTotal");

            migrationBuilder.DropTable(
                name: "Factura");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoDocumento",
                table: "TipoDocumento");

            migrationBuilder.RenameTable(
                name: "TipoDocumento",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "DocumentType",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "DocumentType",
                newName: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentType",
                table: "DocumentType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_DocumentType_TipoDocumentoId",
                table: "Cliente",
                column: "TipoDocumentoId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_DocumentType_TipoDocumentoId",
                table: "Cliente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentType",
                table: "DocumentType");

            migrationBuilder.RenameTable(
                name: "DocumentType",
                newName: "TipoDocumento");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TipoDocumento",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "TipoDocumento",
                newName: "Codigo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoDocumento",
                table: "TipoDocumento",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Factura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    PuntoEmisionId = table.Column<int>(type: "integer", nullable: false),
                    EstablecimientoId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    TotalDescuento = table.Column<decimal>(type: "numeric", nullable: false),
                    FechaFactura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sequencial = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalFactura = table.Column<decimal>(type: "numeric", nullable: false)
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
                    FacturaId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorIva = table.Column<decimal>(type: "numeric", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TasaImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric", nullable: false)
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
                    FacturaId = table.Column<int>(type: "integer", nullable: false),
                    CodigoImpuesto = table.Column<string>(type: "text", nullable: false),
                    TasaImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorImpuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    BaseImponible = table.Column<decimal>(type: "numeric", nullable: false)
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
                name: "IX_FacturaImpuestoTotal_FacturaId",
                table: "FacturaImpuestoTotal",
                column: "FacturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_TipoDocumento_TipoDocumentoId",
                table: "Cliente",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
