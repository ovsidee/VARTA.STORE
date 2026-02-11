using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varta.Store.API.Migrations
{
    /// <inheritdoc />
    public partial class AddServerTagImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ServerTag",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ServerTag",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ServerTag");
        }
    }
}
