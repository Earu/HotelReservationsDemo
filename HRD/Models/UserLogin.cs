namespace HRD.Models
{
    /// <summary>
    /// A login request, contains user's credentials
    /// </summary>
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
