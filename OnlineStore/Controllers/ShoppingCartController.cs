using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IUnitOfWork _uow;

    public ShoppingCartController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [Route("/cart")]
    public IActionResult Index(int? id, int? count, int? deleted)
    {
        var identity = User.Identity;
        if (identity is null || identity.IsAuthenticated == false)
        {
            return RedirectToActionPermanent("Index", "Home");
        }

        if (deleted != null && deleted != -1)
        {
            var del = _uow
                .ShoppingCartRepository
                .Get(x => x.ProductId == deleted && x.User.Email == User.Identity.Name, includeProperties: "User")
                .FirstOrDefault();
            _uow.ShoppingCartRepository.Delete(del);
            _uow.Save();
        }

        if (id != null && count != null)
        {
            var cart = _uow
                .ShoppingCartRepository
                .Get(x => x.ProductId == id && x.User.Email == User.Identity.Name)
                .FirstOrDefault();
            if (cart != null)
            {
                cart.Count = (int)count;
                _uow.ShoppingCartRepository.Update(cart);
            }
        }

        var carts = _uow
            .ShoppingCartRepository
            .Get(x => x.User.Email == User.Identity.Name, includeProperties: "User,Product")
            .ToList();
        var total = carts.Sum(x => x.Product.Price * x.Count);
        var model = new ShoppingCartModel
        {
            ShoppingCarts = carts,
            TotalPrice = total
        };
        return View(model);

    }
}