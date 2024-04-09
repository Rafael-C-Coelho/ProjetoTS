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
            string connectionString = GetConnectionString();
            if (!File.Exists(connectionString))
            {
                SQLiteConnection.CreateFile(connectionString);
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS `users`(
                                `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
                                `username` VARCHAR(255) NOT NULL,
                                `password` VARCHAR(255) NOT NULL,
                                `public_key` VARCHAR(255) NOT NULL
                            );
                            CREATE TABLE IF NOT EXISTS `messages`(
                                `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
                                `message` BIGINT NOT NULL,
                                `sender` BIGINT NOT NULL,
                                `recipient` BIGINT NOT NULL,
                                `sent_at` TIMESTAMP NOT NULL
                            );
                            CREATE TABLE IF NOT EXISTS `token`(
                                `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
                                `token` VARCHAR(255) NOT NULL,
                                `user` BIGINT UNSIGNED NOT NULL
                            );
                            ALTER TABLE
                                `messages` ADD INDEX `messages_sender_index`(`sender`);
                            ALTER TABLE
                                `messages` ADD INDEX `messages_recipient_index`(`recipient`);
                            ALTER TABLE
                                `token` ADD UNIQUE `token_token_unique`(`token`);
                            ALTER TABLE
                                `token` ADD CONSTRAINT `token_user_foreign` FOREIGN KEY(`user`) REFERENCES `users`(`id`);
                            ALTER TABLE
                                `messages` ADD CONSTRAINT `messages_recipient_foreign` FOREIGN KEY(`recipient`) REFERENCES `users`(`id`);
                            ALTER TABLE
                                `token` ADD CONSTRAINT `token_user_foreign` FOREIGN KEY(`user`) REFERENCES `users`(`id`);
                            ALTER TABLE
                                `messages` ADD CONSTRAINT `messages_sender_foreign` FOREIGN KEY(`sender`) REFERENCES `users`(`id`);
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
            int username_id = GetUserId(username);
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `public_key` FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username_id);
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
                        SELECT `message`, `sender` FROM `messages`
                        WHERE `recipient` = @recipient;
                    ";
                    command.Parameters.AddWithValue("@recipient", recipient_id);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add($"{reader.GetString(0)},{GetUsername(int.Parse(reader.GetString(1)))}");
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

        public string GetToken(string token)
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
                            user = GetUsername(int.Parse(reader.GetString(0)));
                        }
                    }
                }
            }
            return user;
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
                    command.Parameters.AddWithValue("@username", GetUserId(username));
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
                            user = GetUsername(int.Parse(reader.GetString(0)));
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
    }
}
