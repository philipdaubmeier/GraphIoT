using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.SmarthomeApi.Migrations
{
    public partial class Solar_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarOccurances_CalendarAppointments_AppointmentId",
                table: "CalendarOccurances");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppointmentId",
                table: "CalendarOccurances",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateTable(
                name: "ViessmannSolarTimeseries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Day = table.Column<DateTime>(nullable: false),
                    SolarWhTotal = table.Column<int>(nullable: true),
                    SolarWhCurve = table.Column<string>(maxLength: 800, nullable: true),
                    SolarCollectorTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    SolarHotwaterTempCurve = table.Column<string>(maxLength: 800, nullable: true),
                    SolarPumpStateCurve = table.Column<string>(maxLength: 100, nullable: true),
                    SolarSuppressionCurve = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViessmannSolarTimeseries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViessmannSolarTimeseries_Day",
                table: "ViessmannSolarTimeseries",
                column: "Day",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarOccurances_CalendarAppointments_AppointmentId",
                table: "CalendarOccurances",
                column: "AppointmentId",
                principalTable: "CalendarAppointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarOccurances_CalendarAppointments_AppointmentId",
                table: "CalendarOccurances");

            migrationBuilder.DropTable(
                name: "ViessmannSolarTimeseries");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppointmentId",
                table: "CalendarOccurances",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarOccurances_CalendarAppointments_AppointmentId",
                table: "CalendarOccurances",
                column: "AppointmentId",
                principalTable: "CalendarAppointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
