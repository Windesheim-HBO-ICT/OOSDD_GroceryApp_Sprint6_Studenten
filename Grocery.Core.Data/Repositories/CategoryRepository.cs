using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class CategoryRepository : DatabaseConnection, ICategoryRepository
    {
        private readonly List<Category> categories = [];

        public CategoryRepository()
        {

            //ISO 8601 format: date.ToString("o", CultureInfo.InvariantCulture)
            CreateTable(@"CREATE TABLE IF NOT EXISTS Category (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(80) UNIQUE NOT NULL)");
            List<string> insertQueries = [@"INSERT OR IGNORE INTO Category(Name) VALUES('Groente')",
                                          @"INSERT OR IGNORE INTO Category(Name) VALUES('Bakkerij')",
                                          @"INSERT OR IGNORE INTO Category(Name) VALUES('Zuivel')",
                                          @"INSERT OR IGNORE INTO Category(Name) VALUES('Conserven')",
                                          @"INSERT OR IGNORE INTO Category(Name) VALUES('Ontbijt')"];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }
        public List<Category> GetAll()
        {
            categories.Clear();
            string selectQuery = "SELECT Id, Name FROM Category";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    categories.Add(new(id, name));
                }
            }
            CloseConnection();

            return categories;
        }

        public Category? Get(int id)
        {
            this.GetAll();
            return categories.FirstOrDefault(p => p.Id == id);
        }
    }
}
