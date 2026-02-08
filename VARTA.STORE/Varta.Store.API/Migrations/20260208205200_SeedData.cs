using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Varta.Store.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Будівельні набори" },
                    { 2, "Запчастини" },
                    { 3, "Інше" },
                    { 4, "Фурнітура" },
                    { 5, "Набори для ремонту" }
                });

            migrationBuilder.InsertData(
                table: "OrderStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Очікування" },
                    { 2, "Оплачено" },
                    { 3, "Видано" }
                });

            migrationBuilder.InsertData(
                table: "ServerTag",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Chernarus 3PP" });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price", "ServerTagId" },
                values: new object[,]
                {
                    { 1, 1, "Код. лок, точильний камінь, лопата, молоток, 4 колоди, топорик, 20 дошок, плоскогубці, пилка, металевий дріт, door kit, пачка цв'яхів", "small_kit.png", "Маленький буд. набір", 120m, 1 },
                    { 2, 1, "3 код. лока, точильний камінь, лопата, молоток, 12 колод, топорик, 60 дошок, плоскогубці, 2 пилки, 3 металевих дрота, door kit, 3 пачки цв'яхів", "medium_kit.png", "Середній буд. набір", 250m, 1 },
                    { 3, 1, "5 код. локів, 2 бочки, 2 точильних камня, 2 лопати, 2 молотка, 20 колод, 2 топорика, 100 дошок, 2 плоскогубців, 2 пилки, 5 металевих дротів, door kit, 5 пачки цв'яхів", "large_kit.png", "Великий буд. набір", 400m, 1 },
                    { 4, 1, "32 каменя, 10 колод, упаковка цвяхів, мотузка, металевий дріт, молоток, кувалда, кирка, прапор 'DayZ'", "flag_kit.png", "Набір для флагштоку", 150m, 1 },
                    { 5, 2, "Автомобільний радіатор", "radiator.png", "Радіатор", 50m, 1 },
                    { 6, 2, "Для бензинових двигунів", "sparkplug.png", "Свічка Запалювання", 50m, 1 },
                    { 7, 2, "Для дизельних двигунів", "glowplug.png", "Свічка Розжарення", 50m, 1 },
                    { 8, 2, "Вантажний акумулятор", "battery_truck.png", "Акумулятор до V3S", 60m, 1 },
                    { 9, 2, "Автомобільний акумулятор", "battery_car.png", "Акумулятор 12В", 50m, 1 },
                    { 10, 2, "Запасне колесо", "wheel_gunter.png", "Колесо до Gunter 2", 40m, 1 },
                    { 11, 2, "Запасне колесо", "wheel_m1024.png", "Колесо до M1024", 40m, 1 },
                    { 12, 2, "Вантажне колесо", "wheel_v3s_front.png", "Переднє Колесо до V3S", 40m, 1 },
                    { 13, 2, "Вантажне колесо (подвійне)", "wheel_v3s_rear.png", "Заднє Колесо до V3S", 50m, 1 },
                    { 14, 2, "Запасне колесо", "wheel_olga.png", "Колесо до Olga24", 40m, 1 },
                    { 15, 2, "Запасне колесо", "wheel_sarka.png", "Колесо до Sarka120", 40m, 1 },
                    { 16, 2, "Запасне колесо", "wheel_ada.png", "Колесо до Ada4x4", 40m, 1 },
                    { 17, 2, "Ремонтний матеріал", "epoxy.png", "Епоксидна смола", 40m, 1 },
                    { 18, 2, "Повна каністра", "jerrycan.png", "Каністра з бензином", 25m, 1 },
                    { 19, 2, "Елемент живлення", "battery_9v.png", "Батарейка 9v", 10m, 1 },
                    { 20, 2, "Інструмент для ремонту", "blowtorch.png", "Паяльна лампа", 30m, 1 },
                    { 21, 3, "30 днів. 69 слотів + мисливський ніж + банка консервованих бобів + банка квасу", "black_set.png", "Чорний сет", 150m, 1 },
                    { 22, 3, "30 днів. 69 слотів + мисливський ніж + банка консервованих бобів + банка квасу", "green_set.png", "Зелений сет", 150m, 1 },
                    { 23, 4, "Для зберігання речей", "barrel.png", "Бочка", 60m, 1 },
                    { 24, 4, "Дерев'яний ящик", "crate.png", "Ящик", 15m, 1 },
                    { 25, 4, "Великий намет", "mil_tent.png", "Військова палатка", 50m, 1 },
                    { 26, 4, "Середній намет", "camping_tent.png", "Туристична палатка", 30m, 1 },
                    { 27, 4, "Sea Chest", "seachest.png", "Скриня", 25m, 1 },
                    { 28, 4, "10 слотів під зброю", "gunrack.png", "Стійка для зброї", 80m, 1 },
                    { 29, 5, "Weapon Cleaning Kit", "cleaning_kit.png", "Набір для чистки зброї", 50m, 1 },
                    { 30, 5, "Sewing Kit", "sewing_kit.png", "Швейний набір", 20m, 1 },
                    { 31, 5, "Leather Sewing Kit", "leather_kit.png", "Набір інструментів скорняку", 20m, 1 },
                    { 32, 5, "Tire Repair Kit", "tire_kit.png", "Набір для ремонту шин", 20m, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServerTag",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
