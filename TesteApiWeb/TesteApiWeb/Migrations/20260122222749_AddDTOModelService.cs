using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesteApiWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddDTOModelService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Categorias_categoriasCategoriaId",
                table: "CategoriaLivro");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Livros_livrosLivroId",
                table: "CategoriaLivro");

            migrationBuilder.RenameColumn(
                name: "tipo",
                table: "Usuarios",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "senha",
                table: "Usuarios",
                newName: "Senha");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Usuarios",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "quantidade",
                table: "Livros",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Livros",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Categorias",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "livrosLivroId",
                table: "CategoriaLivro",
                newName: "LivrosLivroId");

            migrationBuilder.RenameColumn(
                name: "categoriasCategoriaId",
                table: "CategoriaLivro",
                newName: "CategoriasCategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaLivro_livrosLivroId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Categorias_CategoriasCategoriaId",
                table: "CategoriaLivro");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaLivro_Livros_LivrosLivroId",
                table: "CategoriaLivro");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Usuarios",
                newName: "tipo");

            migrationBuilder.RenameColumn(
                name: "Senha",
                table: "Usuarios",
                newName: "senha");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Usuarios",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "Livros",
                newName: "quantidade");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Livros",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Categorias",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "LivrosLivroId",
                table: "CategoriaLivro",
                newName: "livrosLivroId");

            migrationBuilder.RenameColumn(
                name: "CategoriasCategoriaId",
                table: "CategoriaLivro",
                newName: "categoriasCategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaLivro_LivrosLivroId",
                table: "CategoriaLivro",
                newName: "IX_CategoriaLivro_livrosLivroId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaLivro_Categorias_categoriasCategoriaId",
                table: "CategoriaLivro",
                column: "categoriasCategoriaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaLivro_Livros_livrosLivroId",
                table: "CategoriaLivro",
                column: "livrosLivroId",
                principalTable: "Livros",
                principalColumn: "LivroId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
