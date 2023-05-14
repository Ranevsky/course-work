using System;

namespace OnlineStore.Models.ViewModel;

public class ProductExportModel
{
    public int Id { get; set; }
    
    public string Producer { get; set; }
    public string Model { get; set; }
    public string Name { get; set; }

    public decimal Price { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }
    public float AverageRate { get; set; }
    
    public int CategoryId { get; set; }
    public int CreatorUserId { get; set; }
}