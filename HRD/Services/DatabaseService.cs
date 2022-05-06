using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace HRD.Services
{
    public class DatabaseService : IDatabaseService
    {
        public SQLiteConnection Connection;

        /// <summary>
        /// Establishes the connection to the database, re-establishes if down or disconnected
        /// </summary>
        /// <returns></returns>
        private async Task ConnectAsync()
        {
            // we're already connected
            if (this.Connection != null && this.Connection.State == System.Data.ConnectionState.Open) return;

            string connectionString = await File.ReadAllTextAsync("./connection_string.txt");
            this.Connection = new(connectionString.Trim());

            await this.Connection.OpenAsync();
        }

        /// <summary>
        /// Creates a new command to be used for interacting with the database
        /// </summary>
        /// <returns>The created command</returns>
        public async Task<SQLiteCommand> CreateCommandAsync()
        {
            await this.ConnectAsync();
            return new SQLiteCommand(this.Connection);
        }

        /// <summary>
        /// The last id of whatever was inserted last in the database
        /// </summary>
        public long LastInsertedId => this.Connection.LastInsertRowId;
    }
}
