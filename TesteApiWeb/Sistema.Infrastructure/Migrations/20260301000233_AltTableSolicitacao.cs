using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca_WEB_API_REST_ASP.NET.Migrations
{
    /// <inheritdoc />
    public partial class AltTableSolicitacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitacoes_AspNetUsers_IdUsuarioEmprestimo",
                table: "Solicitacoes");

            migrationBuilder.DropTable(
                name: "Emprestimos");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioEmprestimo",
                table: "Solicitacoes",
                newName: "IdUsuarioCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Solicitacoes_IdUsuarioEmprestimo",
                table: "Solicitacoes",
                newName: "IX_Solicitacoes_IdUsuarioCliente");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAlteracaoStatus",
                table: "Solicitacoes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IdUsuarioAdmin",
                table: "Solicitacoes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_IdUsuarioAdmin",
                table: "Solicitacoes",
                column: "IdUsuarioAdmin");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitacoes_AspNetUsers_IdUsuarioAdmin",
                table: "Solicitacoes",
                column: "IdUsuarioAdmin",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitacoes_AspNetUsers_IdUsuarioCliente",
                table: "Solicitacoes",
                column: "IdUsuarioCliente",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitacoes_AspNetUsers_IdUsuarioAdmin",
                table: "Solicitacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Solicitacoes_AspNetUsers_IdUsuarioCliente",
                table: "Solicitacoes");

            migrationBuilder.DropIndex(
                name: "IX_Solicitacoes_IdUsuarioAdmin",
                table: "Solicitacoes");

            migrationBuilder.DropColumn(
                name: "DataAlteracaoStatus",
                table: "Solicitacoes");

            migrationBuilder.DropColumn(
                name: "IdUsuarioAdmin",
                table: "Solicitacoes");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioCliente",
                table: "Solicitacoes",
                newName: "IdUsuarioEmprestimo");

            migrationBuilder.RenameIndex(
                name: "IX_Solicitacoes_IdUsuarioCliente",
                table: "Solicitacoes",
                newName: "IX_Solicitacoes_IdUsuarioEmprestimo");

            migrationBuilder.CreateTable(
                name: "Emprestimos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitacaoEmprestimoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioAdminId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataAceiteEmprestimo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDevolucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPrevistaDevolucao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprestimos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emprestimos_AspNetUsers_UsuarioAdminId",
                        column: x => x.UsuarioAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Emprestimos_Solicitacoes_SolicitacaoEmprestimoId",
                        column: x => x.SolicitacaoEmprestimoId,
                        principalTable: "Solicitacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimos_SolicitacaoEmprestimoId",
                table: "Emprestimos",
                column: "SolicitacaoEmprestimoId");

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimos_UsuarioAdminId",
                table: "Emprestimos",
                column: "UsuarioAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitacoes_AspNetUsers_IdUsuarioEmprestimo",
                table: "Solicitacoes",
                column: "IdUsuarioEmprestimo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
