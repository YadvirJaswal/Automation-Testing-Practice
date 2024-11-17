

namespace AutomationPracticeSiteProject.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public LoginModel(string userName, string password)
        {
            Username = userName;
            Password = password;
        }
        public static object[] validLoginDetails =
        [
            new object[]
            {
                new LoginModel("sumit@gmail.com","Sumit@001")
            }
        ];
    }
}
