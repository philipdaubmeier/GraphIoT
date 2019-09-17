using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.SmarthomeApi.Migrations
{
    public partial class Netatmo_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NetatmoModuleMeasures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DeviceId = table.Column<string>(maxLength: 17, nullable: false),
                    ModuleId = table.Column<string>(maxLength: 17, nullable: false),
                    Measure = table.Column<string>(maxLength: 20, nullable: false),
                    Decimals = table.Column<int>(nullable: false),
                    StationName = table.Column<string>(maxLength: 30, nullable: true),
                    ModuleName = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetatmoModuleMeasures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetatmoMeasureDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ModuleMeasureId = table.Column<Guid>(nullable: false),
                    Decimals = table.Column<int>(nullable: false),
                    MeasureCurve = table.Column<string>(maxLength: 800, nullable: true),
                    Day = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetatmoMeasureDataSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetatmoMeasureDataSet_NetatmoModuleMeasures_ModuleMeasureId",
                        column: x => x.ModuleMeasureId,
                        principalTable: "NetatmoModuleMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetatmoMeasureLowresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ModuleMeasureId = table.Column<Guid>(nullable: false),
                    Decimals = table.Column<int>(nullable: false),
                    MeasureCurve = table.Column<string>(maxLength: 800, nullable: true),
                    Month = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetatmoMeasureLowresDataSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetatmoMeasureLowresDataSet_NetatmoModuleMeasures_ModuleMeasureId",
                        column: x => x.ModuleMeasureId,
                        principalTable: "NetatmoModuleMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NetatmoMeasureDataSet_ModuleMeasureId",
                table: "NetatmoMeasureDataSet",
                column: "ModuleMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_NetatmoMeasureLowresDataSet_ModuleMeasureId",
                table: "NetatmoMeasureLowresDataSet",
                column: "ModuleMeasureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetatmoMeasureDataSet");

            migrationBuilder.DropTable(
                name: "NetatmoMeasureLowresDataSet");

            migrationBuilder.DropTable(
                name: "NetatmoModuleMeasures");
        }
    }
}
