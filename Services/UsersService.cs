using Microsoft.AspNetCore.Identity;
using budget_tracker.Models;
using Microsoft.AspNet.Identity;

namespace budget_tracker.Services
{
    public class UsersService : IUsersService
    {
        private readonly BudgetTrackerDbContext budgetTrackerDbContext;
        private readonly IConfiguration _configuration;
        PasswordHasher passwordHasher = new PasswordHasher();

        public UsersService(BudgetTrackerDbContext _budgetTrackerDbContext, IConfiguration configuration)
        {
            budgetTrackerDbContext = _budgetTrackerDbContext;
            _configuration = configuration;
        }

        //Users
        public bool CreateUser(CreateUser NewUserData)
        {
            Users user = new Users()
            {
                userName = NewUserData.userName,
                memberId = NewUserData.memberId,
                status = 1,
                createdBy = NewUserData.createdBy,
                dateOfCreation = DateTime.Now,
                password = passwordHasher.HashPassword("@Admin123")
            };

            try
            {
                var res = budgetTrackerDbContext.Add<Users>(user);
                budgetTrackerDbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public List<Users> GetUsers()
        {
            var Users = budgetTrackerDbContext.Users
                            .ToList();
            return Users;
        }

        public Users GetUserById(int userId)
        {
            var user = budgetTrackerDbContext.Users
                            .Where(u => u.userId == userId)
                            .FirstOrDefault();
            return user;
        }
        public Users GetUserByUsername(string username)
        {
            var user = budgetTrackerDbContext.Users
                            .Where(u => u.userName == username)
                            .FirstOrDefault();
            return user;
        }

        public Models.Result ChangePassword(ChangePassword changePassword)
        {
            Models.Result res = new Models.Result();
            try
            {
                var user = budgetTrackerDbContext.Users
                                .Where(u => u.userId == changePassword.userId)
                                .FirstOrDefault();
                user.password = passwordHasher.HashPassword(changePassword.newPassword);

                budgetTrackerDbContext.SaveChanges();

                res.status = true;
                res.message = "Passwored changed succesfully";
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = "An error occured. Please try again";
            }

            return res;
        }

        public Models.Result ChangeUserStatus(int userId)
        {
            Models.Result res = new Models.Result();
            try
            {
                var user = budgetTrackerDbContext.Users
                            .Where(u => u.userId == userId)
                            .SingleOrDefault();
                user.status = user.status == 0 ? 1 : 0;
                budgetTrackerDbContext.SaveChanges();

                res.status = true;
                res.message = user.status == 0 ? "User deactivated successfully." : "User activated successfully.";
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = "An error occured. Please try again";
            }

            return res;
        }

        public bool SaveAuthorizationToken(string token, Users user)
        {
            try
            {
                LoginHistory login_History = new LoginHistory()
                {
                    userId = user.userId,
                    token = token,
                    dateOfLogin = DateTime.Now
                };
                budgetTrackerDbContext.Add<LoginHistory>(login_History);
                budgetTrackerDbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
