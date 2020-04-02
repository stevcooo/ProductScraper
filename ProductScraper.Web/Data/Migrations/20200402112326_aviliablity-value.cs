using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductScraper.Data.Migrations
{
    public partial class aviliablityvalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductAvailabilityValue",
                table: "ScrapeConfigs",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "URL",
                table: "ProductInfos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductAvailabilityValue",
                table: "ScrapeConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "URL",
                table: "ProductInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
