﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers;

public class FilterController : Controller
{
    private readonly IUnitOfWork _uow;

    public FilterController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public IActionResult Index(
        int? category,
        float? lowprice,
        float? upprice,
        float? lowrate,
        string name,
        int page = 1,
        SortState sortOrder = SortState.NameAsc)
    {
        var pageSize = 4;
        var products = _uow.ProductRepository.Get(includeProperties: "Image,Rates,Category").AsQueryable();

        if (category != null && category != 0)
        {
            products = products.Where(x => x.Category.Id == category);
        }

        if (lowprice != null)
        {
            products = products.Where(x => x.Price > (decimal)lowprice);
        }

        if (upprice != null)
        {
            products = products.Where(x => x.Price < (decimal)upprice);
        }

        if (lowrate != null)
        {
            products = products.Where(x => x.AverageRate >= lowrate);
        }

        if (!string.IsNullOrEmpty(name))
        {
            products = products.Where(x => (x.Producer + x.Model + x.Description).ToLower().Contains(name.ToLower()));
        }

        switch (sortOrder)
        {
            case SortState.NameDesc:
                products = products.OrderByDescending(x => x.Producer + x.Model);
                break;
            case SortState.PriceAsc:
                products = products.OrderBy(x => x.Price);
                break;
            case SortState.PriceDesc:
                products = products.OrderByDescending(x => x.Price);
                break;
            case SortState.CategoryAsc:
                products = products.OrderBy(x => x.Category.Name);
                break;
            case SortState.CategoryDesc:
                products = products.OrderByDescending(x => x.Category.Name);
                break;
            case SortState.RateAsc:
                products = products.OrderBy(x => x.Rates.Sum(r => r.Score));
                break;
            case SortState.RateDesc:
                products = products.OrderByDescending(x => x.AverageRate);
                break;
            default:
                products = products.OrderBy(x => x.Producer + x.Model);
                break;
        }

        var count = products.Count();
        var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var model = new IndexViewModel
        {
            PageViewModel = new PageViewModel(count, page, pageSize),
            SortViewModel = new SortViewModel(sortOrder),
            FilterViewModel = new FilterViewModel(
                _uow.CategoryRepository.Get().ToList(),
                category,
                name,
                lowprice,
                upprice,
                lowrate),
            Products = items
        };

        return View(model);
    }
}