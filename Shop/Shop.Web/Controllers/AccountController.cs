using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shop.Web.Data.Entities;
using Shop.Web.Helper;
using Shop.Web.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    public class AccountController : Controller
    {
        //readonly: solo se cambia el valor en el contructor
        private readonly IUserHelper userHelper;
        private readonly IConfiguration configuration;

        public AccountController(IUserHelper userHelper, IConfiguration configuration)
        {
            this.userHelper = userHelper;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index","Home");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return this.Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Login","Account");
                    
                }
            }

            this.ModelState.AddModelError(string.Empty, "User or password wrong");
            return this.View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await this.userHelper.LogoutAsync();
            return this.RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            return this.View();
        }   

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userHelper.GetUserByEmailAsync(model.UserName);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email= model.UserName,
                        UserName = model.UserName
                    };

                    var result = await this.userHelper.AddUserAsync(user,model.Password);
                    if (!result.Succeeded)
                    {
                        this.ModelState.AddModelError(string.Empty,"The user couldn't be created ");
                        return this.View(model);
                    }
                    else
                    {
                        this.RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "The username is already registered.");
                }
            }
            return this.View(model);
        }
        
        public async Task<IActionResult> ChangeUser()
        {
            var user = await this.userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();
            
            if(user!=null)
            {
                model.FirstName = user.FirstName;
                model.LastName= user.LastName;

            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if(this.ModelState.IsValid)
            {
                var user = await this.userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    var response = await this.userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        this.ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    var result = await this.userHelper.ChangePasswordAsync(user, model.OldPassword, model.Password);

                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return this.View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userHelper.GetUserByEmailAsync(model.Username);

                if (user != null)
                {
                    var result = await this.userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            this.configuration["Tokens:Issuser"],
                            this.configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials:credentials
                            );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }
            return this.BadRequest();
        }
    }
}
