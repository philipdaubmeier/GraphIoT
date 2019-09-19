using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    public partial class Digitalstrom_Energy_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DsEnergyLowresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CircuitId = table.Column<string>(maxLength: 34, nullable: true),
                    Month = table.Column<DateTime>(nullable: false),
                    EnergyCurve = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsEnergyLowresDataSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DsEnergyLowresDataSet_DsCircuits_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "DsCircuits",
                        principalColumn: "Dsuid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DsEnergyLowresDataSet_CircuitId",
                table: "DsEnergyLowresDataSet",
                column: "CircuitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DsEnergyLowresDataSet");
        }
    }
}
