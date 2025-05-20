using System.Threading.Tasks;
using RecipeBook.Business.Models;
using RecipeBook.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace RecipeBook.Business.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AccountService(UserManager<User> userManager, SignInManager<User> signInManager,
        IEmailService emailService, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel)
    {
        var user = new User()
        {
            Email = userModel.Email,
            UserName = userModel.UserName,
        };
        var result = await _userManager.CreateAsync(user, userModel.Password);
        if (!result.Succeeded)
        {
            return result;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        if (!string.IsNullOrEmpty(token))
        {
            await SendConfirmationEmailAsync(user, token);
        }
        return result;
    }

    public async Task<SignInResult> PasswordLoginAsync(LoginUserModel userModel)
    {
        return await _signInManager.PasswordSignInAsync(userModel.UserName, userModel.Password, userModel.RememberMe, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    private async Task SendConfirmationEmailAsync(User user, string token)
    {
        string appDomain = _configuration.GetSection("Application:AppDomain").Value;
        string link = _configuration.GetSection("Application:EmailConfirmationPath").Value;
        var placeHolders = new List<KeyValuePair<string, string>>()
        {
            new ("{{UserName}}",user.UserName),
            new ("{{token}}", string.Format(appDomain + link, user.Id, token))
        };
        await _emailService.SendEmail("RecipeBook email confirmation", user.Email,
            "SignUpEmailTemplate", placeHolders);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
    {
        return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
    }

    private async Task SendResetPasswordEmailAsync(User user, string token)
    {
        string appDomain = _configuration.GetSection("Application:AppDomain").Value;
        string link = _configuration.GetSection("Application:PasswordResetPath").Value;
        var placeHolders = new List<KeyValuePair<string, string>>()
        {
            new ("{{UserName}}",user.UserName),
            new ("{{token}}", string.Format(appDomain + link, user.Id, token))
        };
        await _emailService.SendEmail("RecipeBook password reset", user.Email,
            "ForgotPasswordTemplate", placeHolders);
    }

    public async Task SendForgotPasswordTokenAsync(string email)
    {
        User user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendResetPasswordEmailAsync(user, token);
            }
        }
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
    {
        return await _userManager.ResetPasswordAsync(
            await _userManager.FindByIdAsync(model.UserId), model.Token, model.NewPassword);
    }
}
