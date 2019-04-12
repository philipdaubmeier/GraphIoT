using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmarthomeApi.Migrations
{
    public partial class Digitalstrom_SceneEvents_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DsSceneEventDataSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Day = table.Column<DateTime>(nullable: false),
                    EventStreamEncoded = table.Column<string>(maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DsSceneEventDataSet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DsSceneEventDataSet_Day",
                table: "DsSceneEventDataSet",
                column: "Day",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DsSceneEventDataSet");
        }
    }
}
