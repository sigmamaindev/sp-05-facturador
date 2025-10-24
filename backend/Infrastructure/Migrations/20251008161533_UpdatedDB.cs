using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Producto",
                newName: "TipoProducto");

            migrationBuilder.AddColumn<string>(
                name: "Secuencia",
                table: "Usuario",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Producto",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Secuencia",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Producto");

            migrationBuilder.RenameColumn(
                name: "TipoProducto",
                table: "Producto",
                newName: "Tipo");
        }
    }
}
