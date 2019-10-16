using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    public partial class Sonnen_Energy_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SonnenEnergyDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Day = table.Column<DateTime>(nullable: false),
                    ProductionPowerCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    ConsumptionPowerCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    DirectUsagePowerCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    BatteryChargingCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    BatteryDischargingCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    GridFeedinCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    GridPurchaseCurve = table.Column<string>(maxLength: 4000, nullable: true),
                    BatteryUsocCurve = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonnenEnergyDataSet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SonnenEnergyDataSet");
        }
    }
}
