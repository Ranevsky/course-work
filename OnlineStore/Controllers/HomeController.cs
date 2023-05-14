using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _uow;

    public HomeController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Index(
        [FromQuery(Name = "page")] int page,
        [FromQuery(Name = "sort")] string sort,
        [FromQuery(Name = "filter")] string filter)
    {
        ViewData["Categories"] = _uow.CategoryRepository.Get();
        var products = _uow.ProductRepository.Get(includeProperties: "Image,Comments,Rates,Category");
        ViewData["prod"] = products;
        return View(products);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}