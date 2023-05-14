using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.Database;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers;

public class PurchaseController : Controller
{
    private readonly IUnitOfWork _uow;

    public PurchaseController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [Route("purchase")]
    public IActionResult Index()
    {
        var user = _uow
            .UserRepository
            .Get(x => x.Email == User.Identity.Name)
            .FirstOrDefault();

        var carts = _uow
            .ShoppingCartRepository
            .Get(x => x.UserId == user.Id, includeProperties: "Product")
            .ToList();

        var total = carts.Sum(x => x.Product.Price * x.Count);
        var model = new PurchaseViewModel
        {
            Cart = carts,
            TotalCost = total,
            Name = user.Name,
            Surname = user.Surname
        };

        return View(model);
    }

    public IActionResult Make(PurchaseViewModel purchase)
    {
        purchase.Surname = purchase.Surname ?? " ";
        purchase.Name = purchase.Name ?? " ";
        purchase.City = purchase.City ?? " ";
        purchase.Street = purchase.Street ?? " ";
        purchase.House = purchase.House ?? " ";
        purchase.Apartment = purchase.Apartment ?? " ";
        
        var office = Request.Form["offices"].ToString();
        var delivery = Request.Form["options"];

        var adress = purchase.Surname +
                     " " + purchase.Name +
                     " " + purchase.City +
                     " " + purchase.Street +
                     " " + purchase.House +
                     " " + purchase.Apartment +
                     " " + office +
                     " " + delivery;

        var costOffice = office switch
        {
            "Meest" => 20,
            "New Post" => 35,
            _ => 15
        };

        var deliveryCost = delivery == "On home" ? 10 : 0;
        var user = _uow.UserRepository.Get(x => x.Email == User.Identity.Name).FirstOrDefault();

        var carts = _uow.ShoppingCartRepository
            .Get(x => x.UserId == user.Id, includeProperties: "Product")
            .ToArray();
        
        var total = carts.Sum(x => x.Product.Price * x.Count);
        total += costOffice + deliveryCost;
        var np = new Purchase
        {
            UserId = user.Id,
            Address = adress,
            FullPrice = total,
            CreationTime = DateTime.Now
        };
        
        _uow.PurchaseRepository.Insert(np);
        _uow.Save();
        
        foreach (var i in carts)
        {
            var npr = new PurchaseProduct
            {
                PurchaseId = np.Id,
                ProductId = i.ProductId,
                Count = i.Count
            };
            _uow.PurchaseProductRepository.Insert(npr);
            _uow
                .ShoppingCartRepository
                .Delete(i);
            _uow.Save();
        }

        _uow.Save();
        var success = new SuccessViewModel { TotalPrice = total };
        return View("Success", success);
    }
}