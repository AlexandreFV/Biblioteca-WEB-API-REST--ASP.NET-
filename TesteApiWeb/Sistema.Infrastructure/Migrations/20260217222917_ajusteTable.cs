using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca_WEB_API_REST_ASP.NET.Migrations
{
    /// <inheritdoc />
    public partial class ajusteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Categorias_CategoriasCategoriaId",
                table: "CategoriaLivro");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Livros_LivrosLivroId",
                table: "CategoriaLivro");

            migrationBuilder.RenameColumn(
                name: "LivroId",
                table: "Livros",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Categorias",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LivrosLivroId",
                table: "CategoriaLivro",
                newName: "LivrosId");

            migrationBuilder.RenameColumn(
                name: "CategoriasCategoriaId",
                table: "CategoriaLivro",
                newName: "CategoriasId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaLivro_LivrosLivroId",
                table: "CategoriaLivro",
                newName: "IX_CategoriaLivro_LivrosId");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Emprestimos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaLivro_Categorias_CategoriasId",
                table: "CategoriaLivro",
                column: "CategoriasId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaLivro_Livros_LivrosId",
                table: "CategoriaLivro",
                column: "LivrosId",
                principalTable: "Livros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Categorias_CategoriasId",
                table: "CategoriaLivro");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Livros_LivrosId",
                table: "CategoriaLivro");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Emprestimos");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Livros",
                newName: "LivroId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categorias",
                newName: "CategoriaId");

            migrationBuilder.RenameColumn(
                name: "LivrosId",
                table: "CategoriaLivro",
                newName: "LivrosLivroId");

            migrationBuilder.RenameColumn(
                name: "CategoriasId",
                table: "CategoriaLivro",
                newName: "CategoriasCategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaLivro_LivrosId",
                table: "CategoriaLivro",
                newName: "IX_CategoriaLivro_LivrosLivroId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaLivro_Categorias_CategoriasCategoriaId",
                table: "CategoriaLivro",
                column: "CategoriasCategoriaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaLivro_Livros_LivrosLivroId",
                table: "CategoriaLivro",
                column: "LivrosLivroId",
                principalTable: "Livros",
                principalColumn: "LivroId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
