using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace HRD.Services
{
    public class DatabaseService : IDatabaseService
    {
        public SQLiteConnection Connection;

        private async Task ConnectAsync()
        {
            // we're already connected
            if (this.Connection != null && this.Connection.State == System.Data.ConnectionState.Open) return;

            string connectionString = await File.ReadAllTextAsync("./connection_string.txt");
            this.Connection = new(connectionString.Trim());

            await this.Connection.OpenAsync();
        }

        public async Task<SQLiteCommand> CreateCommandAsync()
        {
            await this.ConnectAsync();
            return new SQLiteCommand(this.Connection);
        }

        public long LastInsertedId => this.Connection.LastInsertRowId;
    }
}
