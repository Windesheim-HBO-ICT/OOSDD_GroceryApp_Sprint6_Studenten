using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product> products = [];
        public ProductRepository()
        {
            // Create table if not exists
            CreateTable(@"CREATE TABLE IF NOT EXISTS Product (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(120) NOT NULL,
                            [Stock] INTEGER NOT NULL,
                            [ShelfLife] DATE NULL,
                            [Price] DECIMAL(10,2) NOT NULL DEFAULT 0
                        )");

            // Seed a few products if table is empty
            OpenConnection();
            using (var countCmd = new SqliteCommand("SELECT COUNT(1) FROM Product", Connection))
            {
                var count = Convert.ToInt32(countCmd.ExecuteScalar());
                if (count == 0)
                {
                    List<string> insertQueries = [
                        @"INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES('Melk', 300, '2025-09-25', 0.95)",
                        @"INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES('Kaas', 100, '2025-09-30', 7.98)",
                        @"INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES('Brood', 400, '2025-09-12', 2.19)",
                        @"INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES('Cornflakes', 0, '2025-12-31', 1.48)"
                    ];
                    InsertMultipleWithTransaction(insertQueries);
                }
            }
            CloseConnection();
            GetAll();
        }
        public List<Product> GetAll()
        {
            products.Clear();
            string selectQuery = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = default;
                    if (!reader.IsDBNull(3))
                    {
                        shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    }
                    decimal price = reader.GetDecimal(4);
                    products.Add(new(id, name, stock, shelfLife, price));
                }
            }
            CloseConnection();
            return products;
        }

        public Product? Get(int id)
        {
            string selectQuery = $"SELECT Id, Name, Stock, ShelfLife, Price FROM Product WHERE Id = {id}";
            Product? product = null;
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int pid = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = default;
                    if (!reader.IsDBNull(3))
                    {
                        shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    }
                    decimal price = reader.GetDecimal(4);
                    product = new(pid, name, stock, shelfLife, price);
                }
            }
            CloseConnection();
            return product;
        }

        public Product Add(Product item)
        {
            string insertQuery = $"INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES(@Name, @Stock, @ShelfLife, @Price) Returning RowId;";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", item.Name);
                command.Parameters.AddWithValue("Stock", item.Stock);
                if (item.ShelfLife == default)
                    command.Parameters.AddWithValue("ShelfLife", DBNull.Value);
                else
                    command.Parameters.AddWithValue("ShelfLife", item.ShelfLife);
                command.Parameters.AddWithValue("Price", item.Price);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            products.Add(item);
            return item;
        }

        public Product? Delete(Product item)
        {
            string deleteQuery = $"DELETE FROM Product WHERE Id = {item.Id};";
            OpenConnection();
            Connection.ExecuteNonQuery(deleteQuery);
            CloseConnection();
            var local = products.FirstOrDefault(p => p.Id == item.Id);
            if (local != null) products.Remove(local);
            return item;
        }

        public Product? Update(Product item)
        {
            string updateQuery = $"UPDATE Product SET Name = @Name, Stock = @Stock, ShelfLife = @ShelfLife, Price = @Price WHERE Id = {item.Id};";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", item.Name);
                command.Parameters.AddWithValue("Stock", item.Stock);
                if (item.ShelfLife == default)
                    command.Parameters.AddWithValue("ShelfLife", DBNull.Value);
                else
                    command.Parameters.AddWithValue("ShelfLife", item.ShelfLife);
                command.Parameters.AddWithValue("Price", item.Price);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            var local = products.FirstOrDefault(p => p.Id == item.Id);
            if (local != null)
            {
                local.Name = item.Name;
                local.Stock = item.Stock;
                local.ShelfLife = item.ShelfLife;
                local.Price = item.Price;
            }
            return item;
        }
    }
}
