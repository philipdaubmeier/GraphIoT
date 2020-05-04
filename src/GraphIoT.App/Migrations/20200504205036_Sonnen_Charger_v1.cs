using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    public partial class Sonnen_Charger_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SonnenChargerDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChargedEnergyCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ActivePowerCurve = table.Column<string>(maxLength: 800, nullable: true),
                    CurrentCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ConnectedCurve = table.Column<string>(maxLength: 100, nullable: true),
                    ChargingCurve = table.Column<string>(maxLength: 100, nullable: true),
                    SmartModeCurve = table.Column<string>(maxLength: 100, nullable: true),
                    Day = table.Column<DateTime>(nullable: false),
                    ChargedEnergyTotal = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonnenChargerDataSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SonnenChargerLowresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChargedEnergyCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ActivePowerCurve = table.Column<string>(maxLength: 800, nullable: true),
                    CurrentCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ConnectedCurve = table.Column<string>(maxLength: 100, nullable: true),
                    ChargingCurve = table.Column<string>(maxLength: 100, nullable: true),
                    SmartModeCurve = table.Column<string>(maxLength: 100, nullable: true),
                    Month = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonnenChargerLowresDataSet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SonnenChargerDataSet");

            migrationBuilder.DropTable(
                name: "SonnenChargerLowresDataSet");
        }
    }
}
