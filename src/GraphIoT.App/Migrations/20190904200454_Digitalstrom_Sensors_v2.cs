using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    public partial class Digitalstrom_Sensors_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DsSensorLowresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    TemperatureCurve = table.Column<string>(maxLength: 800, nullable: true),
                    HumidityCurve = table.Column<string>(maxLength: 800, nullable: true),
                    Month = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsSensorLowresDataSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DsSensorLowresDataSet_DsZones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "DsZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DsSensorLowresDataSet_ZoneId",
                table: "DsSensorLowresDataSet",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DsSensorLowresDataSet");
        }
    }
}
