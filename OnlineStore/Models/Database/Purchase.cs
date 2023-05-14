using System;
using System.Collections.Generic;

namespace OnlineStore.Models.Database;

public class Purchase
{
    public int Id { get; set; }
    public string Address { get; set; }
    public decimal FullPrice { get; set; }
    public DateTime CreationTime { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<PurchaseProduct> PurchaseProducts { get; set; }
}