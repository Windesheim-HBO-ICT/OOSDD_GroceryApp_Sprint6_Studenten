using System.Globalization;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product> products = [];
        public ProductRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Product (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(80) UNIQUE NOT NULL,
                            [Stock] INTEGER NOT NULL,
                            [ShelfLife] DATE NOT NULL,
                            [Price] DECIMAL NOT NULL)");
            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Melk', 300, '2025-09-25', 0.95)",
                @"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Kaas', 100, '2025-09-30', 7.98)",
                @"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Brood', 400, '2025-09-12', 2.19)",
                @"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Cornflakes', 0, '2025-12-31', 1.48)"
            ];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }
        public List<Product> GetAll()
        {
            products.Clear();
            OpenConnection();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int stock = reader.GetInt32(2);
                        DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                        Decimal price = reader.GetDecimal(4);
                        products.Add(new Product(id, name, stock, shelfLife, price));
                    }
                }
            }
            return products;
        }

        public Product? Get(int id)
        {
            return products.FirstOrDefault(p => p.Id == id);
        }

        public Product Add(Product item)
        {
            OpenConnection();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Product (Name, Stock, ShelfLife, Price) VALUES (@name, @stock, @shelfLife, @price);";
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@stock", item.Stock);
                command.Parameters.AddWithValue("@shelfLife", item.ShelfLife.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@price", item.Price);

                command.ExecuteNonQuery();

                // Retrieve the inserted product from the database
                using (var getCommand = Connection.CreateCommand())
                {
                    getCommand.CommandText = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product ORDER BY Id DESC LIMIT 1";
                    using (var reader = getCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            int stock = reader.GetInt32(2);
                            DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                            decimal price = reader.GetDecimal(4);
                            var product = new Product(id, name, stock, shelfLife, price);
                            products.Add(product);
                            return product;
                        }
                    }
                }
            }
            return item;
        }

        public Product? Delete(Product item)
        {
            throw new NotImplementedException();
        }

        public Product? Update(Product item)
        {
            Product? product = products.FirstOrDefault(p => p.Id == item.Id);
            if (product == null) return null;
            product.Id = item.Id;
            return product;
        }
    }
}
