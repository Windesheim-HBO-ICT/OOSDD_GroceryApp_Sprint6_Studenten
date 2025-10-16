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
                [GroceryListId] INTERGER NOT NULL,
                [ProductId] INTERGER NOT NULL,
                [Amount] INTERGER NOT NULL)");
            List<string> insertQueries = [@"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 1, 3)",
                                          @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 2, 1)",
                                          @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 3, 4)",
                                          @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2, 1, 2)",
                                          @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2, 2, 5)"];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }

        public List<GroceryListItem> GetAll()
        {
            groceryListItems.Clear();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection)) {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    groceryListItems.Add(new(id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id) {
            List<GroceryListItem> allOnId = new();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE groceryListId = @id";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection)) {
                command.Parameters.AddWithValue("@id", id);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    int Id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    allOnId.Add(new(Id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return allOnId;
        }

        public GroceryListItem Add(GroceryListItem item) {
            int rowsAffected;
            string inserQuery = $"INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(@groceryListId, @productId, @amount) Returning RowId;";
            OpenConnection();
            using (SqliteCommand command = new(inserQuery, Connection)) {
                command.Parameters.AddWithValue("@groceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@productId", item.ProductId);
                command.Parameters.AddWithValue("@amount", item.Amount);
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return Get(item.Id) ?? item;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            string deleteQuery = $"DELETE FROM GroceryListItems WHERE Id = {item.Id};";
            OpenConnection();
            Connection.ExecuteNonQuery(deleteQuery);
            CloseConnection();
            return item;
        }

        public GroceryListItem? Get(int id)
        {
            string selectQuery = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE Id = {id}";
            GroceryListItem? gli = null;
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection)) {
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read()) {
                    int Id = reader.GetInt32(0);
                    int GroceryListId = reader.GetInt32(1);
                    int ProductId = reader.GetInt32(2);
                    int Amount = reader.GetInt32(3);
                    gli = new(Id, GroceryListId, ProductId, Amount);
                }
            }
            CloseConnection();
            return gli;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            int recordsAffected;
            string updateQuery = $"UPDATE GroceryListItems SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount  WHERE Id = {item.Id};";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection)) {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);

                recordsAffected = command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }
    }
}
