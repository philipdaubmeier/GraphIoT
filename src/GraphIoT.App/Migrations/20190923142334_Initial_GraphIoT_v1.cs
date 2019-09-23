using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    public partial class Initial_GraphIoT_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarOccurances");

            migrationBuilder.DropTable(
                name: "CalendarAppointments");

            migrationBuilder.DropTable(
                name: "Calendars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Owner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarAppointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BusyState = table.Column<int>(nullable: false),
                    CalendarId = table.Column<Guid>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    LocationLong = table.Column<string>(maxLength: 80, nullable: false),
                    LocationShort = table.Column<string>(maxLength: 32, nullable: false),
                    Summary = table.Column<string>(maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarAppointments_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarOccurances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppointmentId = table.Column<Guid>(nullable: true),
                    CalendarAppointmentId = table.Column<Guid>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    ExBusyState = table.Column<int>(nullable: true),
                    ExLocationLong = table.Column<string>(maxLength: 80, nullable: true),
                    ExLocationShort = table.Column<string>(maxLength: 32, nullable: true),
                    ExSummary = table.Column<string>(maxLength: 120, nullable: true),
                    IsFullDay = table.Column<bool>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarOccurances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarOccurances_CalendarAppointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "CalendarAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarAppointments_CalendarId",
                table: "CalendarAppointments",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarOccurances_AppointmentId",
                table: "CalendarOccurances",
                column: "AppointmentId");
        }
    }
}
