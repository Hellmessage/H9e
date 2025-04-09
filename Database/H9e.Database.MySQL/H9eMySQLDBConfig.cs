using H9e.Database.Base;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace H9e.Database.MySQL {
    public class H9eMySQLDBConfig : H9eDBConfig {

        public H9eMySQLDBConfig(string serverHost, string username, string password, string database) {
            ServerHost = serverHost;
            ServerPort = 3306;
            Username = username;
            Password = password;
            Database = database;
        }

        public H9eMySQLDBConfig(string serverHost, int serverPort, string username, string password, string database) {
            ServerHost = serverHost;
            ServerPort = serverPort;
            Username = username;
            Password = password;
            Database = database;
        }

        public void SetCharset(string charset) {
            DefaultCharset = charset;
            UseCharset = true;
        }
        public void SetCollation(string collation) {
            DefaultCollation = collation;
            UseCharset = true;
        }
        public void SetEngine(string engine) {
            DefaultEngine = engine;
        }

        public string ConnectionString {
            get {
                return $"server={ServerHost};user id={Username};password={Password};database={Database}";
            }
        }

        public override DbConnection CreateNewConnection() {
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public override bool IsConnectionValid(DbConnection connection) {
            try {
                using (var command = new MySqlCommand("SELECT 1", connection as MySqlConnection)) {
                    command.ExecuteScalar();
                    return true;
                }
            } catch {
                return false;
            }
        }
    }
}
