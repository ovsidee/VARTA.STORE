using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varta.Store.API.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServerTag",
                keyColumn: "Id",
                keyValue: 1,
                column: "IpAddress",
                value: "178.150.251.247:2307");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServerTag",
                keyColumn: "Id",
                keyValue: 1,
                column: "IpAddress",
                value: null);
        }
    }
}
