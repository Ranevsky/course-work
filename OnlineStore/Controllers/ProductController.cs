using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.Database;
using OnlineStore.Models.ViewModel;

namespace OnlineStore.Controllers;

public class ProductController : Controller
{
    private readonly IUnitOfWork _uow;

    public ProductController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task SaveImage(IFormFile file)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", file.FileName);
        await using var fs = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(fs);
    }

    [Route("Product/{id}")]
    public IActionResult Index(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var product = _uow.ProductRepository.Get(x => x.Id == id, includeProperties: "Image,Rates,Category")
            .FirstOrDefault();
        if (product == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var comments = _uow
            .CommentRepository
            .Get(x => x.ProductId == product.Id, includeProperties: "User,Product")
            .ToList();
        var inpurchase = false;
        var incart = false;
        var commented = false;
        var rated = false;
        var canEditComments = false;
        var canEditProduct = false;

        if (User.Identity is not null && User.Identity.IsAuthenticated)
        {
            var purchases = _uow
                .PurchaseProductRepository
                .Get(x => x.ProductId == product.Id, includeProperties: "Purchase,Product");
                
            var user = _uow
                .UserRepository
                .Get(x => x.Email == User.Identity.Name, includeProperties: "Role")
                .FirstOrDefault();

            if (user is null)
            {
                // Todo: maybe error
                return RedirectToAction("Index", "Home");
            }
                
            inpurchase = purchases.Any(x => x.Purchase.UserId == user.Id);
                
            incart = _uow
                .ShoppingCartRepository
                .Get(x => x.ProductId == product.Id && x.UserId == user.Id && x.Count != 0)
                .Any();

            rated = _uow
                .RateRepository
                .Get(x => x.UserId == user.Id && x.ProductId == product.Id)
                .Any();
                
            canEditComments = user.Role.Name == "Administrator" || user.Role.Name == "Moderator";
            canEditProduct = user.Role.Name == "Administrator";
                
            _uow.Save();
        }

        var model = new ProductViewModel
        {
            Product = product,
            InPurchase = inpurchase,
            InShopingCart = incart,
            Comments = comments,
            Commented = commented,
            Rated = rated,
            CanEditComments = canEditComments,
            CanEditProduct = canEditProduct
        };
        return View(model);
    }

    [Route("product/edit/{id}")]
    public IActionResult EditProduct(int? id)
    {
        Product product = null;
        if (id != null)
        {
            product = _uow
                .ProductRepository
                .Get(x => x.Id == id, includeProperties: "Category,Image")
                .FirstOrDefault();
        }

        var model = new ProductEditModel();

        if (product != null)
        {
            model.Model = product.Model;
            model.Producer = product.Producer;
            model.Price = product.Price;
            model.Id = product.Id;
            model.Description = product.Description;
            model.ImageName = product.Image?.Path;

            var categories = _uow.CategoryRepository.Get().ToList();
            var sel = categories.Where(x => x.Id == product.CategoryId).FirstOrDefault();
            var sellist = new SelectList(categories, "Id", "Name", sel);

            var commenabled = product.CommentsEnabled == true ? "Yes" : "No";

            model.CommentsEnabled = new SelectList(new List<string> { "Yes", "No" }, commenabled);
            model.Categories = sellist;
        }
        else
        {
            model.Categories = new SelectList(_uow.CategoryRepository.Get().ToList(), "Id", "Name");
            model.CommentsEnabled = new SelectList(new List<string> { "Yes", "No" }, "Yes");
        }

        return View("Edit", model);
    }

    [Route("product/save/")]
    public async Task<IActionResult> Save(ProductEditModel edit)
    {
        var user = _uow
            .UserRepository.Get(x => x.Email == User.Identity.Name)
            .FirstOrDefault();
        if (user != null)
        {
            Product product = null;
            if (edit.Id > 0)
            {
                product = _uow
                    .ProductRepository
                    .Get(x => x.Id == edit.Id)
                    .FirstOrDefault();
            }

            var commentsenabled = Request.Form["CommentsEnabled"].ToString();
            var category = int.Parse(Request.Form["Categories"].ToString());
            if (product != null)
            {
                if (edit.Image != null)
                {
                    await SaveImage(edit.Image);
                    _uow.ImageRepository.Insert(new Image { Path = edit.Image.FileName });
                    _uow.Save();
                    product.Image = _uow.ImageRepository.Get(x => x.Path == edit.Image.FileName).FirstOrDefault();
                    product.ImageId = product.Image.Id;
                    _uow.Save();
                }

                product.Model = edit.Model;
                product.Producer = edit.Producer;
                product.Price = edit.Price;
                product.Description = edit.Description;
                product.CommentsEnabled = commentsenabled == "Yes";
                product.CategoryId = category;

                _uow
                    .ProductRepository
                    .Update(product);
                _uow.Save();
            }
            else
            {
                var id = 6; // empty
                if (edit.Image != null)
                {
                    await SaveImage(edit.Image);
                    _uow.ImageRepository.Insert(new Image { Path = edit.Image.FileName });
                    _uow.Save();
                    id = _uow.ImageRepository.Get(x => x.Path == edit.Image.FileName).FirstOrDefault()!.Id;
                }

                product = new Product
                {
                    Model = edit.Model,
                    Producer = edit.Producer,
                    Price = edit.Price,
                    Description = edit.Description,
                    CommentsEnabled = commentsenabled == "Yes",
                    CategoryId = category,
                    CreatorUserId = user.Id,
                    CreationTime = DateTime.Now,
                    ImageId = id,
                };

                _uow.ProductRepository.Insert(product);
                _uow.Save();
            }

            return Redirect("/admin/products");
        }

        return RedirectToAction("Index", "Home");
    }

    [Route("product/comment/edit")]
    public IActionResult EditComment([FromQuery] int productId, [FromQuery] int userId)
    {
        var comment = _uow
            .CommentRepository
            .Get(x => x.ProductId == productId && x.UserId == userId, includeProperties: "Product")
            .FirstOrDefault();
        if (comment == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new CommentModel
        {
            UserId = comment.UserId,
            Text = comment.Text,
            ProductId = comment.ProductId
        };

        return View(model);
    }

    [Route("product/comment/save")]
    public IActionResult SaveComment([FromForm] CommentModel model)
    {
        Debug.WriteLine($"{model.UserId} {model.ProductId} ");
        var comment = _uow
            .CommentRepository
            .Get(x => x.ProductId == model.ProductId && x.UserId == model.UserId)
            .FirstOrDefault();
        if (comment == null)
        {
            return RedirectToAction("Index", "Home");
        }

        comment.Text = model.Text;
        _uow.CommentRepository.Update(comment);
        _uow.Save();
        return Redirect($"/product/{comment.ProductId}");
    }

    [Route("product/comment/remove")]
    public IActionResult RemoveComment(int productId, int userId)
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = _uow
                .UserRepository
                .Get(x => x.Email == User.Identity.Name, includeProperties: "Role")
                .FirstOrDefault();
            if (user.Role.Name == "Administrator" || user.Role.Name == "Moderator")
            {
                var comment = _uow
                    .CommentRepository
                    .Get(x => x.UserId == userId && x.ProductId == productId)
                    .FirstOrDefault();

                var prodId = comment.ProductId;
                _uow.CommentRepository.Delete(comment);
                _uow.Save();
                return Redirect($"/product/{prodId}");
            }
        }

        return RedirectToAction("Index", "Home");
    }
}