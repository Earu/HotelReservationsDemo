using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HRD.Services
{
    /// <summary>
    /// DISCLAIMER:
    /// "password" really needs to be a SecureString, and this probably shouldnt exist. This should use a known
    /// authentication framework/mechanism or OAuth with another system. Anyone could hijack this system
    /// anyway as a man-in-the-middle attack is possible since there are DNS and SSL certificates recorded for this
    /// making HTTPS impossible and therefore all communications perfectly readable for anyone with sensible tools.
    /// </summary>
    public class SessionService : ISessionService
    {
        private const string TOKEN_BASE = "123456789abcdefghijklmnoprstuvwxyz-:$%^&*_+-/";

        private readonly IDatabaseService Database;
        private readonly ILogger<SessionService> Logger;
        private readonly ConcurrentDictionary<string, int> Sessions;

        public SessionService(IDatabaseService database, ILogger<SessionService> logger)
        {
            this.Database = database;
            this.Logger = logger;
            this.Sessions = new ConcurrentDictionary<string, int>();
        }


        public async Task<string> SignIn(string userName, string password)
        {
            await this.Database.ConnectAsync();

            using (SqliteCommand command = this.Database.CreateCommand())
            {
                command.CommandText = "SELECT Hash, Id FROM Users WHERE Username = @username";
                command.Parameters.AddWithValue("@username", userName);

                using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows) return null; // no such username

                    await reader.ReadAsync();

                    string knownHash = reader.GetString(0);
                    string providedHash = this.HashPassword(password);

                    if (knownHash != providedHash) return null; // incorrect password

                    int userId = reader.GetInt32(1);
                    return this.GenerateToken(userId); // generate the session token, NOTE: These should probably expire
                }
            }
        }

        public async Task<string> SignUp(string userName, string password)
        {
            await this.Database.ConnectAsync();

            using (SqliteCommand command = this.Database.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (Username, Hash) VALUES (@username, @hash)";
                command.Parameters.AddWithValue("@username", userName);
                command.Parameters.AddWithValue("@hash", this.HashPassword(password));

                if (await command.ExecuteNonQueryAsync() < 1) 
                    return null; // failed to register in db
            }

            using (SqliteCommand command = this.Database.CreateCommand())
            {
                command.CommandText = "SELECT Id FROM Users WHERE Username = @username";
                command.Parameters.AddWithValue("@username", userName);

                int? userId = await command.ExecuteScalarAsync() as int?;
                if (!userId.HasValue) return null; // this should never happen but you never know...

                return this.GenerateToken(userId.Value);
            }
        }

        public bool TryGetUserSession(string token, out int userId)
        {
            if (this.Sessions.TryGetValue(token, out userId))
                return true;

            userId = -1;
            return false;
        }

        private string HashPassword(string password)
            => Encoding.UTF8.GetString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

        private string GenerateToken(int userId)
        {
            Random rand = new();
            StringBuilder tokenBuilder = new(32);
            for (int i = 0; i < 32; i++)
                tokenBuilder.Append(TOKEN_BASE[rand.Next(0, TOKEN_BASE.Length)]);

            string token = tokenBuilder.ToString();
            this.Sessions.TryAdd(token, userId); // it's actually fine not to handle this, multiple tokens for the same user are not a problem

            return token;
        }
    }
}
