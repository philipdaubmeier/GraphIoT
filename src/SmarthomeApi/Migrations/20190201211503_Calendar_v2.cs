using Microsoft.EntityFrameworkCore.Migrations;

namespace PhilipDaubmeier.SmarthomeApi.Migrations
{
    public partial class Calendar_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExBusyState",
                table: "CalendarOccurances",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExLocationLong",
                table: "CalendarOccurances",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExLocationShort",
                table: "CalendarOccurances",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExSummary",
                table: "CalendarOccurances",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationLong",
                table: "CalendarAppointments",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationShort",
                table: "CalendarAppointments",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExBusyState",
                table: "CalendarOccurances");

            migrationBuilder.DropColumn(
                name: "ExLocationLong",
                table: "CalendarOccurances");

            migrationBuilder.DropColumn(
                name: "ExLocationShort",
                table: "CalendarOccurances");

            migrationBuilder.DropColumn(
                name: "ExSummary",
                table: "CalendarOccurances");

            migrationBuilder.DropColumn(
                name: "LocationLong",
                table: "CalendarAppointments");

            migrationBuilder.DropColumn(
                name: "LocationShort",
                table: "CalendarAppointments");
        }
    }
}
