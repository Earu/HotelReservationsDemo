using Microsoft.Data.Sqlite;
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
            await this.Connection.OpenAsync();
        }

        public SqliteCommand CreateCommand()
            => this.Connection.CreateCommand();
    }
}
