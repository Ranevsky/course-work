using OnlineStore.Models.Database;

namespace OnlineStore.Data.Interfaces;

public interface IUnitOfWork
{
    public IRepository<Image> ImageRepository { get; }
    public IRepository<Category> CategoryRepository { get; }
    public IRepository<Comment> CommentRepository { get; }
    public IRepository<Product> ProductRepository { get; }
    public IRepository<Purchase> PurchaseRepository { get; }
    public IRepository<PurchaseProduct> PurchaseProductRepository { get; }
    public IRepository<Rate> RateRepository { get; }
    public IRepository<ShoppingCart> ShoppingCartRepository { get; }
    public IRepository<User> UserRepository { get; }
    public IRepository<UserRole> UserRoleRepository { get; }
    void Save();
}