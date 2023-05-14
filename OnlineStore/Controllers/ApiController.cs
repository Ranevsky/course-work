using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.Database;

namespace OnlineStore.Controllers;

[Route("api")]
public class ApiController : Controller
{
    private readonly IUnitOfWork _uow;

    public ApiController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [Route("cart/add/{id:int}")]
    public IActionResult AddToShop(
        int id,
        [FromQuery(Name = "count")] int? count,
        [FromQuery(Name = "redirect")] string redirect)
    {
        var user = _uow.UserRepository.Get(x => x.Email == User.Identity.Name).FirstOrDefault();
        var product = _uow.ProductRepository.Get(x => x.Id == id).FirstOrDefault();
        count ??= 1;
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (product == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var cart = _uow
            .ShoppingCartRepository
            .Get(x => x.ProductId == product.Id && x.UserId == user.Id)
            .FirstOrDefault();
        
        if (cart != null)
        {
            cart.Count = (int)count;
            _uow
                .ShoppingCartRepository
                .Update(cart);
            
            _uow.Save();
        }
        else
        {
            cart = new ShoppingCart
            {
                UserId = user.Id,
                ProductId = product.Id,
                Count = (int)count
            };
            
            _uow
                .ShoppingCartRepository
                .Insert(cart);
            
            _uow.Save();
        }

        if (redirect != null)
        {
            return Redirect(redirect);
        }

        return RedirectToAction("Index", "Home");
    }

    [Route("cart/remove/{id:int}")]
    public IActionResult RemoveFromShop(int? id, [FromQuery(Name = "redirect")] string redirect)
    {
        var cart = _uow.ShoppingCartRepository
            .Get(x => x.User.Email == User.Identity.Name && x.ProductId == id, includeProperties: "User")
            .FirstOrDefault();
        if (cart != null)
        {
            _uow.ShoppingCartRepository.Delete(cart);
            _uow.Save();
        }

        if (redirect != null)
        {
            return Redirect(redirect);
        }

        return RedirectToAction("Index", "Home");
    }

    [Route("product/remove/{id:int}")]
    public IActionResult RemoveProduct(int? id, [FromQuery(Name = "redirect")] string redirect)
    {
        var user = _uow
            .UserRepository.Get(x => x.Email == User.Identity.Name, includeProperties: "Role")
            .FirstOrDefault();
        if (id != null && User.Identity.IsAuthenticated && user.Role.Name == "Administrator")
        {
            _uow.ProductRepository.Delete((int)id);
            _uow.Save();
        }

        return Redirect(redirect);
    }

    [Route("rate/{id:int}")]
    public IActionResult AddRate(
        int? id,
        [FromQuery(Name = "rate")] int? rate,
        [FromQuery(Name = "redirect")] string redirect)
    {
        if (id != null && rate != null)
        {
            var user = _uow.UserRepository.Get(x => x.Email == User.Identity.Name).FirstOrDefault();
            var r = _uow.RateRepository.Get(x => x.UserId == user.Id && x.ProductId == (int)id).FirstOrDefault();

            if (r != null)
            {
                r.Score = (int)rate;
                _uow.RateRepository.Update(r);
                _uow.Save();
            }
            else
            {
                r = new Rate { UserId = user.Id, ProductId = (int)id, Score = (int)rate };
                _uow.RateRepository.Insert(r);
                _uow.Save();
            }
        }

        return Redirect(redirect);
    }

    [Route("rate/remove/{id:int}")]
    public IActionResult RemoveRate(int? id, [FromQuery(Name = "redirect")] string redirect)
    {
        var user = _uow.UserRepository.Get(x => x.Email == User.Identity.Name).FirstOrDefault();
        if (user != null && id != null)
        {
            var rate = _uow.RateRepository.Get(x => x.UserId == user.Id && x.ProductId == (int)id).FirstOrDefault();
            _uow.RateRepository.Delete(rate);
            _uow.Save();
        }

        return Redirect(redirect);
    }

    [Route("comment/{id:int}")]
    public IActionResult AddComment(
        int? id,
        [FromQuery(Name = "comment")] string comment,
        [FromQuery(Name = "redirect")] string redirect)
    {
        var user = _uow.UserRepository.Get(x => x.Email == User.Identity.Name).FirstOrDefault();

        var comm = _uow
            .CommentRepository
            .Get(x => x.UserId == user.Id && x.ProductId == (int)id)
            .FirstOrDefault();

        if (comm == null)
        {
            comm = new Comment
            {
                UserId = user.Id,
                ProductId = (int)id,
                Text = comment
            };
            _uow.CommentRepository.Insert(comm);
        }
        else
        {
            comm.Text = comment;
            _uow.CommentRepository.Update(comm);
        }

        _uow.Save();
        return Redirect(redirect);
    }

    [Route("admin/user/{id:int}")]
    public IActionResult ChangeRole(
        int? id,
        [FromQuery(Name = "role")] string role,
        [FromQuery(Name = "redirect")] string redirect)
    {
        var user = _uow
            .UserRepository
            .Get(x => x.Id == id)
            .FirstOrDefault();

        var admin = _uow.UserRoleRepository.Get(x => x.Name == "Administrator").FirstOrDefault().Id;
        var moder = _uow.UserRoleRepository.Get(x => x.Name == "Moderator").FirstOrDefault().Id;
        var simple = _uow.UserRoleRepository.Get(x => x.Name == "SimpleUser").FirstOrDefault().Id;

        if (user != null)
        {
            if (role == "moderator")
            {
                user.RoleId = moder;
            }
            else if (role == "admin")
            {
                user.RoleId = admin;
            }
            else if (role == "simpleuser")
            {
                user.RoleId = simple;
            }

            _uow.UserRepository.Update(user);
            _uow.Save();
        }

        return Redirect(redirect);
    }
}