using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetStore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ContentModerations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ReviewedAt" },
                values: new object[] { new DateTime(2025, 6, 21, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(6586), new DateTime(2025, 6, 22, 0, 25, 9, 911, DateTimeKind.Utc).AddTicks(6588) });

            migrationBuilder.UpdateData(
                table: "ContentModerations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 22, 6, 25, 9, 911, DateTimeKind.Utc).AddTicks(6594));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 22, 7, 25, 9, 911, DateTimeKind.Utc).AddTicks(6633));

            migrationBuilder.UpdateData(
                table: "PetReports",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LostOrFoundDate" },
                values: new object[] { new DateTime(2025, 6, 20, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(6537), new DateTime(2025, 6, 20, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(6522) });

            migrationBuilder.UpdateData(
                table: "PetReports",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LostOrFoundDate" },
                values: new object[] { new DateTime(2025, 6, 21, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(6547), new DateTime(2025, 6, 21, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(6544) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 22, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(5822));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 22, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(5828));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 22, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(6298));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 22, 12, 25, 9, 911, DateTimeKind.Utc).AddTicks(6306));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ContentModerations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ReviewedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2287), new DateTime(2025, 6, 20, 19, 20, 46, 29, DateTimeKind.Utc).AddTicks(2288) });

            migrationBuilder.UpdateData(
                table: "ContentModerations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 21, 1, 20, 46, 29, DateTimeKind.Utc).AddTicks(2292));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 21, 2, 20, 46, 29, DateTimeKind.Utc).AddTicks(2309));

            migrationBuilder.UpdateData(
                table: "PetReports",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LostOrFoundDate" },
                values: new object[] { new DateTime(2025, 6, 19, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2261), new DateTime(2025, 6, 19, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2252) });

            migrationBuilder.UpdateData(
                table: "PetReports",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LostOrFoundDate" },
                values: new object[] { new DateTime(2025, 6, 20, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2266), new DateTime(2025, 6, 20, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2264) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(1764));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(1766));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2201));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 21, 7, 20, 46, 29, DateTimeKind.Utc).AddTicks(2204));
        }
    }
}
