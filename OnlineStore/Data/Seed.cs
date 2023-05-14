using System;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models.Database;
using OnlineStore.Services;

namespace OnlineStore.Data;

public class Seed
{
    public void SeedData(ModelBuilder builder)
    {
        // CREATE USER ROLES
        var simpleRole = new UserRole { Id = 1, Name = "SimpleUser" };
        var moderRole = new UserRole { Id = 2, Name = "Moderator" };
        var adminRole = new UserRole { Id = 3, Name = "Administrator" };

        // CREATE CATEGORIES
        var smartphones = new Category { Id = 1, Name = "Smartphone" };

        // CREATE IMAGES
        var empty = new Image { Id = Image.Empty.Id, Path = Image.Empty.Path };
        var iphonexr = new Image { Id = 2, Path = "iphonexr.jpg" };
        var samsung10E = new Image { Id = 3, Path = "samsung10e.jpg" };
        var iphone11 = new Image { Id = 4, Path = "iphone11.jpg" };
        var nokia72 = new Image { Id = 5, Path = "nokia72.jpg" };
        var redmi5 = new Image { Id = 6, Path = "redmi5.jpg" };
        var samsungm30 = new Image { Id = 7, Path = "samsungm30.jpg" };
        var xiaomimi9tpro = new Image { Id = 8, Path = "xiaomimi9tpro.jpg" };
        var xiaomimi10note = new Image { Id = 9, Path = "xiaomimi10note.jpg" };

        #region Users

        // CREATE USERS
        // pasword for the user = 12345678
        // role - simple user
        var user1 = new User
        {
            Id = 1,
            CreationTime = DateTime.Now,
            Name = "Vasyl",
            Surname = "Vlasiuk",
            Email = "vasylvlasiuk@gmail.com",
            PasswordHash = PasswordConverter.Hash("12345678"),
            RoleId = 1,
            Address = ""
        };

        // password = qwerty12
        // role - moderator
        var user2 = new User
        {
            Id = 2,
            CreationTime = DateTime.Now,
            Name = "John",
            Surname = "Doe",
            Email = "johndoe@gmail.com",
            PasswordHash = PasswordConverter.Hash("qwerty12"),
            RoleId = 2,
            Address = ""
        };
        // password = 87654321
        // role = administrator
        var user3 = new User
        {
            Id = 3,
            CreationTime = DateTime.Now,
            Name = "Ostap",
            Surname = "Bondar",
            Email = "ostepbondar@gmail.com",
            PasswordHash = PasswordConverter.Hash("87654321"),
            RoleId = 3,
            Address = ""
        };
        #endregion

        #region Products
        // CREATE PRODUCTS
        var product1 = new Product
        {
            Id = 1,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Apple",
            Model = "iPhone XR 64GB",
            Price = 760.0M,
            Description = "Example of description about a smartphone.",
            ImageId = 2,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        var product2 = new Product
        {
            Id = 2,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Samsung",
            Model = "S10e SM-G970",
            Price = 650.00M,
            Description = "New smartphone Samsung S10e is already in sale.",
            ImageId = 3,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        var product3 = new Product
        {
            Id = 3,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Apple",
            Model = "Iphone 11",
            Price = 899.00M,
            Description = "New smartphone Iphone 11 is already in sale.",
            ImageId = 4,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        var product4 = new Product
        {
            Id = 4,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Nokia",
            Model = "Nokia 72",
            Price = 550.00M,
            Description = "New smartphone Nokia 72 is already in sale.",
            ImageId = 5,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        var product5 = new Product
        {
            Id = 5,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Xiaomi",
            Model = "Redmi 5",
            Price = 333.00M,
            Description = "New smartphone Xiaomi Redmi 5 is already in sale.",
            ImageId = 6,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        var product6 = new Product
        {
            Id = 6,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Samsung",
            Model = "M30",
            Price = 950.00M,
            Description = "New smartphone Samsung M30 is already in sale.",
            ImageId = 7,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        var product7 = new Product
        {
            Id = 7,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Xiaomi",
            Model = "Mi 9T Pro",
            Price = 650.00M,
            Description = "New smartphone Xiaomi Mi 9T Pro is already in sale.",
            ImageId = 8,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        var product8 = new Product
        {
            Id = 8,
            CreatorUserId = 3,
            CategoryId = 1,
            Producer = "Xiaomi",
            Model = "Mi 10 Note",
            Price = 770.00M,
            Description = "New smartphone Xiaomi Mi 10 Note is already in sale.",
            ImageId = 9,
            CreationTime = DateTime.Now,
            CommentsEnabled = true
        };
        #endregion

        builder.Entity<UserRole>().HasData(adminRole, moderRole, simpleRole);
        builder.Entity<Image>().HasData(empty, iphonexr, samsung10E, iphone11, nokia72, redmi5, samsungm30, xiaomimi9tpro, xiaomimi10note);
        builder.Entity<Category>().HasData(smartphones);
        builder.Entity<User>().HasData(user1, user2, user3);
        builder.Entity<Product>().HasData(product1, product2, product3, product4, product5, product6, product7, product8);
    }
}