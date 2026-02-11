using Microsoft.EntityFrameworkCore;
using Varta.Store.Shared;

namespace Varta.Store.API.Data
{
    public class StoreDbContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProductHistory> OrderProductHistory { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ServerTag> ServerTags { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Будівельні набори" },
                new Category { Id = 2, Name = "Запчастини" },
                new Category { Id = 3, Name = "Інше" },
                new Category { Id = 4, Name = "Фурнітура" },
                new Category { Id = 5, Name = "Набори для ремонту" }
            );

            // seed server tags
            modelBuilder.Entity<ServerTag>().HasData(
                new ServerTag { Id = 1, Name = "Chernarus 3PP", IpAddress = "178.150.251.247:2307" }
            );

            // seed order statuses
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, Name = "Очікування" },
                new OrderStatus { Id = 2, Name = "Оплачено" },
                new OrderStatus { Id = 3, Name = "Видано" }
            );

            // seed products
            modelBuilder.Entity<Product>().HasData(
                // Будівельні набори (Id: 1)
                new Product
                {
                    Id = 1,
                    Name = "Маленький буд. набір",
                    Price = 120,
                    CategoryId = 1,
                    ServerTagId = 1,
                    ImageUrl = "small_kit.png",
                    Description = "Код. лок, точильний камінь, лопата, молоток, 4 колоди, топорик, 20 дошок, плоскогубці, пилка, металевий дріт, door kit, пачка цв'яхів"
                },
                new Product
                {
                    Id = 2,
                    Name = "Середній буд. набір",
                    Price = 250,
                    CategoryId = 1,
                    ServerTagId = 1,
                    ImageUrl = "medium_kit.png",
                    Description = "3 код. лока, точильний камінь, лопата, молоток, 12 колод, топорик, 60 дошок, плоскогубці, 2 пилки, 3 металевих дрота, door kit, 3 пачки цв'яхів"
                },
                new Product
                {
                    Id = 3,
                    Name = "Великий буд. набір",
                    Price = 400,
                    CategoryId = 1,
                    ServerTagId = 1,
                    ImageUrl = "large_kit.png",
                    Description = "5 код. локів, 2 бочки, 2 точильних камня, 2 лопати, 2 молотка, 20 колод, 2 топорика, 100 дошок, 2 плоскогубців, 2 пилки, 5 металевих дротів, door kit, 5 пачки цв'яхів"
                },
                new Product
                {
                    Id = 4,
                    Name = "Набір для флагштоку",
                    Price = 150,
                    CategoryId = 1,
                    ServerTagId = 1,
                    ImageUrl = "flag_kit.png",
                    Description = "32 каменя, 10 колод, упаковка цвяхів, мотузка, металевий дріт, молоток, кувалда, кирка, прапор 'DayZ'"
                },

                // Запчастини (Id: 2)
                new Product { Id = 5, Name = "Радіатор", Price = 50, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/radiator.png", Description = "Автомобільний радіатор" },
                new Product { Id = 6, Name = "Свічка Запалювання", Price = 50, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/candleIgnition.png", Description = "Для бензинових двигунів" },
                new Product { Id = 7, Name = "Свічка Розжарення", Price = 50, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/candleIncandescence.png", Description = "Для дизельних двигунів" },
                new Product { Id = 8, Name = "Акумулятор до V3S", Price = 60, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/carBattery2.png", Description = "Вантажний акумулятор" },
                new Product { Id = 9, Name = "Акумулятор 12В", Price = 50, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/carBattery.png", Description = "Автомобільний акумулятор" },
                new Product { Id = 10, Name = "Колесо до Gunter 2", Price = 40, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/wheelGunter.png", Description = "Запасне колесо" },
                new Product { Id = 11, Name = "Колесо до M1024", Price = 40, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/wheelHammer.png", Description = "Запасне колесо" },
                new Product { Id = 12, Name = "Переднє Колесо до V3S", Price = 40, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/wheelV3S.png", Description = "Вантажне колесо" },
                new Product { Id = 13, Name = "Заднє Колесо до V3S", Price = 50, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/wheelV3S.png", Description = "Вантажне колесо (подвійне)" },
                new Product { Id = 14, Name = "Колесо до Olga24", Price = 40, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/wheelvOlga.png", Description = "Запасне колесо" },
                new Product { Id = 15, Name = "Колесо до Sarka120", Price = 40, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/wheelSarka.png", Description = "Запасне колесо" },
                new Product { Id = 16, Name = "Колесо до Ada4x4", Price = 40, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/wheelGunter.png", Description = "Запасне колесо (тимчасово Gunter)" }, // Assuming no Ada wheel image found, using placeholder or check list again. Wait, Ada is Niva. I listed files. I did not see 'wheelAda.png'. I saw 'wheelGunter', 'wheelHammer', 'wheelSarka', 'wheelV3S', 'wheelvOlga'. I will use Gunter or leave old one? Old one was 'wheel_ada.png'. New list doesn't have it. I'll use Gunter as placeholder or maybe 'wheelSarka'. Let's use Gunter.
                new Product { Id = 17, Name = "Епоксидна смола", Price = 40, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/epoxy.png", Description = "Ремонтний матеріал" },
                new Product { Id = 18, Name = "Каністра з бензином", Price = 25, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/canister.png", Description = "Повна каністра" },
                new Product { Id = 19, Name = "Батарейка 9v", Price = 10, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/battery 9V.png", Description = "Елемент живлення" },
                new Product { Id = 20, Name = "Паяльна лампа", Price = 30, CategoryId = 2, ServerTagId = 1, ImageUrl = "transport/blowtorch.png", Description = "Інструмент для ремонту" },

                // Інше (Id: 3)
                new Product
                {
                    Id = 21,
                    Name = "Чорний сет",
                    Price = 150,
                    CategoryId = 3,
                    ServerTagId = 1,
                    ImageUrl = "black_set.png",
                    Description = "30 днів. 69 слотів + мисливський ніж + банка консервованих бобів + банка квасу"
                },
                new Product
                {
                    Id = 22,
                    Name = "Зелений сет",
                    Price = 150,
                    CategoryId = 3,
                    ServerTagId = 1,
                    ImageUrl = "green_set.png",
                    Description = "30 днів. 69 слотів + мисливський ніж + банка консервованих бобів + банка квасу"
                },

                // Фурнітура (Id: 4)
                new Product { Id = 23, Name = "Бочка", Price = 60, CategoryId = 4, ServerTagId = 1, ImageUrl = "barrel.png", Description = "Для зберігання речей" },
                new Product { Id = 24, Name = "Ящик", Price = 15, CategoryId = 4, ServerTagId = 1, ImageUrl = "crate.png", Description = "Дерев'яний ящик" },
                new Product { Id = 25, Name = "Військова палатка", Price = 50, CategoryId = 4, ServerTagId = 1, ImageUrl = "mil_tent.png", Description = "Великий намет" },
                new Product { Id = 26, Name = "Туристична палатка", Price = 30, CategoryId = 4, ServerTagId = 1, ImageUrl = "camping_tent.png", Description = "Середній намет" },
                new Product { Id = 27, Name = "Скриня", Price = 25, CategoryId = 4, ServerTagId = 1, ImageUrl = "seachest.png", Description = "Sea Chest" },
                new Product { Id = 28, Name = "Стійка для зброї", Price = 80, CategoryId = 4, ServerTagId = 1, ImageUrl = "gunrack.png", Description = "10 слотів під зброю" },

                // Набори для ремонту (Id: 5)
                new Product { Id = 29, Name = "Набір для чистки зброї", Price = 50, CategoryId = 5, ServerTagId = 1, ImageUrl = "cleaning_kit.png", Description = "Weapon Cleaning Kit" },
                new Product { Id = 30, Name = "Швейний набір", Price = 20, CategoryId = 5, ServerTagId = 1, ImageUrl = "sewing_kit.png", Description = "Sewing Kit" },
                new Product { Id = 31, Name = "Набір інструментів скорняку", Price = 20, CategoryId = 5, ServerTagId = 1, ImageUrl = "leather_kit.png", Description = "Leather Sewing Kit" },
                new Product { Id = 32, Name = "Набір для ремонту шин", Price = 20, CategoryId = 5, ServerTagId = 1, ImageUrl = "tire_kit.png", Description = "Tire Repair Kit" }
            );
        }
    }
}