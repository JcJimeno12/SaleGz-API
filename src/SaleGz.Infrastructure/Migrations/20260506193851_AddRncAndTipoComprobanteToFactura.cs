using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaleGz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRncAndTipoComprobanteToFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fiscals_FacturaId",
                table: "Fiscals");

            migrationBuilder.DropIndex(
                name: "IX_Consumos_FacturaId",
                table: "Consumos");

            migrationBuilder.AddColumn<string>(
                name: "RncCliente",
                table: "Facturas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoComprobante",
                table: "Facturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RncCliente",
                table: "FacturaDetalles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fiscals_FacturaId",
                table: "Fiscals",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumos_FacturaId",
                table: "Consumos",
                column: "FacturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fiscals_FacturaId",
                table: "Fiscals");

            migrationBuilder.DropIndex(
                name: "IX_Consumos_FacturaId",
                table: "Consumos");

            migrationBuilder.DropColumn(
                name: "RncCliente",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "TipoComprobante",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "RncCliente",
                table: "FacturaDetalles");

            migrationBuilder.CreateIndex(
                name: "IX_Fiscals_FacturaId",
                table: "Fiscals",
                column: "FacturaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consumos_FacturaId",
                table: "Consumos",
                column: "FacturaId",
                unique: true);
        }
    }
}
