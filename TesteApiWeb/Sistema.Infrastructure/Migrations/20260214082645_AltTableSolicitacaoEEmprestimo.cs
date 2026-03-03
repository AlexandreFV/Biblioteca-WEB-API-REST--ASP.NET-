using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca_WEB_API_REST_ASP.NET.Migrations
{
    /// <inheritdoc />
    public partial class AltTableSolicitacaoEEmprestimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Solicitacoes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Solicitacoes");
        }
    }
}
