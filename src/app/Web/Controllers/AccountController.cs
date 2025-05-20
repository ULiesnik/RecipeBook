using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using RecipeBook.Business.Models;
using RecipeBook.Business.Services;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [Route("signup")]
    public IActionResult SignUp()
    {
        return View();
    }

    [Route("signup")]
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpUserModel userModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.CreateUserAsync(userModel);
            if (result.Succeeded)
            {
                ViewBag.Success = true;
                return View("EmailSent");
            }
            foreach (var errorMessage in result.Errors)
            {
                ModelState.AddModelError("", errorMessage.Description);
            }
        }
        ModelState.Clear();
        return View(userModel);
    }

    [Route("login")]
    public IActionResult Login()
    {
        return View();
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserModel userModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.PasswordLoginAsync(userModel);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "invalid data");
            return View(userModel);
        }
        return View();
    }

    [Route("email-confirm")]
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string uid, string token)
    {
        if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
        {
            token = token.Replace(' ', '+');
            var result = await _accountService.ConfirmEmailAsync(uid, token);
            if (result.Succeeded)
            {
                ViewBag.Success = true;
            }
        }
        return View();
    }

    [Route("forgot-password")]
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [Route("forgot-password")]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
    {
        if (ModelState.IsValid)
        {
            await _accountService.SendForgotPasswordTokenAsync(forgotPasswordModel.Email);
            ModelState.Clear();
            ViewBag.EmailSent = true;
        }
        return View(forgotPasswordModel);
    }

    [Route("password-reset")]
    [HttpGet]
    public IActionResult ResetPassword(string uid, string token)
    {
        ResetPasswordModel model = new ResetPasswordModel
        {
            Token = token,
            UserId = uid
        };
        return View(model);
    }

    [AllowAnonymous]
    [Route("password-reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            model.Token = model.Token.Replace(' ', '+');
            var result = await _accountService.ResetPasswordAsync(model);
            if (result.Succeeded)
            {
                model.IsSuccess = true;
                return View(model);
            }
            foreach (var errorMessage in result.Errors)
            {
                ModelState.AddModelError("", errorMessage.Description);
            }
        }
        return View(model);
    }

    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}
