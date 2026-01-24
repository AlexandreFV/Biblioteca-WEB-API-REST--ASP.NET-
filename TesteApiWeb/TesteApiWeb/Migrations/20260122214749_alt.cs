using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesteApiWeb.Migrations
{
    /// <inheritdoc />
    public partial class alt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoriaId",
                table: "Livros");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "categoriaId",
                table: "Livros",
                type: "int",
                nullable: true);
        }
    }
}
