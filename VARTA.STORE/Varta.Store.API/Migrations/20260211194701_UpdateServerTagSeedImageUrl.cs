using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varta.Store.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServerTagSeedImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServerTag",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "servers/server1.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServerTag",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: null);
        }
    }
}
