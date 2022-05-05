using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.IO;
using System.Threading.Tasks;

namespace HRD.Services
{
    public class DatabaseService : IDatabaseService
    {
        public SqliteConnection Connection;

        public async Task ConnectAsync()
        {
            // we're already connected
            if (this.Connection != null && this.Connection.State == System.Data.ConnectionState.Open) return;

            string connectionString = await File.ReadAllTextAsync("./connection_string.txt");
            this.Connection = new(connectionString.Trim());
            raw.SetProvider(new SQLite3Provider_e_sqlite3());


            await this.Connection.OpenAsync();
#if DEBUG
            await this.InitializeDbSchemeAsync();
#endif
        }

        private async Task InitializeDbSchemeAsync()
        {
            string sql = await File.ReadAllTextAsync("./db_scheme.sql");
            using (SqliteCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }

        public SqliteCommand CreateCommand()
            => this.Connection.CreateCommand();
    }
}
