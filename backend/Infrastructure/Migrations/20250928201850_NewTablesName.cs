using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewTablesName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Roles_RolId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Users_UsuId",
                table: "UsersRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersRoles",
                table: "UsersRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "UsersRoles",
                newName: "UsuarioRol");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Usuario");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Rol");

            migrationBuilder.RenameColumn(
                name: "UsuId",
                table: "UsuarioRol",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_UsuId",
                table: "UsuarioRol",
                newName: "IX_UsuarioRol_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_RolId",
                table: "UsuarioRol",
                newName: "IX_UsuarioRol_RolId");

            migrationBuilder.RenameColumn(
                name: "UsuAct",
                table: "Usuario",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "TelUsu",
                table: "Usuario",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "RazSocUsu",
                table: "Usuario",
                newName: "RazonSocial");

            migrationBuilder.RenameColumn(
                name: "NomUsu",
                table: "Usuario",
                newName: "NombreUsuario");

            migrationBuilder.RenameColumn(
                name: "NomComUsu",
                table: "Usuario",
                newName: "NombreCompleto");

            migrationBuilder.RenameColumn(
                name: "ImaUrlUsu",
                table: "Usuario",
                newName: "ImagenUrl");

            migrationBuilder.RenameColumn(
                name: "FecCreUsu",
                table: "Usuario",
                newName: "FechaCreado");

            migrationBuilder.RenameColumn(
                name: "FecActUsu",
                table: "Usuario",
                newName: "FechaActualizado");

            migrationBuilder.RenameColumn(
                name: "EmaUsu",
                table: "Usuario",
                newName: "Documento");

            migrationBuilder.RenameColumn(
                name: "DocUsu",
                table: "Usuario",
                newName: "Correo");

            migrationBuilder.RenameColumn(
                name: "DirUsu",
                table: "Usuario",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "ClaUsu",
                table: "Usuario",
                newName: "Celular");

            migrationBuilder.RenameColumn(
                name: "CelUsu",
                table: "Usuario",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "NomRol",
                table: "Rol",
                newName: "Nombre");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioRol",
                table: "UsuarioRol",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rol",
                table: "Rol",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Empresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Documento = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Provincia = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Establecimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establecimiento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioNegocio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    NegocioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioNegocio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioNegocio_Empresa_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioNegocio_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NegocioEstablecimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NegocioId = table.Column<int>(type: "integer", nullable: false),
                    EstablecimientoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NegocioEstablecimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NegocioEstablecimiento_Empresa_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NegocioEstablecimiento_Establecimiento_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioEstablecimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EstablecimientoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEstablecimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioEstablecimiento_Establecimiento_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioEstablecimiento_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NegocioEstablecimiento_EstablecimientoId",
                table: "NegocioEstablecimiento",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_NegocioEstablecimiento_NegocioId",
                table: "NegocioEstablecimiento",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstablecimiento_EstablecimientoId",
                table: "UsuarioEstablecimiento",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstablecimiento_UsuarioId",
                table: "UsuarioEstablecimiento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioNegocio_NegocioId",
                table: "UsuarioNegocio",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioNegocio_UsuarioId",
                table: "UsuarioNegocio",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRol_Rol_RolId",
                table: "UsuarioRol",
                column: "RolId",
                principalTable: "Rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRol_Usuario_UsuarioId",
                table: "UsuarioRol",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRol_Rol_RolId",
                table: "UsuarioRol");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRol_Usuario_UsuarioId",
                table: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "NegocioEstablecimiento");

            migrationBuilder.DropTable(
                name: "UsuarioEstablecimiento");

            migrationBuilder.DropTable(
                name: "UsuarioNegocio");

            migrationBuilder.DropTable(
                name: "Establecimiento");

            migrationBuilder.DropTable(
                name: "Empresa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioRol",
                table: "UsuarioRol");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rol",
                table: "Rol");

            migrationBuilder.RenameTable(
                name: "UsuarioRol",
                newName: "UsersRoles");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Rol",
                newName: "Roles");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "UsersRoles",
                newName: "UsuId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioRol_UsuarioId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_UsuId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioRol_RolId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_RolId");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Users",
                newName: "TelUsu");

            migrationBuilder.RenameColumn(
                name: "RazonSocial",
                table: "Users",
                newName: "RazSocUsu");

            migrationBuilder.RenameColumn(
                name: "NombreUsuario",
                table: "Users",
                newName: "NomUsu");

            migrationBuilder.RenameColumn(
                name: "NombreCompleto",
                table: "Users",
                newName: "NomComUsu");

            migrationBuilder.RenameColumn(
                name: "ImagenUrl",
                table: "Users",
                newName: "ImaUrlUsu");

            migrationBuilder.RenameColumn(
                name: "FechaCreado",
                table: "Users",
                newName: "FecCreUsu");

            migrationBuilder.RenameColumn(
                name: "FechaActualizado",
                table: "Users",
                newName: "FecActUsu");

            migrationBuilder.RenameColumn(
                name: "Documento",
                table: "Users",
                newName: "EmaUsu");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Users",
                newName: "DirUsu");

            migrationBuilder.RenameColumn(
                name: "Correo",
                table: "Users",
                newName: "DocUsu");

            migrationBuilder.RenameColumn(
                name: "Celular",
                table: "Users",
                newName: "ClaUsu");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Users",
                newName: "CelUsu");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Users",
                newName: "UsuAct");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Roles",
                newName: "NomRol");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersRoles",
                table: "UsersRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Roles_RolId",
                table: "UsersRoles",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Users_UsuId",
                table: "UsersRoles",
                column: "UsuId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
