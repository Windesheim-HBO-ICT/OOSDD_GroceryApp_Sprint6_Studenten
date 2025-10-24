using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ProductCategoryRepository : DatabaseConnection, IProductCategoryRepository
    {
        private readonly List<ProductCategory> productCategories = [];

        public ProductCategoryRepository()
        {

            //ISO 8601 format: date.ToString("o", CultureInfo.InvariantCulture)
            CreateTable(@"CREATE TABLE IF NOT EXISTS ProductCategory (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [CategoryId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            UNIQUE (CategoryId, ProductId))");
            List<string> insertQueries = [@"INSERT OR IGNORE INTO ProductCategory(CategoryId, ProductId) VALUES(3, 1)",
                                          @"INSERT OR IGNORE INTO ProductCategory(CategoryId, ProductId) VALUES(3, 2)",
                                          @"INSERT OR IGNORE INTO ProductCategory(CategoryId, ProductId) VALUES(2, 3)",
                                          @"INSERT OR IGNORE INTO ProductCategory(CategoryId, ProductId) VALUES(5, 4)"];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }

        public List<Models.ProductCategory> GetAll()
        {
            productCategories.Clear();
            string selectQuery = "SELECT Id, CategoryId, ProductId FROM ProductCategory";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int categoryId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    productCategories.Add(new(id, categoryId, productId));
                }
            }
            CloseConnection();

            return productCategories;
        }

        public Models.ProductCategory Add(Models.ProductCategory item)
        {
            int recordsAffected;
            string insertQuery = $"INSERT INTO ProductCategory(CategoryId, ProductId) VALUES(@CategoryId, @ProductId) Returning RowId;";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("CategoryId", item.CategoryId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);

                //recordsAffected = command.ExecuteNonQuery();
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }
    }
}
