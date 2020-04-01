using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductScraper.Data.Migrations
{
    public partial class confgis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrapeConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: true),
                    ProductNamePath = table.Column<string>(nullable: true),
                    ProductPricePath = table.Column<string>(nullable: true),
                    ProductSecondPricePath = table.Column<string>(nullable: true),
                    ProductAvailabilityPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapeConfigs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrapeConfigs");
        }
    }
}
