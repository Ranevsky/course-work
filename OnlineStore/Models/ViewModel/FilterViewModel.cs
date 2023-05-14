using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Models.Database;

namespace OnlineStore.Models.ViewModel;

public class FilterViewModel
{
    public FilterViewModel(
        List<Category> categories,
        int? category,
        string name,
        float? lowprice,
        float? upprice,
        float? lowrate
    )
    {
        categories.Insert(0, new Category { Name = "All", Id = 0 });
        Categories = new SelectList(categories, "Id", "Name", category);
        SelectedCategory = category;
        SelectedName = name;
        LowPrice = lowprice;
        UpPrice = upprice;
        LowRate = lowrate;
    }

    public SelectList Categories { get; }
    public int? SelectedCategory { get; }
    public string SelectedName { get; }
    public float? LowPrice { get; }
    public float? UpPrice { get; }
    public float? LowRate { get; }
}