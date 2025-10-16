using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product> products = [];
        public ProductRepository()
        {

            //ISO 8601 format: date.ToString("o", CultureInfo.InvariantCulture)
            CreateTable(@"CREATE TABLE IF NOT EXISTS Product (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(80) UNIQUE NOT NULL,
                            [Stock] INTEGER NOT NULL,
                            [ShelfLife] DATE NOT NULL,
                            [Price] DECIMAL NOT NULL)");
            List<string> insertQueries = [@"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Melk', 300, '2025-09-25', 0.95)",
                                          @"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Kaas', 100, '2025-09-30', 7.98)",
                                          @"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Brood', 400, '2025-09-12', 2.19)",
                                          @"INSERT OR IGNORE INTO Product(Name, Stock, ShelfLife, Price) VALUES('Cornflakes', 0, '2025-12-31', 1.48)"];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }
        public List<Product> GetAll()
        {
            products.Clear();
            string selectQuery = "SELECT Id, Name, Stock, date(ShelfLife), Price FROM Product";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    decimal price = reader.GetDecimal(4);
                    products.Add(new(id, name, stock, shelfLife, price));
                }
            }
            CloseConnection();

            return products;
        }

        public Product? Get(int id)
        {
            this.GetAll();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public Product Add(Product item)
        {
            int recordsAffected;
            string insertQuery = $"INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES(@Name, @Stock, @ShelfLife, @Price);";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", item.Name);
                command.Parameters.AddWithValue("Stock", item.Stock);
                command.Parameters.AddWithValue("ShelfLife", item.ShelfLife.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("Price", item.Price);

                command.ExecuteNonQuery();
                using (var getCommand = Connection.CreateCommand())
                {
                    getCommand.CommandText = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product ORDER BY Id DESC LIMIT 1";
                    using (var reader = getCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var product = new Product(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), DateOnly.FromDateTime(reader.GetDateTime(3)), reader.GetDecimal(4));
                            this.GetAll();
                            item = product;
                        }
                    }
                }
            }
            CloseConnection();
            return item;
        }

        public Product? Delete(Product item)
        {
            string deleteQuery = $"DELETE FROM Product WHERE Id = {item.Id};";
            OpenConnection();
            Connection.ExecuteNonQuery(deleteQuery);
            CloseConnection();
            return item;
        }

        public Product? Update(Product item)
        {

            int recordsAffected;
            string updateQuery = $"UPDATE Product SET Name = @Name, Stock = @Stock, ShelfLife = @ShelfLife, Price = @Price  WHERE Id = {item.Id};";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", item.Name);
                command.Parameters.AddWithValue("Stock", item.Stock);
                command.Parameters.AddWithValue("ShelfLife", item.ShelfLife);
                command.Parameters.AddWithValue("Price", item.Price);

                recordsAffected = command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }
    }
}
