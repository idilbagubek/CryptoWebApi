using Microsoft.EntityFrameworkCore;

namespace CryptoApi.Data
{
    public class CryptoDbContext : DbContext
    {
        public DbSet<Crypto> Cryptos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CryptoDb");
        }
        public class Crypto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Wallet
        {
            public int Id { get; set; }
            public int Price { get; set; }
            public int TotalPrice { get; set; }
            public int Quantity { get; set; }
            public DateTime DateTime { get; set; }
            public int CryptoId { get; set; }
            public int UserId { get; set; }
        }

    }
}
