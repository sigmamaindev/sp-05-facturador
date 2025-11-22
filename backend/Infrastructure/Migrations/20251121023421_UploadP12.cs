using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UploadP12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificadoEmpresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    CertificadoBase64 = table.Column<string>(type: "text", nullable: false),
                    CertificadoClave = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificadoEmpresa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificadoEmpresa_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificadoEmpresa_EmpresaId",
                table: "CertificadoEmpresa",
                column: "EmpresaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificadoEmpresa");
        }
    }
}
