using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        private readonly List<GroceryListItem> groceryListItems = [];

        public GroceryListItemsRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItems (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            [Amount] INTEGER NOT NULL,
                            Unique(GroceryListId, ProductId)
                )");
            List<string> insertQueries = [
                @"INSERT OR REPLACE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 1, 3)",
                @"INSERT OR REPLACE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 2, 1)",
                @"INSERT OR REPLACE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 3, 4)",
                @"INSERT OR REPLACE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2, 1, 2)",
                @"INSERT OR REPLACE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2, 2, 5)"
            ];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }

        public List<GroceryListItem> GetAll()
        {
            groceryListItems.Clear();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    groceryListItems.Add(new GroceryListItem(id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            List<GroceryListItem> items = new();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE GroceryListId = @GroceryListId";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@GroceryListId", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int itemId = reader.GetInt32(0);
                        int groceryListId = reader.GetInt32(1);
                        int productId = reader.GetInt32(2);
                        int amount = reader.GetInt32(3);
                        items.Add(new GroceryListItem(itemId, groceryListId, productId, amount));
                    }
                }
            }
            CloseConnection();
            return items;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            string insertQuery = @"INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) VALUES (@GroceryListId, @ProductId, @Amount); SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@ProductId", item.ProductId);
                command.Parameters.AddWithValue("@Amount", item.amount);
                long newId = (long)command.ExecuteScalar();
                item.Id = (int)newId;
            }
            CloseConnection();
            return Get(item.Id)!;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? item = null;
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE Id = @Id";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int itemId = reader.GetInt32(0);
                        int groceryListId = reader.GetInt32(1);
                        int productId = reader.GetInt32(2);
                        int amount = reader.GetInt32(3);
                        item = new GroceryListItem(itemId, groceryListId, productId, amount);
                    }
                }
            }
            CloseConnection();
            return item;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            string updateQuery = @"UPDATE GroceryListItems SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount WHERE Id = @Id";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("@GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@ProductId", item.ProductId);
                command.Parameters.AddWithValue("@Amount", item.amount);
                command.Parameters.AddWithValue("@Id", item.Id);
                int affectedRows = command.ExecuteNonQuery();
            }
            CloseConnection();
            return Get(item.Id);
        }
    }
}
