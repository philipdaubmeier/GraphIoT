using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    public partial class Solar_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViessmannSolarLowresTimeseries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SolarWhCurve = table.Column<string>(maxLength: 800, nullable: true),
                    SolarCollectorTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    SolarHotwaterTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    SolarPumpStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    SolarSuppressionCurve = table.Column<string>(maxLength: 100, nullable: true),
                    Month = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViessmannSolarLowresTimeseries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViessmannSolarLowresTimeseries");
        }
    }
}
