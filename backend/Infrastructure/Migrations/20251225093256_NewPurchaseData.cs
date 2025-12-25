using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewPurchaseData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Empresa_EmpresaId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Establecimiento_EstablecimientoId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_Compra_PuntoEmision_PuntoEmisionId",
                table: "Compra");

            migrationBuilder.DropIndex(
                name: "IX_Compra_PuntoEmisionId",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "FechaCompra",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "PuntoEmisionId",
                table: "Compra");

            migrationBuilder.RenameColumn(
                name: "EstablecimientoId",
                table: "Compra",
                newName: "EstablishmentId");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "Compra",
                newName: "BusinessId");

            migrationBuilder.RenameColumn(
                name: "Referencia",
                table: "Compra",
                newName: "TipoRecibo");

            migrationBuilder.RenameColumn(
                name: "NumeroDocumento",
                table: "Compra",
                newName: "TipoIdSujetoRetenido");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_EstablecimientoId",
                table: "Compra",
                newName: "IX_Compra_EstablishmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_EmpresaId",
                table: "Compra",
                newName: "IX_Compra_BusinessId");

            migrationBuilder.AddColumn<decimal>(
                name: "PesoBruto",
                table: "CompraDetalle",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoNeto",
                table: "CompraDetalle",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UnitMeasureId",
                table: "CompraDetalle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "EstablishmentId",
                table: "Compra",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Ambiente",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClaveAcceso",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContribuyenteEspecial",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEstablecimiento",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionMatriz",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Documento",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentoSujetoRetenido",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Electronico",
                table: "Compra",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Establecimiento",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAutorizacion",
                table: "Compra",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEmision",
                table: "Compra",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NombreComercial",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroAutorizacion",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObligadoContabilidad",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartidoRelacionado",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PeriodoFiscal",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PuntoEmision",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RazSocSujetoRetenido",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Secuencial",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoEmision",
                table: "Compra",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoSujetoRetenido",
                table: "Compra",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalle_UnitMeasureId",
                table: "CompraDetalle",
                column: "UnitMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Empresa_BusinessId",
                table: "Compra",
                column: "BusinessId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Establecimiento_EstablishmentId",
                table: "Compra",
                column: "EstablishmentId",
                principalTable: "Establecimiento",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompraDetalle_UnidadMedida_UnitMeasureId",
                table: "CompraDetalle",
                column: "UnitMeasureId",
                principalTable: "UnidadMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Empresa_BusinessId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Establecimiento_EstablishmentId",
                table: "Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_CompraDetalle_UnidadMedida_UnitMeasureId",
                table: "CompraDetalle");

            migrationBuilder.DropIndex(
                name: "IX_CompraDetalle_UnitMeasureId",
                table: "CompraDetalle");

            migrationBuilder.DropColumn(
                name: "PesoBruto",
                table: "CompraDetalle");

            migrationBuilder.DropColumn(
                name: "PesoNeto",
                table: "CompraDetalle");

            migrationBuilder.DropColumn(
                name: "UnitMeasureId",
                table: "CompraDetalle");

            migrationBuilder.DropColumn(
                name: "Ambiente",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "ClaveAcceso",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "ContribuyenteEspecial",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "DireccionEstablecimiento",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "DireccionMatriz",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "Documento",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "DocumentoSujetoRetenido",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "Electronico",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "Establecimiento",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "FechaAutorizacion",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "FechaEmision",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "NombreComercial",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "NumeroAutorizacion",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "ObligadoContabilidad",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "PartidoRelacionado",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "PeriodoFiscal",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "PuntoEmision",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "RazSocSujetoRetenido",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "Secuencial",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "TipoEmision",
                table: "Compra");

            migrationBuilder.DropColumn(
                name: "TipoSujetoRetenido",
                table: "Compra");

            migrationBuilder.RenameColumn(
                name: "EstablishmentId",
                table: "Compra",
                newName: "EstablecimientoId");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Compra",
                newName: "EmpresaId");

            migrationBuilder.RenameColumn(
                name: "TipoRecibo",
                table: "Compra",
                newName: "Referencia");

            migrationBuilder.RenameColumn(
                name: "TipoIdSujetoRetenido",
                table: "Compra",
                newName: "NumeroDocumento");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_EstablishmentId",
                table: "Compra",
                newName: "IX_Compra_EstablecimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_Compra_BusinessId",
                table: "Compra",
                newName: "IX_Compra_EmpresaId");

            migrationBuilder.AlterColumn<int>(
                name: "EstablecimientoId",
                table: "Compra",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompra",
                table: "Compra",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PuntoEmisionId",
                table: "Compra",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Compra_PuntoEmisionId",
                table: "Compra",
                column: "PuntoEmisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Empresa_EmpresaId",
                table: "Compra",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Establecimiento_EstablecimientoId",
                table: "Compra",
                column: "EstablecimientoId",
                principalTable: "Establecimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_PuntoEmision_PuntoEmisionId",
                table: "Compra",
                column: "PuntoEmisionId",
                principalTable: "PuntoEmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
