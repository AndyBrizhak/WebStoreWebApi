using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels.Identity;

namespace WebStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _UserManager;
        private readonly SignInManager<User> _SignInManager;
        private readonly ILogger<AccountController> _Logger;

        public AccountController(UserManager<User> UserManager, SignInManager<User> SignInManager, ILogger<AccountController> Logger )
        {
            _UserManager = UserManager;
            _SignInManager = SignInManager;
            _Logger = Logger;
        }

        public async Task<IActionResult> IsNameFree(string UserName)
        {
            var user = await _UserManager.FindByNameAsync(UserName);
            return Json(user is null ? "true" : "Пользователь с таким именем уже существует");
        }

        #region Процесс регистрации нового пользвоателя

        public IActionResult Register() => View(new RegisterUserViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel Model)
        {
            if (!ModelState.IsValid) return View(Model);

            using (_Logger.BeginScope("Регистрация пользователя {0}", Model.UserName))
            {

                _Logger.LogInformation("Starting the new user registration process {0}", Model.UserName);

                var user = new User
                {
                    UserName = Model.UserName
                };

                var registration_result = await _UserManager.CreateAsync(user, Model.Password);
                if (registration_result.Succeeded)
                {
                    _Logger.LogInformation("User {0} registered successfully", user.UserName);

                    await _UserManager.AddToRoleAsync(user, Role.User);

                    _Logger.LogInformation("User {0} has been assigned role {1}", user.UserName, Role.User);

                    await _SignInManager.SignInAsync(user, false);

                    _Logger.LogInformation("User {0} is automatically logged in after registration", user.UserName);
                    return RedirectToAction("Index", "Home");
                }

                _Logger.LogWarning("Error while registering a new user {0}\r\n",
                    Model.UserName,
                    string.Join(Environment.NewLine, registration_result.Errors.Select(error => error.Description)));

                //_Logger.Log(LogLevel.Information, new EventId(5), registration_result, null, (result, _) => string.Join(Environment.NewLine, result.Errors.Select(error => error.Description)));

                foreach (var error in registration_result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(Model);
        }

        #endregion

        #region Процесс входа пользователя в систему

        public IActionResult Login(string ReturnUrl) => View(new LoginViewModel { ReturnUrl = ReturnUrl });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel Model)
        {
            if (!ModelState.IsValid) return View(Model);

            var login_result = await _SignInManager.PasswordSignInAsync(
                Model.UserName,
                Model.Password,
                Model.RememberMe,
                false);

            _Logger.LogInformation("Attempt to login user {0} into the system", Model.UserName);

            if (login_result.Succeeded)
            {
                _Logger.LogInformation("User {0} has successfully logged in", Model.UserName);

                if (Url.IsLocalUrl(Model.ReturnUrl))
                    return Redirect(Model.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }

            _Logger.LogWarning("Username or password error when trying to login {0}", Model.UserName);

            ModelState.AddModelError(string.Empty, "The username or password you entered is incorrect!");

            return View(Model);
        } 

        #endregion

        public async Task<IActionResult> Logout()
        {
            var user_name = User.Identity.Name;

            await _SignInManager.SignOutAsync();

            _Logger.LogInformation("User {0} is logged out", user_name);

            return RedirectToAction("Index", "Home");
        }

      
        public IActionResult AccessDenied() => View();
    }
}
