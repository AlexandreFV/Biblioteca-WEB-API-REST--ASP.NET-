using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesteApiWeb.Migrations
{
    /// <inheritdoc />
    public partial class AltLivroCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livros_Categorias_categoriaId",
                table: "Livros");

            migrationBuilder.DropIndex(
                name: "IX_Livros_categoriaId",
                table: "Livros");

            migrationBuilder.CreateTable(
                name: "CategoriaLivro",
                columns: table => new
                {
                    categoriasCategoriaId = table.Column<int>(type: "int", nullable: false),
                    livrosLivroId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaLivro", x => new { x.categoriasCategoriaId, x.livrosLivroId });
                    table.ForeignKey(
                        name: "FK_CategoriaLivro_Categorias_categoriasCategoriaId",
                        column: x => x.categoriasCategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriaLivro_Livros_livrosLivroId",
                        column: x => x.livrosLivroId,
                        principalTable: "Livros",
                        principalColumn: "LivroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaLivro_livrosLivroId",
                table: "CategoriaLivro",
                column: "livrosLivroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriaLivro");

            migrationBuilder.CreateIndex(
                name: "IX_Livros_categoriaId",
                table: "Livros",
                column: "categoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livros_Categorias_categoriaId",
                table: "Livros",
                column: "categoriaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaId");
        }
    }
}
