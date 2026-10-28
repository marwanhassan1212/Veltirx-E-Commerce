using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veltrix.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreSettingsIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StoreSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreSettings_IsActive",
                table: "StoreSettings",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreSettings_IsActive",
                table: "StoreSettings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StoreSettings");
        }
    }
}
