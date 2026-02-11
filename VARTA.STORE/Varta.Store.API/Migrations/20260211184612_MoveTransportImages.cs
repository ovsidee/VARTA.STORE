using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varta.Store.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveTransportImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "transport/radiator.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "transport/candleIgnition.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "transport/candleIncandescence.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "transport/carBattery2.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "transport/carBattery.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "transport/wheelGunter.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 11,
                column: "ImageUrl",
                value: "transport/wheelHammer.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImageUrl",
                value: "transport/wheelV3S.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImageUrl",
                value: "transport/wheelV3S.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImageUrl",
                value: "transport/wheelvOlga.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImageUrl",
                value: "transport/wheelSarka.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImageUrl",
                value: "transport/wheelGunter.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImageUrl",
                value: "transport/epoxy.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 18,
                column: "ImageUrl",
                value: "transport/canister.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 19,
                column: "ImageUrl",
                value: "transport/battery 9V.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 20,
                column: "ImageUrl",
                value: "transport/blowtorch.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "products/transport/radiator.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "products/transport/candleIgnition.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "products/transport/candleIncandescence.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "products/transport/carBattery2.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "products/transport/carBattery.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "products/transport/wheelGunter.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 11,
                column: "ImageUrl",
                value: "products/transport/wheelHammer.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImageUrl",
                value: "products/transport/wheelV3S.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImageUrl",
                value: "products/transport/wheelV3S.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImageUrl",
                value: "products/transport/wheelvOlga.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImageUrl",
                value: "products/transport/wheelSarka.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImageUrl",
                value: "products/transport/wheelGunter.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImageUrl",
                value: "products/transport/epoxy.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 18,
                column: "ImageUrl",
                value: "products/transport/canister.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 19,
                column: "ImageUrl",
                value: "products/transport/battery 9V.png");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 20,
                column: "ImageUrl",
                value: "products/transport/blowtorch.png");
        }
    }
}
