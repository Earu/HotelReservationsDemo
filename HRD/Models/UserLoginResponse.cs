namespace HRD.Models
{
    /// <summary>
    /// A login response, returned by the SignUp and SignIn methods
    /// </summary>
    public class UserLoginResponse
    {
        public string SessionToken { get; set; }
        public int UserId { get; set; }
    }
}
