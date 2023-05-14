using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Interfaces;

namespace OnlineStore.Controllers;

public class ImageController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public ImageController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [Route("image/{name}")]
    public IActionResult Get(string name)
    {
        var image = _uow.ImageRepository.Get(x => x.Path == name).FirstOrDefault();
        if (image == null)
        {
            return NotFound();
        }

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", image.Path);
        FileStream img;
        try
        {
            img = System.IO.File.OpenRead(fullPath);
        }
        catch (Exception)
        {
            fullPath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "empty.jpg");
            img = System.IO.File.OpenRead(fullPath);
        }

        return File(img, "image/*");
    }
}