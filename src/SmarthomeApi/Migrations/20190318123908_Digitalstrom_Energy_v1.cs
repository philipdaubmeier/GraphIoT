using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.SmarthomeApi.Migrations
{
    public partial class Digitalstrom_Energy_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DsEnergyHighresDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Day = table.Column<DateTime>(nullable: false),
                    EnergyCurvesEveryMeter = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsEnergyHighresDataSet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DsEnergyHighresDataSet");
        }
    }
}
