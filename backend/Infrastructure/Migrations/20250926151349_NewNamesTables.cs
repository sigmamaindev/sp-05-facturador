using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewNamesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuariosRoles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Roles",
                newName: "NomRol");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocUsu = table.Column<string>(type: "text", nullable: false),
                    NomComUsu = table.Column<string>(type: "text", nullable: false),
                    EmaUsu = table.Column<string>(type: "text", nullable: false),
                    NomUsu = table.Column<string>(type: "text", nullable: false),
                    ClaUsu = table.Column<string>(type: "text", nullable: true),
                    DirUsu = table.Column<string>(type: "text", nullable: true),
                    CelUsu = table.Column<string>(type: "text", nullable: true),
                    TelUsu = table.Column<string>(type: "text", nullable: true),
                    ImaUrlUsu = table.Column<string>(type: "text", nullable: true),
                    RazSocUsu = table.Column<string>(type: "text", nullable: true),
                    UsuAct = table.Column<bool>(type: "boolean", nullable: false),
                    FecCreUsu = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FecActUsu = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoles_RoleId",
                table: "UsersRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRoles_UserId",
                table: "UsersRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "NomRol",
                table: "Roles",
                newName: "Nombre");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    Clave = table.Column<string>(type: "text", nullable: true),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Documento = table.Column<string>(type: "text", nullable: false),
                    FechaActualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCreado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    NombreUsuario = table.Column<string>(type: "text", nullable: false),
                    RazonSocial = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_RolId",
                table: "UsuariosRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_UsuarioId",
                table: "UsuariosRoles",
                column: "UsuarioId");
        }
    }
}
