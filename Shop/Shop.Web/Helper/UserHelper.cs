using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shop.Web.Data.Entities;

namespace Shop.Web.Helper
{
    public class UserHelper : IUserHelper
    {
        public readonly UserManager<User> userManager;

        public UserHelper(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        async Task<IdentityResult> IUserHelper.AddUserAsync(User user, string password)
        {
            return await this.userManager.CreateAsync(user,password);
        }

        async Task<User> IUserHelper.GetUserByEmailAsync(string email)
        {
            return await this.userManager.FindByEmailAsync(email);
        }
    }
}
