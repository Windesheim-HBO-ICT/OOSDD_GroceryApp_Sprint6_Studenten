using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        public GroceryListItemsRepository()
        {
            // Tabel voor lijstitems
            CreateTable(@"
                CREATE TABLE IF NOT EXISTS GroceryListItem (
                    [Id]             INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [GroceryListId]  INTEGER NOT NULL,
                    [ProductId]      INTEGER NOT NULL,
                    [Amount]         INTEGER NOT NULL,
                    FOREIGN KEY (GroceryListId) REFERENCES GroceryList(Id) ON DELETE CASCADE
                )");
        }

        public List<GroceryListItem> GetAll()
        {
            var result = new List<GroceryListItem>();

            const string sql = @"
                SELECT Id, GroceryListId, ProductId, Amount
                FROM GroceryListItem;";

            OpenConnection();
            using (var cmd = new SqliteCommand(sql, Connection))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int id = r.GetInt32(0);
                    int listId = r.GetInt32(1);
                    int productId = r.GetInt32(2);
                    int amount = r.GetInt32(3);

                    result.Add(new GroceryListItem(id, listId, productId, amount));
                }
            }
            CloseConnection();

            return result;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            var result = new List<GroceryListItem>();

            const string sql = @"
                SELECT Id, GroceryListId, ProductId, Amount
                FROM GroceryListItem
                WHERE GroceryListId = @ListId;";

            OpenConnection();
            using (var cmd = new SqliteCommand(sql, Connection))
            {
                cmd.Parameters.AddWithValue("@ListId", id);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int itemId = r.GetInt32(0);
                        int listId = r.GetInt32(1);
                        int productId = r.GetInt32(2);
                        int amount = r.GetInt32(3);

                        result.Add(new GroceryListItem(itemId, listId, productId, amount));
                    }
                }
            }
            CloseConnection();

            return result;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            const string insert = @"
                INSERT INTO GroceryListItem (GroceryListId, ProductId, Amount)
                VALUES (@ListId, @ProductId, @Amount)
                RETURNING ROWID;";

            OpenConnection();
            using (var cmd = new SqliteCommand(insert, Connection))
            {
                cmd.Parameters.AddWithValue("@ListId", item.GroceryListId);
                cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                cmd.Parameters.AddWithValue("@Amount", item.Amount);

                item.Id = System.Convert.ToInt32(cmd.ExecuteScalar());
            }
            CloseConnection();

            return item;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            const string delete = @"DELETE FROM GroceryListItem WHERE Id = @Id;";

            OpenConnection();
            using (var cmd = new SqliteCommand(delete, Connection))
            {
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.ExecuteNonQuery();
            }
            CloseConnection();

            return item;
        }

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? item = null;

            const string sql = @"
                SELECT Id, GroceryListId, ProductId, Amount
                FROM GroceryListItem
                WHERE Id = @Id;";

            OpenConnection();
            using (var cmd = new SqliteCommand(sql, Connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        int itemId = r.GetInt32(0);
                        int listId = r.GetInt32(1);
                        int productId = r.GetInt32(2);
                        int amount = r.GetInt32(3);

                        item = new GroceryListItem(itemId, listId, productId, amount);
                    }
                }
            }
            CloseConnection();

            return item;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            const string update = @"
                UPDATE GroceryListItem
                SET ProductId = @ProductId,
                    Amount    = @Amount
                WHERE Id = @Id;";

            OpenConnection();
            using (var cmd = new SqliteCommand(update, Connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                cmd.Parameters.AddWithValue("@Amount", item.Amount);
                cmd.Parameters.AddWithValue("@Id", item.Id);

                cmd.ExecuteNonQuery();
            }
            CloseConnection();

            return item;
        }
    }
}
