using System.ComponentModel.DataAnnotations;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace HospOps.Pages.Account;


[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;


    public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


    [BindProperty] public InputModel Input { get; set; } = new();
    public string? ReturnUrl { get; set; }


    public class InputModel
    {
        [Required, Display(Name = "Username")] public string UserName { get; set; } = string.Empty;
        [EmailAddress, Display(Name = "Email")] public string? Email { get; set; }
        [StringLength(100), Display(Name = "Display name")] public string? DisplayName { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), Display(Name = "Confirm password"), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }


    public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl ?? Url.Content("~/");


    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        if (!ModelState.IsValid) return Page();


        var user = new ApplicationUser
        {
            UserName = Input.UserName,
            Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email,
            DisplayName = string.IsNullOrWhiteSpace(Input.DisplayName) ? null : Input.DisplayName,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(ReturnUrl!);
        }
        foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
        return Page();
    }
}