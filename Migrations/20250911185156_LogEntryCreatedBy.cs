using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospOps.Migrations
{
    /// <inheritdoc />
    public partial class LogEntryCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LogEntries",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LogEntries");
        }
    }
}
