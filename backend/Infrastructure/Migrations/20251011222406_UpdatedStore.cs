using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEmissionPoints_PuntoEmision_EmissionPointId",
                table: "UserEmissionPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEmissionPoints_Usuario_UserId",
                table: "UserEmissionPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEstablishments_Establecimiento_EstablishmentId",
                table: "UserEstablishments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEstablishments_Usuario_UserId",
                table: "UserEstablishments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEstablishments",
                table: "UserEstablishments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEmissionPoints",
                table: "UserEmissionPoints");

            migrationBuilder.RenameTable(
                name: "UserEstablishments",
                newName: "UsuarioEstablecimiento");

            migrationBuilder.RenameTable(
                name: "UserEmissionPoints",
                newName: "UsuarioPuntoEmision");

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

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UsuarioPuntoEmision",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "EmissionPointId",
                table: "UsuarioPuntoEmision",
                newName: "PuntoEmisionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserEmissionPoints_UserId",
                table: "UsuarioPuntoEmision",
                newName: "IX_UsuarioPuntoEmision_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_UserEmissionPoints_EmissionPointId",
                table: "UsuarioPuntoEmision",
                newName: "IX_UsuarioPuntoEmision_PuntoEmisionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioEstablecimiento",
                table: "UsuarioEstablecimiento",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioPuntoEmision",
                table: "UsuarioPuntoEmision",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPuntoEmision_PuntoEmision_PuntoEmisionId",
                table: "UsuarioPuntoEmision",
                column: "PuntoEmisionId",
                principalTable: "PuntoEmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioPuntoEmision_Usuario_UsuarioId",
                table: "UsuarioPuntoEmision",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEstablecimiento_Establecimiento_EstablecimientoId",
                table: "UsuarioEstablecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioEstablecimiento_Usuario_UsuarioId",
                table: "UsuarioEstablecimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPuntoEmision_PuntoEmision_PuntoEmisionId",
                table: "UsuarioPuntoEmision");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioPuntoEmision_Usuario_UsuarioId",
                table: "UsuarioPuntoEmision");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioPuntoEmision",
                table: "UsuarioPuntoEmision");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioEstablecimiento",
                table: "UsuarioEstablecimiento");

            migrationBuilder.RenameTable(
                name: "UsuarioPuntoEmision",
                newName: "UserEmissionPoints");

            migrationBuilder.RenameTable(
                name: "UsuarioEstablecimiento",
                newName: "UserEstablishments");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "UserEmissionPoints",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PuntoEmisionId",
                table: "UserEmissionPoints",
                newName: "EmissionPointId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioPuntoEmision_UsuarioId",
                table: "UserEmissionPoints",
                newName: "IX_UserEmissionPoints_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioPuntoEmision_PuntoEmisionId",
                table: "UserEmissionPoints",
                newName: "IX_UserEmissionPoints_EmissionPointId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEmissionPoints",
                table: "UserEmissionPoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEstablishments",
                table: "UserEstablishments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEmissionPoints_PuntoEmision_EmissionPointId",
                table: "UserEmissionPoints",
                column: "EmissionPointId",
                principalTable: "PuntoEmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEmissionPoints_Usuario_UserId",
                table: "UserEmissionPoints",
                column: "UserId",
                principalTable: "Usuario",
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
    }
}
