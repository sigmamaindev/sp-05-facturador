using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BusinessName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NegocioEstablecimiento_Empresa_NegocioId",
                table: "NegocioEstablecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_NegocioEstablecimiento_Establecimiento_EstablecimientoId",
                table: "NegocioEstablecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioNegocio_Empresa_NegocioId",
                table: "UsuarioNegocio");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioNegocio_Usuario_UsuarioId",
                table: "UsuarioNegocio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioNegocio",
                table: "UsuarioNegocio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NegocioEstablecimiento",
                table: "NegocioEstablecimiento");

            migrationBuilder.RenameTable(
                name: "UsuarioNegocio",
                newName: "UsuarioEmpresa");

            migrationBuilder.RenameTable(
                name: "NegocioEstablecimiento",
                newName: "EmpresaEstablecimiento");

            migrationBuilder.RenameColumn(
                name: "NegocioId",
                table: "UsuarioEmpresa",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioNegocio_UsuarioId",
                table: "UsuarioEmpresa",
                newName: "IX_UsuarioEmpresa_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioNegocio_NegocioId",
                table: "UsuarioEmpresa",
                newName: "IX_UsuarioEmpresa_EmpresaId");

            migrationBuilder.RenameColumn(
                name: "NegocioId",
                table: "EmpresaEstablecimiento",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_NegocioEstablecimiento_NegocioId",
                table: "EmpresaEstablecimiento",
                newName: "IX_EmpresaEstablecimiento_EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_NegocioEstablecimiento_EstablecimientoId",
                table: "EmpresaEstablecimiento",
                newName: "IX_EmpresaEstablecimiento_EstablecimientoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioEmpresa",
                table: "UsuarioEmpresa",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpresaEstablecimiento",
                table: "EmpresaEstablecimiento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaEstablecimiento_Empresa_EmpresaId",
                table: "EmpresaEstablecimiento",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaEstablecimiento_Establecimiento_EstablecimientoId",
                table: "EmpresaEstablecimiento",
                column: "EstablecimientoId",
                principalTable: "Establecimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEmpresa_Empresa_EmpresaId",
                table: "UsuarioEmpresa",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioEmpresa_Usuario_UsuarioId",
                table: "UsuarioEmpresa",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaEstablecimiento_Empresa_EmpresaId",
                table: "EmpresaEstablecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaEstablecimiento_Establecimiento_EstablecimientoId",
                table: "EmpresaEstablecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEmpresa_Empresa_EmpresaId",
                table: "UsuarioEmpresa");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEmpresa_Usuario_UsuarioId",
                table: "UsuarioEmpresa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioEmpresa",
                table: "UsuarioEmpresa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpresaEstablecimiento",
                table: "EmpresaEstablecimiento");

            migrationBuilder.RenameTable(
                name: "UsuarioEmpresa",
                newName: "UsuarioNegocio");

            migrationBuilder.RenameTable(
                name: "EmpresaEstablecimiento",
                newName: "NegocioEstablecimiento");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "UsuarioNegocio",
                newName: "NegocioId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioEmpresa_UsuarioId",
                table: "UsuarioNegocio",
                newName: "IX_UsuarioNegocio_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioEmpresa_EmpresaId",
                table: "UsuarioNegocio",
                newName: "IX_UsuarioNegocio_NegocioId");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "NegocioEstablecimiento",
                newName: "NegocioId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpresaEstablecimiento_EstablecimientoId",
                table: "NegocioEstablecimiento",
                newName: "IX_NegocioEstablecimiento_EstablecimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpresaEstablecimiento_EmpresaId",
                table: "NegocioEstablecimiento",
                newName: "IX_NegocioEstablecimiento_NegocioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioNegocio",
                table: "UsuarioNegocio",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NegocioEstablecimiento",
                table: "NegocioEstablecimiento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NegocioEstablecimiento_Empresa_NegocioId",
                table: "NegocioEstablecimiento",
                column: "NegocioId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NegocioEstablecimiento_Establecimiento_EstablecimientoId",
                table: "NegocioEstablecimiento",
                column: "EstablecimientoId",
                principalTable: "Establecimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioNegocio_Empresa_NegocioId",
                table: "UsuarioNegocio",
                column: "NegocioId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioNegocio_Usuario_UsuarioId",
                table: "UsuarioNegocio",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
