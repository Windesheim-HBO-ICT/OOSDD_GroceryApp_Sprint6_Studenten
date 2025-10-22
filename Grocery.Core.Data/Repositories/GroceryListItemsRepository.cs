using Grocery.Core.Data;
using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        // Nodig om product info op te halen voor elk item
        private readonly IProductRepository _productRepository;

        public GroceryListItemsRepository(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            // Maak de tabel aan als die nog niet bestaat
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItems (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            [Amount] INTEGER NOT NULL,
                            FOREIGN KEY (GroceryListId) REFERENCES GroceryList(Id),
                            FOREIGN KEY (ProductId) REFERENCES Products(Id))");

            // Voeg test data toe (alleen als het item nog niet bestaat)
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

            // SELECT query om alle items op te halen
            string query = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";

            using (SqliteCommand command = new(query, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                // Loop door alle rijen heen
                while (reader.Read())
                {
                    // Haal data uit de database kolommen
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);

                    // Maak een nieuw GroceryListItem object
                    GroceryListItem item = new(id, groceryListId, productId, amount);

                    // Haal het bijbehorende product op en koppel het
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

        // Haal alleen items op die bij een specifieke boodschappenlijst horen
        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> items = new();

            OpenConnection();

            // WHERE clause filtert op de juiste boodschappenlijst
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

                    // Koppel product informatie aan het item
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

        // Haalt 1 specifiek item op via zijn Id
        public GroceryListItem? Get(int id)
        {
            GroceryListItem? item = null;

            OpenConnection();

            // WHERE Id = ... om het juiste item te vinden
            string query = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE Id = {id}";

            using (SqliteCommand command = new(query, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                // Gebruik Read() (niet while, want er woord maar v maar 1 resultaat verwacht)
                if (reader.Read())
                {
                    int itemId = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);

                    item = new(itemId, groceryListId, productId, amount);

                    // Koppelt product informatie
                    var product = _productRepository.Get(productId);
                    if (product != null)
                    {
                        item.Product = product;
                    }
                }
            }

            CloseConnection();

            return item; // Kan null zijn als item niet bestaat
        }

        // Voeg een nieuw item toe aan de database
        public GroceryListItem Add(GroceryListItem item)
        {
            OpenConnection();

            // INSERT query met parameters (@) om SQL injection te voorkomen
            string query = $"INSERT INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(@GroceryListId, @ProductId, @Amount) RETURNING RowId;";

            using (SqliteCommand command = new(query, Connection))
            {
                // Bind de waardes aan de parameters
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);

                // ExecuteScalar geeft de nieuwe Id terug (RETURNING RowId)
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }

            CloseConnection();

            return item; // Geeft item terug met de nieuwe Id
        }

        // Werk een bestaand item bij in de database
        public GroceryListItem? Update(GroceryListItem item)
        {
            OpenConnection();

            // UPDATE query met parameters, filtert op Id
            string query = $"UPDATE GroceryListItems SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount WHERE Id = {item.Id};";

            using (SqliteCommand command = new(query, Connection))
            {
                // Bind nieuwe waardes aan de parameters
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.Amount);

                // Voer de UPDATE uit
                command.ExecuteNonQuery();
            }

            CloseConnection();

            return item; // Geeft het bijgewerkte item terug
        }

        // Verwijder een item uit de database
        public GroceryListItem? Delete(GroceryListItem item)
        {
            OpenConnection();

            // DELETE query om het item te verwijderen
            string query = $"DELETE FROM GroceryListItems WHERE Id = {item.Id};";

            Connection.ExecuteNonQuery(query);

            CloseConnection();

            return item; // Geeft het verwijderde item terug (ter bevestiging)
        }
    }
}