using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.Database;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers;

public class ImportExportController : Controller
{
    private const string Json = "json";
    private const string Xml = "xml";

    private static readonly string[] SupportFormat =
    {
        Json, Xml,
    };

    private readonly IUnitOfWork _uow;

    public ImportExportController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public ActionResult Index()
    {
        var model = new ImportExportModel
        {
            Formats = new SelectList(SupportFormat, SupportFormat.First()),
        };

        return View("Index", model);
    }

    [HttpPost]
    [Route("product/import/")]
    public IActionResult Import(IFormFileCollection files) // , IEnumerable<ProductEditModel> model)
    {
        if (!IsAdministrator())
        {
            return RedirectToAction("Index", "Home");
        }

        var productList = new List<Product>();
        foreach (var file in files)
        {
            var type = file.FileName.Split('.').Last().ToLower();
            using var contentStream = file.OpenReadStream();

            var productImports = type switch
            {
                Json => JsonSerializer.Deserialize<IEnumerable<ProductImportModel>>(contentStream),
                Xml => (IEnumerable<ProductImportModel>)new XmlSerializer(typeof(ProductImportModel[])).Deserialize(
                    contentStream),
                _ => null,
            };

            if (productImports is null)
            {
                throw new NotImplementedException();
            }

            var user = _uow
                .UserRepository.Get(x => x.Email == User.Identity.Name)
                .First();

            var emptyImage = _uow.ImageRepository.GetById(Image.Empty.Id);
            var products = productImports.Select(productImport => new Product
            {
                Model = productImport.Model,
                Producer = productImport.Producer,
                Price = productImport.Price,
                Description = productImport.Description,
                CommentsEnabled = productImport.CommentsEnabled,
                CategoryId = productImport.CategoryId,
                Category = _uow.CategoryRepository.GetById(productImport.CategoryId),
                CreatorUserId = user.Id,
                CreatorUser = user,
                Image = emptyImage,
                ImageId = emptyImage.Id,
            });
            productList.AddRange(products);
        }

        _uow.ProductRepository.InsertMany(productList);
        _uow.Save();

        return RedirectToAction("AdminProducts", "Admin");
    }

    [Route("product/export/")]
    public IActionResult Export(string formats)
    {
        if (!IsAdministrator())
        {
            return RedirectToAction("Index", "Home");
        }

        var products = _uow.ProductRepository.Get().Select(product => new ProductExportModel
        {
            Id = product.Id,
            Producer = product.Producer,
            Model = product.Model,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CreationTime = product.CreationTime,
            AverageRate = product.AverageRate,
            CategoryId = product.CategoryId,
            CreatorUserId = product.CreatorUserId,
        }).ToArray();

        formats = formats.ToLower();
        using var stream = new MemoryStream();
        string contentType;
        switch (formats)
        {
            case Json:
                JsonSerializer.Serialize(stream, products);
                contentType = MediaTypeNames.Application.Json;

                break;

            case Xml:
                new XmlSerializer(typeof(ProductExportModel[])).Serialize(stream, products);
                contentType = MediaTypeNames.Application.Xml;

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(formats));
        }

        return File(stream.ToArray(), contentType, $"data.{formats}");
    }

    private bool IsAdministrator()
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            return true;
        }

        var user = _uow
            .UserRepository
            .Get(x => x.Email == User.Identity.Name, includeProperties: "Role")
            .FirstOrDefault();

        return user?.Role.Name == "Administrator";
    }
}