using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductScraper.Data.Migrations
{
    public partial class addAtributeProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProductAvailabilityIsAtributeValue",
                table: "ScrapeConfigs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductAvailabilityIsAtributeValue",
                table: "ScrapeConfigs");
        }
    }
}
