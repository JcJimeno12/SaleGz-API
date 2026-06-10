using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaleGz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoEntidadToClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoEntidad",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoEntidad",
                table: "Clientes");
        }
    }
}
