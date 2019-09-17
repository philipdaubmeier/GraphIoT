using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.SmarthomeApi.Migrations
{
    public partial class Sonnen_Energy_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SonnenEnergyLowresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductionPowerCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    ConsumptionPowerCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    DirectUsagePowerCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    BatteryChargingCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    BatteryDischargingCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    GridFeedinCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    GridPurchaseCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    BatteryUsocCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    Month = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonnenEnergyLowresDataSet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SonnenEnergyLowresDataSet");
        }
    }
}
