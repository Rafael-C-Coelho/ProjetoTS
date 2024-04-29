using Server;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace ProjetoTS
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
                "client.sqlite"
            );
        }

        public static void CreateDatabase()
        {
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
                                privateKey TEXT NOT NULL,
                                publicKey TEXT NOT NULL,
                                authToken TEXT
                            );

                            CREATE TABLE IF NOT EXISTS messages (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                sender TEXT NOT NULL,
                                recipient TEXT NOT NULL,
                                content TEXT NOT NULL,
                                sent_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                FOREIGN KEY (sender) REFERENCES users(username),
                                FOREIGN KEY (recipient) REFERENCES users(username)
                            );
                        ";
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            CreateDatabase();
                        }
                    }
                }
            }
        }

        public void InsertUser(string username, string privateKey, string publicKey, string authToken)
        {
            try
            {
                string connectionString = GetConnectionString();
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                        INSERT INTO `users`(`username`, `privateKey`, `publicKey`, `authToken`)
                        VALUES(@username, @privateKey, @publicKey, @authToken);
                    ";
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@privateKey", privateKey);
                        command.Parameters.AddWithValue("@publicKey", publicKey);
                        command.Parameters.AddWithValue("@authToken", authToken);
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception ex)
            {
                // ADD LOGGER HERE @Rafael Coelho
            }
        }

        public string GetPrivateKey(string username)
        {
            string privateKey = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `privateKey` FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            privateKey = reader.GetString(0);
                        }
                    }
                }
            }
            return privateKey;
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
                        SELECT `publicKey` FROM `users`
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

        public string GetAuthToken(string username)
        {
            string authToken = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `authToken` FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            authToken = reader.GetString(0);
                        }
                    }
                }
            }
            return authToken;
        }

        public void UpdateAuthToken(string username, string newAuthToken)
        {
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        UPDATE `users`
                        SET `authToken` = @newAuthToken
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@newAuthToken", newAuthToken);
                    command.Parameters.AddWithValue("@username", username);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertMessage(string sender, string recipient, string content)
        {
            try
            {
                string connectionString = GetConnectionString();
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                        INSERT INTO `messages`(`sender`, `recipient`, `content`)
                        VALUES(@sender, @recipient, @content);
                    ";
                        command.Parameters.AddWithValue("@sender", sender);
                        command.Parameters.AddWithValue("@recipient", recipient);
                        command.Parameters.AddWithValue("@content", content);
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception ex)
            {
                // ADD LOGGER HERE @Rafael Coelho
            }
        }

        public List<string> GetMessages(string recipient)
        {
            List<string> messages = new List<string>();
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT `sender`, `content`, `sent_at` FROM `messages`
                        WHERE `recipient` = @recipient;
                    ";
                    command.Parameters.AddWithValue("@recipient", recipient);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add($"{reader.GetString(0)}: {reader.GetString(1)} (Sent at: {reader.GetDateTime(2)})");
                        }
                    }
                }
            }
            return messages;
        }
    }
}
