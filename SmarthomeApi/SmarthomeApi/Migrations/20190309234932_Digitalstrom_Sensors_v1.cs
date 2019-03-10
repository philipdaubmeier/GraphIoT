using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmarthomeApi.Migrations
{
    public partial class Digitalstrom_Sensors_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DsZones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<int>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DsSensorDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    Day = table.Column<DateTime>(nullable: false),
                    TemperatureCurve = table.Column<string>(maxLength: 800, nullable: true),
                    HumidityCurve = table.Column<string>(maxLength: 800, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsSensorDataSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DsSensorDataSet_DsZones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "DsZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_DsSensorDataSet_ZoneId_Day",
                table: "DsSensorDataSet",
                columns: new string[] { "ZoneId", "Day" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DsSensorDataSet_ZoneId",
                table: "DsSensorDataSet",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DsSensorDataSet");

            migrationBuilder.DropTable(
                name: "DsZones");
        }
    }
}
