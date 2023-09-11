using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.ReadRepositories.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bus",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TripId = table.Column<string>(type: "text", nullable: false),
                    CurrentStopIndex = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ride",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BusId = table.Column<string>(type: "text", nullable: false),
                    DepartureId = table.Column<string>(type: "text", nullable: false),
                    DestinationId = table.Column<string>(type: "text", nullable: false),
                    FirstRecordedStopId = table.Column<string>(type: "text", nullable: false),
                    TrackingComplete = table.Column<bool>(type: "boolean", nullable: false),
                    TripBegunTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartureReachedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ride", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledStopDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StopId = table.Column<string>(type: "text", nullable: false),
                    TripId = table.Column<string>(type: "text", nullable: false),
                    DepartureTimespan = table.Column<TimeSpan>(type: "interval", nullable: false),
                    StopSequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledStopDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stop",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Position_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Position_Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stop", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledStopDto_TripId",
                table: "ScheduledStopDto",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bus");

            migrationBuilder.DropTable(
                name: "Ride");

            migrationBuilder.DropTable(
                name: "ScheduledStopDto");

            migrationBuilder.DropTable(
                name: "Stop");
        }
    }
}
