namespace AuthenticationMicroservice.Model
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
