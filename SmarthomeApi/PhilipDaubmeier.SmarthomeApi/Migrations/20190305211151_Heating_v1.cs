using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.SmarthomeApi.Migrations
{
    public partial class Heating_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViessmannHeatingTimeseries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Day = table.Column<DateTime>(nullable: false),
                    BurnerHoursTotal = table.Column<double>(nullable: false),
                    BurnerStartsTotal = table.Column<int>(nullable: false),
                    BurnerMinutesCurve = table.Column<string>(maxLength: 800, nullable: true),
                    BurnerStartsCurve = table.Column<string>(maxLength: 800, nullable: true),
                    BurnerModulationCurve = table.Column<string>(maxLength: 800, nullable: true),
                    OutsideTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    BoilerTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    BoilerTempMainCurve = table.Column<string>(maxLength: 800, nullable: true),
                    Circuit0TempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    Circuit1TempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    DhwTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    BurnerActiveCurve = table.Column<string>(maxLength: 100, nullable: true),
                    Circuit0PumpCurve = table.Column<string>(maxLength: 100, nullable: true),
                    Circuit1PumpCurve = table.Column<string>(maxLength: 100, nullable: true),
                    DhwPrimaryPumpCurve = table.Column<string>(maxLength: 100, nullable: true),
                    DhwCirculationPumpCurve = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViessmannHeatingTimeseries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViessmannHeatingTimeseries");
        }
    }
}
