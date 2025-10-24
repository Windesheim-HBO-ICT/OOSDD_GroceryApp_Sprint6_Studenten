
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ClientRepository : DatabaseConnection, IClientRepository
    {
        private readonly List<Client> clientList = [];

        //public ClientRepository()
        //{
        //    Client admin = new(3, "A.J. Kwak", "user3@mail.com", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=");
        //    admin.Role = Role.Admin;
        //    clientList = [
        //        new Client(1, "M.J. Curie", "user1@mail.com", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08="),
        //        new Client(2, "H.H. Hermans", "user2@mail.com", "dOk+X+wt+MA9uIniRGKDFg==.QLvy72hdG8nWj1FyL75KoKeu4DUgu5B/HAHqTD2UFLU="),
        //        admin
        //    ];
        //}

        public Client? Get(string email)
        {
            this.GetAll();
            Client? client = clientList.FirstOrDefault(c => c.EmailAddress.Equals(email));
            return client;
        }

        public Client? Get(int id)
        {
            this.GetAll();
            Client? client = clientList.FirstOrDefault(c => c.Id == id);
            return client;
        }

        //public List<Client> GetAll()
        //{
        //    return clientList;
        //}



        public ClientRepository()
        {

            //ISO 8601 format: date.ToString("o", CultureInfo.InvariantCulture)
            CreateTable(@"CREATE TABLE IF NOT EXISTS Client (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] STRING UNIQUE NOT NULL,
                            [EmailAddress] STRING NOT NULL,
                            [Password] STRING NOT NULL,
                            [IsAdmin] INTEGER NOT NULL DEFAULT 0)");
            List<string> insertQueries = [@"INSERT OR IGNORE INTO Client(Name, EmailAddress, Password) VALUES('M.J. Curie', 'user1@mail.com', 'IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08=')",
                                          @"INSERT OR IGNORE INTO Client(Name, EmailAddress, Password) VALUES('H.H. Hermans', 'user2@mail.com', 'dOk+X+wt+MA9uIniRGKDFg==.QLvy72hdG8nWj1FyL75KoKeu4DUgu5B/HAHqTD2UFLU=')",
                                          @"INSERT OR IGNORE INTO Client(Name, EmailAddress, Password, IsAdmin) VALUES('A.J. Kwak', 'user3@mail.com', 'sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=', 1)"];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }

        public List<Client> GetAll()
        {
            clientList.Clear();
            string selectQuery = "SELECT Id, Name, EmailAddress, Password, IsAdmin FROM Client";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string email = reader.GetString(2);
                    string password = reader.GetString(3);
                    int isAdmin = reader.GetInt32(4);
                    Client client = new(id, name, email, password);
                    if (isAdmin == 1) client.Role = Role.Admin;

                    clientList.Add(client);
                }
            }
            CloseConnection();

            return clientList;
        }
    }
}
