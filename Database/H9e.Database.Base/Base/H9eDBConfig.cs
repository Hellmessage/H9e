using System.Data.Common;

namespace H9e.Database.Base {
    public abstract class H9eDBConfig : IH9eDBConfig {
        
        private bool useCharset = false;
        private string defaultCharset = "utf8mb4";
        private string defaultCollation = "utf8mb4_general_ci";
        private string defaultEngine = "InnoDB";

        public bool UseCharset { 
            get => useCharset; 
            protected set { useCharset = value; }
        }
        public string DefaultCharset {
            get => defaultCharset;
            protected set { defaultCharset = value; }
        }
        public string DefaultEngine {
            get => defaultEngine;
            protected set { defaultEngine = value; }
        }
        public string DefaultCollation {
            get => defaultCollation;
            protected set { defaultCollation = value; }
        }

        public string serverHost = "localhost";
        public int serverPort = 3306;
        private string username = "root";
        private string password = "root";
        private string database = "test";
        public string ServerHost {
            get => serverHost;
            protected set { serverHost = value; }
        }
        public int ServerPort {
            get => serverPort;
            protected set { serverPort = value; }
        }
        public string Username {
            get => username;
            protected set { username = value; }
        }
        public string Password {
            get => password;
            protected set { password = value; }
        }
        public string Database {
            get => database;
            protected set { database = value; }
        }

        public abstract DbConnection CreateNewConnection();

        public abstract bool IsConnectionValid(DbConnection connection);
    }
}
