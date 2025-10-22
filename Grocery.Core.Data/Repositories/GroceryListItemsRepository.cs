using Grocery.Core.Data;
using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        private readonly IProductRepository _productRepository;

        public GroceryListItemsRepository(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            // Maak de tabel aan
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItems (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            [Amount] INTEGER NOT NULL,
                            FOREIGN KEY (GroceryListId) REFERENCES GroceryList(Id),
                            FOREIGN KEY (ProductId) REFERENCES Products(Id))");

            // Voeg dummy data toe (optioneel - verwijder als je geen test data wilt)
            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(1, 1, 1, 2)",
                @"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(2, 1, 2, 3)",
                @"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(3, 2, 3, 1)"
            ];
            InsertMultipleWithTransaction(insertQueries);
        }

        // Haal alle items op uit de database
        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> items = new();

            OpenConnection();

            string query = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";

            using (SqliteCommand command = new(query, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);

                    GroceryListItem item = new(id, groceryListId, productId, amount);

                    // Haal het bijbehorende product op
                    var product = _productRepository.Get(productId);
                    if (product != null)
                    {
                        item.Product = product;
                    }

                    items.Add(item);
                }
            }

            CloseConnection();

            return items;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> items = new();

            OpenConnection();

            string query = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE GroceryListId = {groceryListId}";

            using (SqliteCommand command = new(query, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int listId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);

                    GroceryListItem item = new(id, listId, productId, amount);

                    // Haal het bijbehorende product op
                    var product = _productRepository.Get(productId);
                    if (product != null)
                    {
                        item.Product = product;
                    }

                    items.Add(item);
                }
            }

            CloseConnection();

            return items;
        }

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? item = null;

            OpenConnection();

            string query = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE Id = {id}";

            using (SqliteCommand command = new(query, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int itemId = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);

                    item = new(itemId, groceryListId, productId, amount);

                    // Haal het bijbehorende product op
                    var product = _productRepository.Get(productId);
                    if (product != null)
                    {
                        item.Product = product;
                    }
                }
            }

            CloseConnection();

            return item;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            OpenConnection();

            string query = $"INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(@GroceryListId, @ProductId, @Amount) RETURNING RowId;";

            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }

            CloseConnection();

            return item;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            OpenConnection();

            string query = $"UPDATE GroceryListItems SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount WHERE Id = {item.Id};";

            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);

                command.ExecuteNonQuery();
            }

            CloseConnection();

            return item;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            OpenConnection();

            string query = $"DELETE FROM GroceryListItems WHERE Id = {item.Id};";

            Connection.ExecuteNonQuery(query);

            CloseConnection();

            return item;
        }
    }
}