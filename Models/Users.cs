using System.ComponentModel.DataAnnotations;

namespace budget_tracker.Models
{
    public class Users
    {
        #region Properties
        [Key]
        public int userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public int memberId { get; set; }
        public int status { get; set; }
        public DateTime dateOfCreation { get; set; }
        public int createdBy { get; set; }
        public string userRole { get; set; }
        #endregion    
    }

    public class CreateUser
    {
        public string userName { get; set; }
        public int memberId { get; set; }
        public int createdBy { get; set; }
    }

    public class UserLoginData
    {
        public string testUsername { get; set; }
        public string testPassword { get; set; }
    }
    public class Result
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
    public class LoginResult
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string token { get; set; }
    }
    public class ChangePassword
    {
        public int userId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }

    public class LoginHistory
    {
        [Key]
        public int recordId { get; set; }
        public int userId { get; set; }
        public string token { get; set; }
        public DateTime dateOfLogin { get; set; }
    }
}
