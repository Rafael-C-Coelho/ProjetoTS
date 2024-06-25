using Server;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace ProjetoTS
{
	internal class DBHelper
	{
		private Logger logger = new Logger("DBHelper.log");
		public DBHelper()
		{
			CreateDatabase();
		}

		//caminho completo para a base de dados SQLite. O caminho inicia-se na  pasta 'ApplicationData' é o nome da pasta,
		//seguindo para a subpasta 'Porjeto TS' e por fim o nome do ficheiro da base de dados 'client.sqlite'
		public static string GetConnectionString()
		{
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"ProjetoTS",
				"client.sqlite"
			);

		}

		public static void CreateDatabase()
		{ //criação da base de dados com a inclusão das tabelas users e messages 
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

		//Inserir um novo registo de utilizador na base de dados e as respetivas chaves
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
						logger.Info($"Info: User {username} has been successfully entered");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred while assigning the keys to the username");
				logger.Exception(ex);
			}
		}

		//método para obter a chave privada de um utilizador específico através do seu username 
		public string GetPrivateKey(string username)
		{
			string privateKey = "";
			string connectionString = GetConnectionString();
			using (SQLiteConnection connection = new SQLiteConnection($"Data Source={connectionString};Version=3;"))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(connection))
				{
					logger.Info("Info: Starting to obtain the private key ");
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
					logger.Info("Info: Private key successfully obtained.");
				}

			}
			return privateKey;
		}

		//método para obter a chave privada de um utilizador específico através do seu username 
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
				logger.Info("Info: Public key successfully obtained.");
			}
			return publicKey;
		}

		// método responsável por atualizar o token de autenticação de um determinado utilizador na base de dados 
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
					logger.Info("Info: AuthToken successfully obtained");
				}
			}
			return authToken;
		}

		// método responsável por atualizar o token de autenticação de um determinado utilizador na base de dados 
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
				logger.Info("Info: AuthToken updated successfully");
			}
		}

		// método que possibilita o armazenamento de mensagens na base de dados, mais especificamente na tabela "messages"
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

					MessageBox.Show("Message inserted successfully");
					logger.Info("Info: Message inserted successfully");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occured while inserting a message");
				logger.Exception(ex);
			}
		}

		//método que obtem a lista de mensagens de um utilizador.
		//Para isso, é efetuada uma conexão com a base de dados, realizada uma consulta e selecionadas as mensagens,
		//sendo apresentadas com o utilizador remetente, o conteúdo e a data/hora de envio 
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
					logger.Info("Info: List of messages successfully obtained.");
				}
			}
			return messages;
		}

		public void DeleteUser(string username)
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
                         DELETE FROM users WHERE username = @username;
                    ";
						command.Parameters.AddWithValue("@username", username);
						command.ExecuteNonQuery();
						logger.Info($"Info: User {username} has been successfully destroyed");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred while assigning the keys to the username");
				logger.Exception(ex);
			}
		}
	}
}
