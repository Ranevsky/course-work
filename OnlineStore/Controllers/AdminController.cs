using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers;

public class AdminController : Controller
{
    private readonly IUnitOfWork _uow;

    public AdminController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [Route("admin/products")]
    public IActionResult AdminProducts()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = _uow
                .UserRepository
                .Get(x => x.Email == User.Identity.Name, includeProperties: "Role")
                .FirstOrDefault();
            if (user.Role.Name != "Administrator")
            {
                return RedirectToAction("Index", "Home");
            }

            var products = _uow.ProductRepository.Get(includeProperties: "CreatorUser").ToList();

            var model = new AdminProducts
            {
                Products = products
            };
            return View("Products", model);
        }

        return RedirectToAction("Index", "Home");
    }

    [Route("admin/users")]
    public IActionResult AdminUsers()
    {
        var users = _uow.UserRepository.Get(includeProperties: "Role").ToList();

        var index = users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
        users.Remove(index);

        var admins = users.Where(x => x.Role.Name == "Administrator").ToList();
        var moderators = users.Where(x => x.Role.Name == "Moderator").ToList();
        var simple = users.Where(x => x.Role.Name == "SimpleUser").ToList();

        var model = new AdminUsers
        {
            Users = simple,
            Admins = admins,
            Moderators = moderators
        };
        return View("Users", model);
    }
}