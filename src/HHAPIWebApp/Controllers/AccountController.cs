using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using HHAPIWebApp.Models;


namespace HHAPIWebApp.Controllers
{
    public class AccountController : Controller
    {
        //1
        private readonly UserManager<ApplicationUser> _securityManager;
        private readonly SignInManager<ApplicationUser> _loginManager;
        //2
        public AccountController(UserManager<ApplicationUser> secMgr, SignInManager<ApplicationUser> loginManager)
        {
            _securityManager = secMgr;
            _loginManager = loginManager;
        }

        //3
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //Добавление нового пользователя
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            string ErrorsStr = "";
            // Проверка на валидацию модели
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Token = model.Token
                };
                var result = await _securityManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _loginManager.SignInAsync(user, isPersistent: false);
                    return Json(new { isOk = true});
                }
                else
                {
                    foreach (var e in result.Errors) ErrorsStr = ErrorsStr + e.Description + ";"; 
                    return Json(new { isOk = false, Errors = ErrorsStr });
                }
            }
            // Получаем текстовое представление списка ошибок валидации, чтобы отправить на клиент
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    ErrorsStr = ErrorsStr + error.ErrorMessage + ";";
                }
            }
            return Json(new {isOk=false,Errors=ErrorsStr});
        }

        //5
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //6
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //ApplicationUser user = await _securityManager.FindByEmailAsync(model.Email);
                //if (user != null)
                //{
                    var result = await _loginManager.PasswordSignInAsync(model.Name, model.Password, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToReturnUrl(returnUrl);
                    }
                //}
            }


            return View(model);
        }

        //7
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _loginManager.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        //8
        private IActionResult RedirectToReturnUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var register = new Register();

            if (User.Identity.IsAuthenticated)
            {
                var user = _securityManager.FindByEmailAsync(User.Identity.Name).Result;
                register.Email = user.Email;
                register.Token = user.Token;
                register.UserId = user.UserId;

                ViewData["ShowUpdateStatus"] = false;
            
                
            }
            return View(register);
        }

        /// <summary>
        /// Обновление данных о пользователи в БД
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(string Token, string UserId)
        {
            string Email = User.Identity.Name;
            var user = _securityManager.FindByEmailAsync(Email).Result;
            if (user != null)
            {
                user.Token = Token;
                user.UserId = UserId;
                var result = await _securityManager.UpdateAsync(user);

                if (!result.Succeeded) {
                    return Json(new { Success = false, Errors = result.Errors });
                }
            }
            return Json(new { Success = true, Errors = "" });
        }
    }
    
}
