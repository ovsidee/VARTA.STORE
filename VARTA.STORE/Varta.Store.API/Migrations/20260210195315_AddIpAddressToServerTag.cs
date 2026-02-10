using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varta.Store.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAddressToServerTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ServerTag",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ServerTag",
                keyColumn: "Id",
                keyValue: 1,
                column: "IpAddress",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ServerTag");
        }
    }
}
