using System.Data.SQLite;
using System.Threading.Tasks;

namespace HRD.Services
{
    public interface IDatabaseService
    {
        Task<SQLiteCommand> CreateCommandAsync();
        long LastInsertedId { get; }
    }
}
