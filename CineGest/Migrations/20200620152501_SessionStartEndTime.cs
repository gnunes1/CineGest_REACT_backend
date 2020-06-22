using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CineGest.Migrations
{
    public partial class SessionStartEndTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Start_Time",
                table: "Sessions");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "DoB",
                value: new DateTime(2020, 6, 20, 15, 25, 1, 226, DateTimeKind.Utc).AddTicks(4459));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Start_Time",
                table: "Sessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "DoB",
                value: new DateTime(2020, 6, 19, 17, 20, 7, 224, DateTimeKind.Utc).AddTicks(36));
        }
    }
}
