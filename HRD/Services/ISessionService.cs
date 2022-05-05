using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace HRD.Services
{
    public interface ISessionService
    {
        Task<string> SignIn(string userName, string password);
        Task<string> SignUp(string userName, string password);
        bool TryGetUserSession(string token, out int userId);
    }
}
