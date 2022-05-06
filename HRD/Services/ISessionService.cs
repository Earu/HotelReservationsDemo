using HRD.Models;
using System.Threading.Tasks;

namespace HRD.Services
{
    public interface ISessionService
    {
        Task<UserLoginResponse> SignIn(string userName, string password);
        Task<UserLoginResponse> SignUp(string userName, string password);
        bool TryGetUserSession(string token, out int userId);
    }
}
