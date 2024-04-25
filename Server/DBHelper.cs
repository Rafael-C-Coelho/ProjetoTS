using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class DBHelper
    {
        public DBHelper()
        {
            CreateDatabase();
        }

        public static string GetConnectionString()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ProjetoTS",
                "database.sqlite"
            );
        }

        public static void CreateDatabase()
        {
            if (!Directory.Exists(Directory.GetParent(GetConnectionString()).ToString()))
                Directory.CreateDirectory(Directory.GetParent(GetConnectionString()).ToString());

            string connectionString = GetConnectionString();
            if (!Directory.Exists(Path.GetDirectoryName(connectionString)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(connectionString));
            }
            if (!File.Exists(connectionString))
            {
                SQLiteConnection.CreateFile(connectionString);
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS users (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                username TEXT NOT NULL UNIQUE,
                                password TEXT NOT NULL,
                                public_key TEXT NOT NULL
                            );

                            CREATE TABLE IF NOT EXISTS messages (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                message TEXT NOT NULL,
                                sender INTEGER NOT NULL,
                                recipient INTEGER NOT NULL,
                                sent_at TIMESTAMP NOT NULL,
                                FOREIGN KEY (sender) REFERENCES users(id),
                                FOREIGN KEY (recipient) REFERENCES users(id)
                            );

                            CREATE TABLE IF NOT EXISTS token (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                token TEXT NOT NULL,
                                user INTEGER NOT NULL,
                                UNIQUE (token),
                                FOREIGN KEY (user) REFERENCES users(id)
                            );

                            CREATE TABLE IF NOT EXISTS directory (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                dir_name INTEGER NOT NULL,
                                user INTEGER NOT NULL,
                                UNIQUE (token),
                                FOREIGN KEY (user) REFERENCES users(id)
                            );
                        ";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void InsertUser(string username, string password, string publicKey)
        {
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO `users`(`username`, `password`, `public_key`)
                        VALUES(@username, @password, @public_key);
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@public_key", publicKey);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetUserPublicKey(string username)
        {
            string publicKey = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `public_key` FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            publicKey = reader.GetString(0);
                        }
                    }
                }
            }
            return publicKey;
        }

        public string GetUserAESKey(string username)
        {
            string aesKey = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `aes_key` FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            aesKey = reader.GetString(0);
                        }
                    }
                }
            }
            return aesKey;
        }

        private int GetUserId(string username)
        {
            int id = -1;
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `id` FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = reader.GetInt32(0);
                        }
                    }
                }
            }
            return id;
        }

        private string GetUsername(int id)
        {
            string username = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `username` FROM `users`
                        WHERE `id` = @id;
                    ";
                    command.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            username = reader.GetString(0);
                        }
                    }
                }
            }
            return username;
        }

        public string GetPublicKey(string username)
        {
            string publicKey = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `public_key` FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            publicKey = reader.GetString(0);
                        }
                    }
                }
            }
            return publicKey;
        }

        public List<string> ListUsers()
        {
            List<string> users = new List<string>();
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `username` FROM `users`;
                    ";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return users;
        }

        public List<string> ListMessages(string recipient)
        {
            List<string> messages = new List<string>();
            string connectionString = GetConnectionString();
            int recipient_id = GetUserId(recipient);
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `sender`, `message` FROM `messages`
                        WHERE `recipient` = @recipient;
                    ";
                    command.Parameters.AddWithValue("@recipient", recipient_id);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add($"{GetUsername(reader.GetInt32(0))},{reader.GetString(1)}");
                        }
                    }
                }
            }
            return messages;
        }

        public void InsertMessage(string message, string sender, string recipient)
        {
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO `messages`(`message`, `sender`, `recipient`, `sent_at`)
                        VALUES(@message, @sender, @recipient, @sent_at);
                    ";
                    command.Parameters.AddWithValue("@message", message);
                    command.Parameters.AddWithValue("@sender", GetUserId(sender));
                    command.Parameters.AddWithValue("@recipient", GetUserId(recipient));
                    command.Parameters.AddWithValue("@sent_at", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertToken(string token, string user)
        {
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO `token`(`token`, `user`)
                        VALUES(@token, @user);
                    ";
                    command.Parameters.AddWithValue("@token", token);
                    command.Parameters.AddWithValue("@user", GetUserId(user));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteToken(string token)
        {
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        DELETE FROM `token`
                        WHERE `token` = @token;
                    ";
                    command.Parameters.AddWithValue("@token", token);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(string username)
        {
            int usernameID = GetUserId(username);
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        DELETE FROM `messages`
                        WHERE `recipient` = @recipient OR `sender` = @sender;
                    ";
                    command.Parameters.AddWithValue("@recipient", usernameID);
                    command.Parameters.AddWithValue("@sender", usernameID);
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        DELETE FROM `token`
                        WHERE `user` = @user;
                    ";
                    command.Parameters.AddWithValue("@user", usernameID);
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        DELETE FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool CheckUser(string username, string password)
        {
            bool exists = false;
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `id` FROM `users`
                        WHERE `username` = @username AND `password` = @password;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            exists = true;
                        }
                    }
                }
            }
            return exists;
        }

        public string GetUserFromToken(string token)
        {
            string user = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `user` FROM `token`
                        WHERE `token` = @token;
                    ";
                    command.Parameters.AddWithValue("@token", token);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = GetUsername(reader.GetInt32(0));
                        }
                    }
                }
            }
            if (user == "")
            {
                throw new Exception("Invalid token");
            }
            return user;
        }

        internal void InsertMessage(string username, string messageContent)
        {
            throw new NotImplementedException();
        }
    }
}
