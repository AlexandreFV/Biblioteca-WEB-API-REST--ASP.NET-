using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca_WEB_API_REST_ASP.NET.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitacaoEmprestimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimos_AspNetUsers_UsuarioEmprestimoId",
                table: "Emprestimos");

            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimos_Livros_LivroId",
                table: "Emprestimos");

            migrationBuilder.DropIndex(
                name: "IX_Emprestimos_UsuarioEmprestimoId",
                table: "Emprestimos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Emprestimos");

            migrationBuilder.DropColumn(
                name: "UsuarioEmprestimoId",
                table: "Emprestimos");

            migrationBuilder.RenameColumn(
                name: "LivroId",
                table: "Emprestimos",
                newName: "SolicitacaoEmprestimoId");

            migrationBuilder.RenameColumn(
                name: "DataEmprestimo",
                table: "Emprestimos",
                newName: "DataPrevistaDevolucao");

            migrationBuilder.RenameIndex(
                name: "IX_Emprestimos_LivroId",
                table: "Emprestimos",
                newName: "IX_Emprestimos_SolicitacaoEmprestimoId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAceiteEmprestimo",
                table: "Emprestimos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDevolucao",
                table: "Emprestimos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Solicitacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioEmprestimo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdLivro = table.Column<int>(type: "int", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitacoes_AspNetUsers_IdUsuarioEmprestimo",
                        column: x => x.IdUsuarioEmprestimo,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitacoes_Livros_IdLivro",
                        column: x => x.IdLivro,
                        principalTable: "Livros",
                        principalColumn: "LivroId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_IdLivro",
                table: "Solicitacoes",
                column: "IdLivro");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_IdUsuarioEmprestimo",
                table: "Solicitacoes",
                column: "IdUsuarioEmprestimo");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimos_Solicitacoes_SolicitacaoEmprestimoId",
                table: "Emprestimos",
                column: "SolicitacaoEmprestimoId",
                principalTable: "Solicitacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprestimos_Solicitacoes_SolicitacaoEmprestimoId",
                table: "Emprestimos");

            migrationBuilder.DropTable(
                name: "Solicitacoes");

            migrationBuilder.DropColumn(
                name: "DataAceiteEmprestimo",
                table: "Emprestimos");

            migrationBuilder.DropColumn(
                name: "DataDevolucao",
                table: "Emprestimos");

            migrationBuilder.RenameColumn(
                name: "SolicitacaoEmprestimoId",
                table: "Emprestimos",
                newName: "LivroId");

            migrationBuilder.RenameColumn(
                name: "DataPrevistaDevolucao",
                table: "Emprestimos",
                newName: "DataEmprestimo");

            migrationBuilder.RenameIndex(
                name: "IX_Emprestimos_SolicitacaoEmprestimoId",
                table: "Emprestimos",
                newName: "IX_Emprestimos_LivroId");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Emprestimos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEmprestimoId",
                table: "Emprestimos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimos_UsuarioEmprestimoId",
                table: "Emprestimos",
                column: "UsuarioEmprestimoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimos_AspNetUsers_UsuarioEmprestimoId",
                table: "Emprestimos",
                column: "UsuarioEmprestimoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emprestimos_Livros_LivroId",
                table: "Emprestimos",
                column: "LivroId",
                principalTable: "Livros",
                principalColumn: "LivroId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
