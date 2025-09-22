// ============================================================================
// File: Migrations/20250922_AddAdminDataLists.cs  (ADD NEW FILE)
// Creates Floors, RoomTypes, Rooms, PhonebookTypes; extends WorkOrderStatuses.
// Handles both SQL Server and SQLite for the "single default status" rule.
// ============================================================================
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospOps.Migrations
{
    public partial class AddAdminDataLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Floors
            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table => { table.PrimaryKey("PK_Floors", x => x.Id); });

            migrationBuilder.CreateIndex(
                name: "IX_Floors_SortOrder",
                table: "Floors",
                column: "SortOrder");

            // RoomTypes
            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    Description = table.Column<string>(maxLength: 120, nullable: true),
                    SortOrder = table.Column<int>(nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table => { table.PrimaryKey("PK_RoomTypes", x => x.Id); });

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_IsActive",
                table: "RoomTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_SortOrder",
                table: "RoomTypes",
                column: "SortOrder");

            // Rooms
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomNumber = table.Column<string>(maxLength: 20, nullable: false),
                    FloorId = table.Column<int>(nullable: false),
                    RoomTypeId = table.Column<int>(nullable: true),
                    PosX = table.Column<int>(nullable: true),
                    PosY = table.Column<int>(nullable: true),
                    Width = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    IsOutOfOrder = table.Column<bool>(nullable: false, defaultValue: false),
                    Notes = table.Column<string>(maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey("FK_Rooms_Floors_FloorId", x => x.FloorId, "Floors", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Rooms_RoomTypes_RoomTypeId", x => x.RoomTypeId, "RoomTypes", "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomNumber",
                table: "Rooms",
                column: "RoomNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_FloorId",
                table: "Rooms",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms",
                column: "RoomTypeId");

            // PhonebookTypes
            migrationBuilder.CreateTable(
                name: "PhonebookTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    ColorHex = table.Column<string>(maxLength: 7, nullable: false, defaultValue: "#6c757d"),
                    SortOrder = table.Column<int>(nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table => { table.PrimaryKey("PK_PhonebookTypes", x => x.Id); });

            migrationBuilder.CreateIndex(
                name: "IX_PhonebookTypes_IsActive",
                table: "PhonebookTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PhonebookTypes_SortOrder",
                table: "PhonebookTypes",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_PhonebookTypes_Name",
                table: "PhonebookTypes",
                column: "Name",
                unique: true);

            // Alter WorkOrderStatuses - add ColorHex, IsDefault
            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "WorkOrderStatuses",
                type: "TEXT",
                maxLength: 7,
                nullable: false,
                defaultValue: "#6c757d");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "WorkOrderStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "WorkOrderStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WorkOrderStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            // Enforce single default status per table
            if (ActiveProvider.Contains("SqlServer"))
            {
                migrationBuilder.CreateIndex(
                    name: "IX_WorkOrderStatuses_IsDefault_OneTrue",
                    table: "WorkOrderStatuses",
                    column: "IsDefault",
                    unique: true,
                    filter: "[IsDefault] = 1");
            }
            else if (ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.Sql(
                    "CREATE UNIQUE INDEX IF NOT EXISTS IX_WorkOrderStatuses_IsDefault_OneTrue ON WorkOrderStatuses(IsDefault) WHERE IsDefault = 1;");
            }

            // Seed minimal data if empty
            migrationBuilder.Sql(@"
                -- Floors
                INSERT INTO Floors (Name, Description, SortOrder, CreatedAt, UpdatedAt, IsDeleted)
                SELECT '1', 'First Floor', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 0
                WHERE NOT EXISTS (SELECT 1 FROM Floors);

                -- RoomTypes
                INSERT INTO RoomTypes (Name, Description, SortOrder, IsActive)
                SELECT 'King', 'KNG', 1, 1
                WHERE NOT EXISTS (SELECT 1 FROM RoomTypes);
                INSERT INTO RoomTypes (Name, Description, SortOrder, IsActive)
                SELECT 'Double Queen', 'QQ', 2, 1
                WHERE NOT EXISTS (SELECT 1 FROM RoomTypes WHERE Name = 'Double Queen');

                -- PhonebookTypes
                INSERT INTO PhonebookTypes (Name, ColorHex, SortOrder, IsActive)
                SELECT 'Employee', '#0d6efd', 1, 1
                WHERE NOT EXISTS (SELECT 1 FROM PhonebookTypes);
                INSERT INTO PhonebookTypes (Name, ColorHex, SortOrder, IsActive)
                SELECT 'Vendor', '#198754', 2, 1
                WHERE NOT EXISTS (SELECT 1 FROM PhonebookTypes WHERE Name = 'Vendor');

                -- WorkOrderStatuses backfill
                UPDATE WorkOrderStatuses SET ColorHex = '#0d6efd' WHERE Name IN ('New','Open') AND ColorHex = '#6c757d';
                UPDATE WorkOrderStatuses SET ColorHex = '#6c757d' WHERE ColorHex IS NULL OR ColorHex = '';

                -- Set a default status if none marked
                UPDATE WorkOrderStatuses SET IsDefault = 1
                WHERE Id = (
                    SELECT Id FROM WorkOrderStatuses
                    WHERE Name IN ('New','Open')
                    ORDER BY Id LIMIT 1
                )
                AND (SELECT COUNT(*) FROM WorkOrderStatuses WHERE IsDefault = 1) = 0;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop unique index on default status
            if (ActiveProvider.Contains("SqlServer"))
            {
                migrationBuilder.DropIndex(
                    name: "IX_WorkOrderStatuses_IsDefault_OneTrue",
                    table: "WorkOrderStatuses");
            }
            else if (ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.Sql("DROP INDEX IF EXISTS IX_WorkOrderStatuses_IsDefault_OneTrue;");
            }

            // Revert WorkOrderStatuses columns
            migrationBuilder.DropColumn(name: "ColorHex", table: "WorkOrderStatuses");
            migrationBuilder.DropColumn(name: "IsDefault", table: "WorkOrderStatuses");
            migrationBuilder.DropColumn(name: "SortOrder", table: "WorkOrderStatuses");
            migrationBuilder.DropColumn(name: "IsActive", table: "WorkOrderStatuses");

            // Drop new tables
            migrationBuilder.DropTable(name: "Rooms");
            migrationBuilder.DropTable(name: "PhonebookTypes");
            migrationBuilder.DropTable(name: "RoomTypes");
            migrationBuilder.DropTable(name: "Floors");
        }
    }
}
