using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncPurchaseNoEstablishmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
    ALTER TABLE "Compra"
    DROP CONSTRAINT IF EXISTS "FK_Compra_Establecimiento_EstablishmentId";
    """);

            // Index
            migrationBuilder.Sql("""
    DROP INDEX IF EXISTS "IX_Compra_EstablishmentId";
    """);

            // Column
            migrationBuilder.Sql("""
    ALTER TABLE "Compra"
    DROP COLUMN IF EXISTS "EstablishmentId";
    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstablishmentId",
                table: "Compra",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compra_EstablishmentId",
                table: "Compra",
                column: "EstablishmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Establecimiento_EstablishmentId",
                table: "Compra",
                column: "EstablishmentId",
                principalTable: "Establecimiento",
                principalColumn: "Id");
        }
    }
}
