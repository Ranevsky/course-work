using System;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.Database;

namespace OnlineStore.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly Lazy<IRepository<Category>> _categoryRepository;
    private readonly Lazy<IRepository<Comment>> _commentRepository;
    private readonly ApplicationContext _db;
    private readonly Lazy<IRepository<Image>> _imageRepository;
    private readonly Lazy<IRepository<Product>> _productRepository;
    private readonly Lazy<IRepository<PurchaseProduct>> _purchaseProductRepository;
    private readonly Lazy<IRepository<Purchase>> _purchaseRepository;
    private readonly Lazy<IRepository<Rate>> _rateRepository;
    private readonly Lazy<IRepository<ShoppingCart>> _shoppingCartRepository;
    private readonly Lazy<IRepository<User>> _userRepository;
    private readonly Lazy<IRepository<UserRole>> _userRoleRepository;

    public UnitOfWork(
        ApplicationContext db,
        Lazy<IRepository<Category>> categoryRepository,
        Lazy<IRepository<Comment>> commentRepository,
        Lazy<IRepository<Image>> imageRepository,
        Lazy<IRepository<Product>> productRepository,
        Lazy<IRepository<PurchaseProduct>> purchaseProductRepository,
        Lazy<IRepository<Purchase>> purchaseRepository,
        Lazy<IRepository<Rate>> rateRepository,
        Lazy<IRepository<ShoppingCart>> shoppingCartRepository,
        Lazy<IRepository<User>> userRepository,
        Lazy<IRepository<UserRole>> userRoleRepository)
    {
        _db = db;

        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _imageRepository = imageRepository;
        _productRepository = productRepository;
        _purchaseProductRepository = purchaseProductRepository;
        _purchaseRepository = purchaseRepository;
        _rateRepository = rateRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public void Save()
    {
        _db.SaveChanges();
    }

    public IRepository<Image> ImageRepository => _imageRepository.Value;
    public IRepository<User> UserRepository => _userRepository.Value;
    public IRepository<Product> ProductRepository => _productRepository.Value;
    public IRepository<Rate> RateRepository => _rateRepository.Value;
    public IRepository<Category> CategoryRepository => _categoryRepository.Value;
    public IRepository<Comment> CommentRepository => _commentRepository.Value;
    public IRepository<Purchase> PurchaseRepository => _purchaseRepository.Value;
    public IRepository<PurchaseProduct> PurchaseProductRepository => _purchaseProductRepository.Value;
    public IRepository<ShoppingCart> ShoppingCartRepository => _shoppingCartRepository.Value;
    public IRepository<UserRole> UserRoleRepository => _userRoleRepository.Value;
}