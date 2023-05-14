using System;

namespace OnlineStore.Models.ViewModel;

public class ProductImportModel
{
    public string Producer { get; set; }
    public string Model { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    
    public DateTime CreationTime { get; set; } = DateTime.Now; // ?
    public bool? CommentsEnabled { get; set; }

    public int CategoryId { get; set; }
    public int CreatorUserId { get; set; }
}