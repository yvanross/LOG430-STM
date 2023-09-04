using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Events.Migrations
{
    /// <inheritdoc />
    public partial class InitEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventDto",
                columns: table => new
                {
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Event = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDto", x => x.EventType);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventDto");
        }
    }
}
