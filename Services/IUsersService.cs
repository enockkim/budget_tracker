using budget_tracker.Models;
using MySqlX.XDevAPI.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using budget_tracker.Models;

namespace budget_tracker.Services
{
    public interface IUsersService
    {
        //Users
        bool CreateUser(CreateUser NewUserData);
        List<Users> GetUsers();
        Users GetUserById(int userId);
        Users GetUserByUsername(string username);
        Models.Result ChangePassword(ChangePassword changePassword);
        Models.Result ChangeUserStatus(int userId);
        bool SaveAuthorizationToken(string token, Users user);
    }
}