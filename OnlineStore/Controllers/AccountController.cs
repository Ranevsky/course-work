using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.Database;
using OnlineStore.Models.ViewModel;
using OnlineStore.Services;

namespace OnlineStore.Controllers;

public class AccountController : Controller
{
    private readonly IUnitOfWork _uow;

    public AccountController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpPost]
    private async Task Authenticate(User user)
    {
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _uow.UserRepository
                .Get(x => x.Email == model.Email && x.PasswordHash == PasswordConverter.Hash(model.Password),
                    includeProperties: "Role").FirstOrDefault();
            if (user != null)
            {
                await Authenticate(user);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Uncorrect email or password input");
        }

        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _uow.UserRepository.Get(x => x.Email == model.Email).FirstOrDefault();
            if (user == null)
            {
                user = new User
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    PasswordHash = PasswordConverter.Hash(model.Password),
                    CreationTime = DateTime.Now
                };
                var userRole = _uow.UserRoleRepository.Get(x => x.Name == "SimpleUser").FirstOrDefault();
                if (userRole != null)
                {
                    user.Role = userRole;
                }

                _uow.UserRepository.Insert(user);
                _uow.Save();
                await Authenticate(user);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "The same account already exist");
        }

        return View(model);
    }

    // Todo: watch
    // public IActionResult Purchases()
    // {
    //     return View();
    //
    // }
    [Route("account/info")]
    public IActionResult Info()
    {
        var identity = User.Identity;
        if (identity is null || identity.IsAuthenticated == false)
        {
            // Todo: maybe error
            return RedirectToAction("Index", "Home");
        }

        var user = _uow
            .UserRepository
            .Get(x => x.Email == User.Identity.Name, includeProperties: "Role")
            .FirstOrDefault();

        if (user is null)
        {
            // Todo: maybe error
            return RedirectToAction("Index", "Home");
        }
        
        var isModer = user.Role.Name == "Moderator";
        var isAdmin = user.Role.Name == "Administrator";

        var model = new AccountInfoViewModel
        {
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            CreationTime = user.CreationTime,
            IsModerator = isModer,
            IsAdmin = isAdmin
        };

        return View(model);
    }

    [Authorize]
    public IActionResult UserPurchases()
    {
        var email = User.Identity!.Name;

        var user = _uow
            .UserRepository
            .Get(x => x.Email == email)
            .FirstOrDefault();

        var purchases = _uow
            .PurchaseRepository
            .Get(x => x.UserId == user.Id, includeProperties: "PurchaseProducts.Product")
            .OrderByDescending(x => x.CreationTime);

        var model = new UserPurchasesViewModel
        {
            Purchases = purchases
        };

        return View(model);
    }
}