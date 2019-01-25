using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmarthomeApi.Migrations
{
    public partial class Calendar_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UUID = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UID = table.Column<string>(maxLength: 120, nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    IsFullDay = table.Column<bool>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    RecurranceRule = table.Column<string>(maxLength: 255, nullable: false),
                    Summary = table.Column<string>(nullable: true),
                    BusyState = table.Column<int>(nullable: false),
                    CalendarId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEntry_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEntry_CalendarId",
                table: "CalendarEntry",
                column: "CalendarId");
            
            migrationBuilder.CreateIndex(
                name: "IX_CalendarEntry_UID",
                table: "CalendarEntry",
                column: "UID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEntry");

            migrationBuilder.DropTable(
                name: "Calendars");
        }
    }
}
