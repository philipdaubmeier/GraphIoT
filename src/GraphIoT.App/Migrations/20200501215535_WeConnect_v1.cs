using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    public partial class WeConnect_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DsEnergyLowresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyLowresDataSet");

            migrationBuilder.DropForeignKey(
                name: "FK_DsEnergyMidresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyMidresDataSet");

            migrationBuilder.AlterColumn<string>(
                name: "CircuitId",
                table: "DsEnergyMidresDataSet",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 34,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CircuitId",
                table: "DsEnergyLowresDataSet",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 34,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WeConnectDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Vin = table.Column<string>(maxLength: 20, nullable: false),
                    DrivenKilometersCurve = table.Column<string>(maxLength: 800, nullable: true),
                    BatterySocCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ConsumedKwhCurve = table.Column<string>(maxLength: 800, nullable: true),
                    AverageConsumptionCurve = table.Column<string>(maxLength: 800, nullable: true),
                    AverageSpeedCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ChargingStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    ClimateTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ClimateStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    WindowMeltStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    RemoteHeatingStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    Day = table.Column<DateTime>(nullable: false),
                    Mileage = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeConnectDataSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeConnectLowresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Vin = table.Column<string>(maxLength: 20, nullable: false),
                    DrivenKilometersCurve = table.Column<string>(maxLength: 800, nullable: true),
                    BatterySocCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ConsumedKwhCurve = table.Column<string>(maxLength: 800, nullable: true),
                    AverageConsumptionCurve = table.Column<string>(maxLength: 800, nullable: true),
                    AverageSpeedCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ChargingStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    ClimateTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    ClimateStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    WindowMeltStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    RemoteHeatingStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    Month = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeConnectLowresDataSet", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DsEnergyLowresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyLowresDataSet",
                column: "CircuitId",
                principalTable: "DsCircuits",
                principalColumn: "Dsuid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DsEnergyMidresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyMidresDataSet",
                column: "CircuitId",
                principalTable: "DsCircuits",
                principalColumn: "Dsuid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DsEnergyLowresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyLowresDataSet");

            migrationBuilder.DropForeignKey(
                name: "FK_DsEnergyMidresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyMidresDataSet");

            migrationBuilder.DropTable(
                name: "WeConnectDataSet");

            migrationBuilder.DropTable(
                name: "WeConnectLowresDataSet");

            migrationBuilder.AlterColumn<string>(
                name: "CircuitId",
                table: "DsEnergyMidresDataSet",
                maxLength: 34,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 34);

            migrationBuilder.AlterColumn<string>(
                name: "CircuitId",
                table: "DsEnergyLowresDataSet",
                maxLength: 34,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 34);

            migrationBuilder.AddForeignKey(
                name: "FK_DsEnergyLowresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyLowresDataSet",
                column: "CircuitId",
                principalTable: "DsCircuits",
                principalColumn: "Dsuid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DsEnergyMidresDataSet_DsCircuits_CircuitId",
                table: "DsEnergyMidresDataSet",
                column: "CircuitId",
                principalTable: "DsCircuits",
                principalColumn: "Dsuid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
