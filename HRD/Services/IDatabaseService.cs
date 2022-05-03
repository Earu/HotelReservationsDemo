using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace HRD.Services
{
    public interface IDatabaseService
    {
        Task ConnectAsync();
        SqliteCommand CreateCommand();
    }
}
