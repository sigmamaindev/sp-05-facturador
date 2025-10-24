using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductUpdatedBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "MaxStock",
                table: "ProductoBodega",
                newName: "CantidadMinima");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_TipoDocumento_TipoDocumentoId",
                table: "Cliente",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_TipoDocumento_TipoDocumentoId",
                table: "Cliente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoDocumento",
                table: "TipoDocumento");

            migrationBuilder.RenameTable(
                name: "TipoDocumento",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "CantidadMinima",
                table: "ProductoBodega",
                newName: "MaxStock");

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
    }
}
