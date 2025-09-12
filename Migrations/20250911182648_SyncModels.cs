using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospOps.Migrations
{
    /// <inheritdoc />
    public partial class SyncModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shift",
                table: "LogEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Shift",
                table: "LogEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
