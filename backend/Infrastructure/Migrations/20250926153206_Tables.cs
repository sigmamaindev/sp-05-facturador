using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UsersRoles",
                newName: "UsuId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "UsersRoles",
                newName: "RolId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_UserId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_UsuId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_RoleId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_RolId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Roles_RolId",
                table: "UsersRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersRoles_Users_UsuId",
                table: "UsersRoles");

            migrationBuilder.RenameColumn(
                name: "UsuId",
                table: "UsersRoles",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "RolId",
                table: "UsersRoles",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_UsuId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersRoles_RolId",
                table: "UsersRoles",
                newName: "IX_UsersRoles_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Roles_RoleId",
                table: "UsersRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersRoles_Users_UserId",
                table: "UsersRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
