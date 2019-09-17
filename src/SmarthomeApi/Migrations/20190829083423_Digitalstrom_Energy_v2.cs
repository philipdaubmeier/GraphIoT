using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.SmarthomeApi.Migrations
{
    public partial class Digitalstrom_Energy_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DsCircuits",
                columns: table => new
                {
                    Dsuid = table.Column<string>(maxLength: 34, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsCircuits", x => x.Dsuid);
                });

            migrationBuilder.CreateTable(
                name: "DsEnergyMidresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CircuitId = table.Column<string>(maxLength: 34, nullable: true),
                    Day = table.Column<DateTime>(nullable: false),
                    EnergyCurve = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsEnergyMidresDataSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DsEnergyMidresDataSet_DsCircuits_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "DsCircuits",
                        principalColumn: "Dsuid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DsEnergyMidresDataSet_CircuitId",
                table: "DsEnergyMidresDataSet",
                column: "CircuitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DsEnergyMidresDataSet");

            migrationBuilder.DropTable(
                name: "DsCircuits");
        }
    }
}
