using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories {
    public class ProductRepository : DatabaseConnection, IProductRepository {
        private readonly List<Product> products = [];

        public ProductRepository() {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Product (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(80) NOT NULL,
                            [Stock] INTEGER NOT NULL,
                            [ShelfLife] DATE,
                            [Price] DECIMAL(5,2) NOT NULL)");

            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(1, 'Melk', 10, '2024-12-31', 1.29)",
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(2, 'Brood', 5, '2024-12-10', 2.49)",
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(3, 'Appels', 20, '2024-12-20', 0.99)",
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(4, 'Kaas', 8, '2025-01-15', 3.79)",
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(5, 'Eieren', 15, '2024-12-25', 2.99)"
            ];

            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }

        public List<Product> GetAll() {
            products.Clear();
            string selectQuery = "SELECT Id, Name, Stock, date(ShelfLife), Price FROM Product";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection)) {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = reader.IsDBNull(3) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime(3));
                    decimal price = reader.IsDBNull(4) ? 0m : reader.GetDecimal(4);

                    products.Add(new(id, name, stock, shelfLife, price));
                }
            }
            CloseConnection();
            return products;
        }

        public Product? Get(int id) {
            string selectQuery = "SELECT Id, Name, Stock, date(ShelfLife), Price FROM Product WHERE Id = @Id";
            Product? product = null;

            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection)) {
                command.Parameters.AddWithValue("@Id", id);
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read()) {
                    int productId = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = reader.IsDBNull(3) ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime(3));
                    decimal price = reader.IsDBNull(4) ? 0m : reader.GetDecimal(4);

                    product = new(productId, name, stock, shelfLife, price);
                }
            }
            CloseConnection();
            return product;
        }

        public Product Add(Product item) {
            string insertQuery = "INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES(@Name, @Stock, @ShelfLife, @Price) RETURNING RowId;";

            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection)) {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Stock", item.Stock);
                command.Parameters.AddWithValue("@ShelfLife", item.ShelfLife.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@Price", item.Price);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();

            return item;
        }

        public Product? Delete(Product item) {
            string deleteQuery = "DELETE FROM Product WHERE Id = @Id;";

            OpenConnection();
            using (SqliteCommand command = new(deleteQuery, Connection)) {
                command.Parameters.AddWithValue("@Id", item.Id);
                command.ExecuteNonQuery();
            }
            CloseConnection();

            return item;
        }

        public Product? Update(Product item) {
            Product? product = products.FirstOrDefault(p => p.Id == item.Id);
            if (product == null)
                return null;
            product.Id = item.Id;
            return product;
        }
    }
}
