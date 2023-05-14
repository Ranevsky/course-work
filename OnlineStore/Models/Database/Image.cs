using System.Collections.Generic;

namespace OnlineStore.Models.Database;

public class Image
{
    public static class Empty
    {
        public const int Id = 1;
        public const string Path = "empty.jpg";
    }
    
    public int Id { get; set; }
    public string Path { get; set; }
    public ICollection<Product> Products { get; set; }
}