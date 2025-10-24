using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEstablecimiento_Establecimiento_EstablecimientoId",
                table: "UsuarioEstablecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEstablecimiento_Usuario_UsuarioId",
                table: "UsuarioEstablecimiento");

            migrationBuilder.DropTable(
                name: "EmpresaEstablecimiento");

            migrationBuilder.DropTable(
                name: "ProductBodega");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Impuesto");

            migrationBuilder.DropTable(
                name: "UnidadMedida");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioEstablecimiento",
                table: "UsuarioEstablecimiento");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Empresa");

            migrationBuilder.RenameTable(
                name: "UsuarioEstablecimiento",
                newName: "UserEstablishments");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "UserEstablishments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "EstablecimientoId",
                table: "UserEstablishments",
                newName: "EstablishmentId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioEstablecimiento_UsuarioId",
                table: "UserEstablishments",
                newName: "IX_UserEstablishments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioEstablecimiento_EstablecimientoId",
                table: "UserEstablishments",
                newName: "IX_UserEstablishments_EstablishmentId");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Establecimiento",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEstablishments",
                table: "UserEstablishments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PuntoEmision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    EstablishmentId = table.Column<int>(type: "integer", nullable: false),
                    FechaCreado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntoEmision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuntoEmision_Establecimiento_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establecimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEmissionPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EmissionPointId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmissionPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEmissionPoints_PuntoEmision_EmissionPointId",
                        column: x => x.EmissionPointId,
                        principalTable: "PuntoEmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEmissionPoints_Usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Establecimiento_EmpresaId",
                table: "Establecimiento",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PuntoEmision_EstablishmentId",
                table: "PuntoEmision",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmissionPoints_EmissionPointId",
                table: "UserEmissionPoints",
                column: "EmissionPointId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmissionPoints_UserId",
                table: "UserEmissionPoints",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Establecimiento_Empresa_EmpresaId",
                table: "Establecimiento",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEstablishments_Establecimiento_EstablishmentId",
                table: "UserEstablishments",
                column: "EstablishmentId",
                principalTable: "Establecimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEstablishments_Usuario_UserId",
                table: "UserEstablishments",
                column: "UserId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Establecimiento_Empresa_EmpresaId",
                table: "Establecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEstablishments_Establecimiento_EstablishmentId",
                table: "UserEstablishments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEstablishments_Usuario_UserId",
                table: "UserEstablishments");

            migrationBuilder.DropTable(
                name: "UserEmissionPoints");

            migrationBuilder.DropTable(
                name: "PuntoEmision");

            migrationBuilder.DropIndex(
                name: "IX_Establecimiento_EmpresaId",
                table: "Establecimiento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEstablishments",
                table: "UserEstablishments");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Establecimiento");

            migrationBuilder.RenameTable(
                name: "UserEstablishments",
                newName: "UsuarioEstablecimiento");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UsuarioEstablecimiento",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "EstablishmentId",
                table: "UsuarioEstablecimiento",
                newName: "EstablecimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_UserEstablishments_UserId",
                table: "UsuarioEstablecimiento",
                newName: "IX_UsuarioEstablecimiento_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_UserEstablishments_EstablishmentId",
                table: "UsuarioEstablecimiento",
                newName: "IX_UsuarioEstablecimiento_EstablecimientoId");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Empresa",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioEstablecimiento",
                table: "UsuarioEstablecimiento",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EmpresaEstablecimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    EstablecimientoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaEstablecimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaEstablecimiento_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpresaEstablecimiento_Establecimiento_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Impuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impuesto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnidadMedida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FactorBase = table.Column<decimal>(type: "numeric", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadMedida", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ImpuestoId = table.Column<int>(type: "integer", nullable: false),
                    UnidadMedidaId = table.Column<int>(type: "integer", nullable: false),
                    FechaCreado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    TipoProducto = table.Column<string>(type: "text", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    FechaActualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producto_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producto_Impuesto_ImpuestoId",
                        column: x => x.ImpuestoId,
                        principalTable: "Impuesto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producto_UnidadMedida_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalTable: "UnidadMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductBodega",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBodega", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBodega_Bodega_BodegaId",
                        column: x => x.BodegaId,
                        principalTable: "Bodega",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductBodega_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaEstablecimiento_EmpresaId",
                table: "EmpresaEstablecimiento",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaEstablecimiento_EstablecimientoId",
                table: "EmpresaEstablecimiento",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBodega_BodegaId",
                table: "ProductBodega",
                column: "BodegaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBodega_ProductoId",
                table: "ProductBodega",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_EmpresaId",
                table: "Producto",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_ImpuestoId",
                table: "Producto",
                column: "ImpuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_UnidadMedidaId",
                table: "Producto",
                column: "UnidadMedidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEstablecimiento_Establecimiento_EstablecimientoId",
                table: "UsuarioEstablecimiento",
                column: "EstablecimientoId",
                principalTable: "Establecimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEstablecimiento_Usuario_UsuarioId",
                table: "UsuarioEstablecimiento",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
