﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class DBHelper
    {
        private Logger logger = new Logger("DBHelper.log");
        //construtor responsável por instanciar a classe DBhelper que vai criar a base de dados com o método "CreateDatabase()"
        public DBHelper()
        {
            CreateDatabase();
        }

        // caminho/ligação entre a base de dados e o projeto 
        public static string GetConnectionString()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ProjetoTS",
                "database.sqlite"
            );
        }

        //criação da base de dados e das respetivas tabelas necessárias (users,messages, token, directory) para armazenar os dados provenientes dos utilizadores 
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

        public void InsertUser(string username, string password, string publicKey)
        {
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    // Gerar um salt aleatório
                    byte[] salt = new byte[16];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(salt);
                    }

                    // Gerar o hash
                    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000); 
                    byte[] hash = pbkdf2.GetBytes(20);

                    // Combinar o salt e o hash para armazenamento
                    byte[] hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);
                    string savedPasswordHash = Convert.ToBase64String(hashBytes);

                    command.CommandText = @"
                        INSERT INTO `users`(`username`, `password`, `public_key`)
                        VALUES(@username, @password, @public_key);
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", savedPasswordHash);
                    command.Parameters.AddWithValue("@public_key", publicKey);
                    command.ExecuteNonQuery();
                }
            }
            logger.Info("Info: User data entered successfully");
        }

        //introdução dos valores do utilizador na base de dados (username, password e chave pública)
        /*
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
            logger.Info("Info:User data entered successfully");
        }
        */

        //conexão e consulta da base de dados SQL lite para obtenção da chave pública que está associada ao nome do utilizador em questão e depois retorna essa chave 
        public string GetUserPublicKey(string username)
        {
            string publicKey = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info($" Info: Starting to obtain the publick key for {username}");
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

                    logger.Info($"Info: Public key successfully obtained for the user: {username} ");
                }
            }
            return publicKey;
        }

        //conexão e consulta da base SQL Lite de dados para selecionar a chave AES que está associada ao nome do utilizador em questão e depois retorna essa chave 
        public string GetUserAESKey(string username)
        {
            string aesKey = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info($" Info: Starting to obtain the AESkey for {username}");
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
            logger.Info("Info: User AESKEY successfully obtained");
            return aesKey;
        }

        //conexão à base de dados SQL Lite e realização de consulta para selecionar o ID do utilizador, da tabela users, retornando-o caso seja selecionado. Senão retorna uma string vazia  
        private int GetUserId(string username)
        {
            logger.Info("Info: Getting User Id...");
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
            logger.Info($"Info: User id successfully obtained for {username}.");
            return id;
        }

        //conexão à base de dados SQL Lite e consulta SQL do username do utilizador que está associado a um Id na tabela users. Após associar o nome ao id, retorna o username. 
        private string GetUsername(int id)
        {
            string username = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info("Info: Getting username...");
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
                logger.Info("Info: Username successfully obtained.");
            }
            return username;
        }

        //conexão à base de dados SQL Lite e realização de uma consulta SQL para obter a chave pública associada ao username de um utilizador presente na tabela users. Caso a associe, retorna a chave pública em questão
        public string GetPublicKey(string username)
        {
            string publicKey = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info("Info: Getting public key...");
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
                logger.Info("Info: Public key successfully obtained.");
            }
            return publicKey;
        }

        // listagem de todos os utilizadores que se encontram registados na base de dados SQL Lite, mais especificamente na tabela users 
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
            logger.Info("Info: Users sucessfully listed.");
            return users;
        }

        // listagem de todas mensagens que se encontram registadas na base de dados SQL Lite, mais especificamente na tabela messages 

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

        // conexão à base de dados SQL Lite para adicionar um novo registo, uma a mensagem na tabela mensagem,
        // sendo associada a um remetente e destinatário, bem como ao registo de data/hora do envio da mesma 
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

            logger.Info("Info: Message successfully inserted ");
        }

        // conexão à base de dados SQL Lite para adicionar o novo registo do token de acesso de um utilizador específico na tabela token 
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
            logger.Info("Info: Token successfully inserted ");
        }

        //Conexão à base de dados SQL Lite para eliminar o registo do token do utilizador da tabela Token 
        public void DeleteToken(string token)
        {
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info("Info: Deleting token...");
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

            logger.Info("Info: Token deleted sucessfully.");
        }

        //Conexão à base de dados SQL Lite para eliminar o registo do utilizador da tabela mensagens enquanto destinatário e remetente, da tabela token e users 
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
                    logger.Info("Info: User successfully deleted from messages table.");
                }
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        DELETE FROM `token`
                        WHERE `user` = @user;
                    ";
                    command.Parameters.AddWithValue("@user", usernameID);
                    command.ExecuteNonQuery();
                    logger.Info("Info: User successfully deleted from token table.");
                }
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        DELETE FROM `users`
                        WHERE `username` = @username;
                    ";
                    command.Parameters.AddWithValue("@username", username);
                    command.ExecuteNonQuery();
                    logger.Info("Info: User successfully deleted from users table");
                }
            }
        }

        // conexão à base de dados SQL Lite e uso do comando SQL para verificar se um utilizador se encontra registado com um determinado username e password na tabela users.
        // Como se trata de um bool, se o utilizador existir retronará verdadeiro, ou então falso se náo houver uma associação 
        /*
        public bool CheckUser(string username, string password)
        {
            bool exists = false;
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info("Info: User verification has started");
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    // Gerar um salt aleatório
                    byte[] salt = new byte[16];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(salt);
                    }

                    // Gerar o hash
                    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                    byte[] hash = pbkdf2.GetBytes(20);

                    // Combinar o salt e o hash para armazenamento
                    byte[] hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);
                    string savedPasswordHash = Convert.ToBase64String(hashBytes);

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

                    logger.Info("Info: User verification has finished");
                }
            }

            return exists;
        }
        */

        public bool CheckUser(string username, string password)
        {
            bool exists = false;
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info("Info: User verification has started");
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                SELECT `password` FROM `users`
                WHERE `username` = @username;
            ";
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Recupera o hash armazenado
                            string savedPasswordHash = reader.GetString(0);

                            // Extrai o salt e o hash armazenados
                            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                            byte[] salt = new byte[16];
                            Array.Copy(hashBytes, 0, salt, 0, 16);

                            // Gera o hash com a senha fornecida e o salt armazenado
                            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                            byte[] hash = pbkdf2.GetBytes(20);

                            // Compara o hash gerado com o hash armazenado
                            for (int i = 0; i < 20; i++)
                            {
                                if (hashBytes[i + 16] != hash[i])
                                {
                                    return false;
                                }
                            }
                            exists = true;
                        }
                    }
                }
                logger.Info("Info: User verification has finished");
            }

            return exists;
        }

        // neste método é estabelecidade uma conexão com a base de dados SQL Lite e efetuada uma consulta com o comando SQL para se obter o Id de um utilizador associado ao token correspondente na tabela token.
        // Se o token for encontrado, é retornado o nome do utilizador associado ao token. Se não for encontrado, é lançada uma exceção e apresentada a mensagem "Invalid token".
        public string GetUserFromToken(string token)
        {
            string user = "";
            string connectionString = GetConnectionString();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
            {
                logger.Info("Info: Getting user from token...");
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
                logger.Warn($"Warn: Invalid token: {token}");
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
