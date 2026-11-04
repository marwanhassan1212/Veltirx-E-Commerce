using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veltrix.Migrations
{
    /// <inheritdoc />
    public partial class FixProductVariantDefaultIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId_Default",
                table: "ProductVariants",
                column: "ProductId",
                unique: true,
                filter: "[IsDefault] = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProductId_Default",
                table: "ProductVariants");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId",
                unique: true);
        }
    }
}
