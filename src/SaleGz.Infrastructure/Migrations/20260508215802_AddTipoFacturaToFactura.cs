using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaleGz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoFacturaToFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoFactura",
                table: "Facturas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoFactura",
                table: "Facturas");
        }
    }
}
