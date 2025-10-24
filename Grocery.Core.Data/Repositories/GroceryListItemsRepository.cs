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
                            [Amount] INTEGER NOT NULL)");

            List<string> insertQueries =
            [
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 1, 3)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 2, 1)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 3, 4)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2, 1, 2)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2, 2, 5)"
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

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            return GetAll().Where(item => item.GroceryListId == groceryListId).ToList();
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            string insertQuery = @"INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) 
                                   VALUES(@GroceryListId, @ProductId, @Amount) Returning RowId;";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
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
            GroceryListItem? listItem = null;
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int Id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    listItem = new GroceryListItem(Id, groceryListId, productId, amount);
                }
            }

            CloseConnection();
            return listItem;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            string updateQuery = $"UPDATE GroceryListItems SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount WHERE Id = {item.Id};";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }
    }
}
