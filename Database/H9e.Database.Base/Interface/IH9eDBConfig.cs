using System.Data.Common;

namespace H9e.Database.Base {
    public interface IH9eDBConfig {

        bool UseCharset { get; }
        string DefaultEngine { get; }
        string DefaultCharset { get; }
        string DefaultCollation { get; }

        string ServerHost { get; }
        int ServerPort { get; }
        string Username { get; }
        string Password { get; }
        string Database { get; }

        bool IsConnectionValid(DbConnection connection);

        DbConnection CreateNewConnection();

    }
}
